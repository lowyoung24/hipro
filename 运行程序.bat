@echo off
chcp 65001 >nul
echo ========================================
echo   启动 HideProcess Client
echo ========================================
echo.

set "ExePath=%~dp0Bin\hipro.exe"

if not exist "%ExePath%" (
    echo [错误] 未找到可执行文件: %ExePath%
    echo.
    echo 请先运行 "编译客户端.bat" 进行编译
    pause
    exit /b 1
)

echo [信息] 正在启动程序（需要管理员权限）...
echo.

powershell -Command "Start-Process '%ExePath%' -Verb RunAs"

echo [提示] 程序已启动，请查看UAC提示并允许运行
echo.
pause
