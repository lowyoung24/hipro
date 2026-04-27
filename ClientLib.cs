using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace hipro_Client
{
    /// <summary>
    /// 与内核驱动通信的客户端类
    /// </summary>
    public class DriverClient : IDisposable
    {
        private const string DEVICE_PATH = @"\\.\hiproSL";
        private SafeFileHandle _deviceHandle;
        private bool _disposed = false;

        // Windows API
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            IntPtr lpOutBuffer,
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        // 访问权限常量
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;
        private const uint OPEN_EXISTING = 3;

        /// <summary>
        /// 是否已连接到驱动
        /// </summary>
        public bool IsConnected 
        { 
            get { return _deviceHandle != null && !_deviceHandle.IsInvalid; }
        }

        /// <summary>
        /// 连接到驱动程序
        /// </summary>
        public bool Connect()
        {
            try
            {
                _deviceHandle = CreateFile(
                    DEVICE_PATH,
                    GENERIC_READ | GENERIC_WRITE,
                    FILE_SHARE_READ | FILE_SHARE_WRITE,
                    IntPtr.Zero,
                    OPEN_EXISTING,
                    0,
                    IntPtr.Zero);

                if (_deviceHandle.IsInvalid)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception(string.Format("无法连接到驱动，错误代码: {0}", error));
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("连接驱动失败: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 隐藏指定PID的进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns>是否成功</returns>
        public bool HideProcess(uint processId)
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("未连接到驱动程序");
            }

            try
            {
                // IOCTL码：使用CTL_CODE宏计算
                // CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)
                uint ioctlCode = 0x22A000; // FILE_DEVICE_UNKNOWN=0x22, 0x800<<2=0x2000, METHOD_BUFFERED=0, FILE_ANY_ACCESS=0

                uint bytesReturned;
                bool result = DeviceIoControl(
                    _deviceHandle,
                    ioctlCode,
                    IntPtr.Zero,
                    0,
                    IntPtr.Zero,
                    0,
                    out bytesReturned,
                    IntPtr.Zero);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("隐藏进程失败: {0}", ex.Message));
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (_deviceHandle != null && !_deviceHandle.IsInvalid)
            {
                _deviceHandle.Dispose();
                _deviceHandle = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                }
                _disposed = true;
            }
        }

        ~DriverClient()
        {
            Dispose(false);
        }
    }
}
