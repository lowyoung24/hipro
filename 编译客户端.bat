@echo off
chcp 65001 >nul
echo ========================================
echo   HideProcess Client - 快速编译工具
echo ========================================
echo.

set "ProjectDir=%~dp0"
set "CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set "OutputDir=%ProjectDir%Bin"
set "OutputExe=%OutputDir%\hipro.exe"

REM 检查编译器
if not exist "%CSC%" (
    echo [错误] 未找到C#编译器
    echo 请确保已安装 .NET Framework 4.x
    pause
    exit /b 1
)

echo [信息] 使用编译器: %CSC%
echo.

REM 创建输出目录
if not exist "%OutputDir%" mkdir "%OutputDir%"

REM 检查源文件
for %%f in (Program.cs MainForm.cs ClientLib.cs DriverService.cs) do (
    if not exist "%ProjectDir%%%f" (
        echo [错误] 缺少源文件: %%f
        pause
        exit /b 1
    )
)

echo [信息] 正在编译...
echo.

REM 编译命令
"%CSC%" ^
    /target:winexe ^
    /out:"%OutputExe%" ^
    /platform:x64 ^
    /optimize+ ^
    /warn:4 ^
    /utf8output ^
    /reference:System.dll ^
    /reference:System.Core.dll ^
    /reference:System.Windows.Forms.dll ^
    /reference:System.Drawing.dll ^
    /reference:System.ServiceProcess.dll ^
    "%ProjectDir%Program.cs" ^
    "%ProjectDir%MainForm.cs" ^
    "%ProjectDir%ClientLib.cs" ^
    "%ProjectDir%DriverService.cs"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   ✓ 编译成功!
    echo ========================================
    echo.
    echo 可执行文件位置:
    echo %OutputExe%
    echo.
    
    REM 显示文件大小
    for %%A in ("%OutputExe%") do (
        set FileSize=%%~zA
    )
    set /a FileSizeKB=%FileSize% / 1024
    echo 文件大小: %FileSizeKB% KB
    echo.
    
    choice /C YN /M "是否立即运行程序"
    if errorlevel 2 goto :end
    if errorlevel 1 (
        echo.
        echo [信息] 正在启动程序（需要管理员权限）...
        powershell -Command "Start-Process '%OutputExe%' -Verb RunAs"
    )
) else (
    echo.
    echo ✗ 编译失败
    echo.
    echo 请检查错误信息并重试
)

:end
echo.
pause
