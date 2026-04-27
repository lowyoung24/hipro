# 快速编译脚本 - PowerShell

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  HideProcess Cheat - 快速编译工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查是否以管理员身份运行
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "警告: 建议以管理员身份运行此脚本" -ForegroundColor Yellow
    Write-Host ""
}

# 获取当前目录
$projectRoot = $PSScriptRoot
Write-Host "项目路径: $projectRoot" -ForegroundColor Green
Write-Host ""

# 选择编译模式
Write-Host "请选择编译模式:" -ForegroundColor Cyan
Write-Host "1. 仅编译客户端 (推荐初学者)" -ForegroundColor White
Write-Host "2. 编译驱动 + 客户端 (需要WDK)" -ForegroundColor White
Write-Host "3. 清理所有编译输出" -ForegroundColor White
Write-Host ""

$choice = Read-Host "请输入选项 (1/2/3)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "正在编译客户端程序..." -ForegroundColor Yellow
        
        # 检查MSBuild
        $msbuildPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
        
        if (-not $msbuildPath) {
            Write-Host "错误: 未找到MSBuild，请确认已安装Visual Studio" -ForegroundColor Red
            pause
            exit 1
        }
        
        Write-Host "使用MSBuild: $msbuildPath" -ForegroundColor Gray
        
        # 编译客户端
        & $msbuildPath "$projectRoot\HideProcessClient.csproj" /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "✓ 客户端编译成功!" -ForegroundColor Green
            Write-Host "输出位置: $projectRoot\HideProcessClient\bin\Release\HideProcessClient.exe" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "✗ 客户端编译失败" -ForegroundColor Red
        }
    }
    
    "2" {
        Write-Host ""
        Write-Host "正在编译驱动和客户端..." -ForegroundColor Yellow
        
        # 检查WDK
        $wdkInstalled = Test-Path "C:\Program Files (x86)\Windows Kits\10\VSMajorMinor.txt"
        if (-not $wdkInstalled) {
            Write-Host "错误: 未检测到WDK，请先安装Windows Driver Kit" -ForegroundColor Red
            Write-Host "下载地址: https://docs.microsoft.com/windows-hardware/drivers/download-the-wdk" -ForegroundColor Yellow
            pause
            exit 1
        }
        
        Write-Host "步骤1: 编译驱动程序..." -ForegroundColor Cyan
        msbuild "$projectRoot\ObRegisterCallbacks.vcxproj" /p:Configuration=Release /p:Platform=x64 /t:Rebuild
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "✗ 驱动编译失败" -ForegroundColor Red
            pause
            exit 1
        }
        
        Write-Host "✓ 驱动编译成功" -ForegroundColor Green
        Write-Host ""
        
        Write-Host "步骤2: 编译客户端程序..." -ForegroundColor Cyan
        msbuild "$projectRoot\HideProcessClient.csproj" /p:Configuration=Release /p:Platform="Any CPU" /t:Rebuild
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "✗ 客户端编译失败" -ForegroundColor Red
            pause
            exit 1
        }
        
        Write-Host "✓ 客户端编译成功" -ForegroundColor Green
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  ✓ 全部编译完成!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "驱动文件: $projectRoot\x64\Release\hipro.sys" -ForegroundColor White
        Write-Host "客户端程序: $projectRoot\hipro-Client\bin\Release\hipro.exe" -ForegroundColor White
    }
    
    "3" {
        Write-Host ""
        Write-Host "正在清理编译输出..." -ForegroundColor Yellow
        
        # 清理驱动
        if (Test-Path "$projectRoot\x64") {
            Remove-Item "$projectRoot\x64" -Recurse -Force
            Write-Host "✓ 已清理驱动输出" -ForegroundColor Green
        }
        
        if (Test-Path "$projectRoot\Debug") {
            Remove-Item "$projectRoot\Debug" -Recurse -Force
            Write-Host "✓ 已清理Debug输出" -ForegroundColor Green
        }
        
        # 清理客户端
        if (Test-Path "$projectRoot\HideProcessClient\bin") {
            Remove-Item "$projectRoot\HideProcessClient\bin" -Recurse -Force
            Write-Host "✓ 已清理客户端输出" -ForegroundColor Green
        }
        
        if (Test-Path "$projectRoot\HideProcessClient\obj") {
            Remove-Item "$projectRoot\HideProcessClient\obj" -Recurse -Force
            Write-Host "✓ 已清理客户端临时文件" -ForegroundColor Green
        }
        
        Write-Host ""
        Write-Host "✓ 清理完成" -ForegroundColor Green
    }
    
    default {
        Write-Host "无效的选项" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "按任意键退出..." -ForegroundColor Gray
pause | Out-Null
