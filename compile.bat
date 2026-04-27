# 简单编译脚本 - 使用系统自带的C#编译器

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  HideProcess Client - 编译工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 设置路径
$projectRoot = $PSScriptRoot
$cscPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
$outputDir = Join-Path $projectRoot "Bin"
$outputExe = Join-Path $outputDir "hipro.exe"

# 检查编译器
if (-not (Test-Path $cscPath)) {
    Write-Host "错误: 未找到C#编译器" -ForegroundColor Red
    Write-Host "请安装 .NET Framework 4.x" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "使用编译器: $cscPath" -ForegroundColor Green
Write-Host ""

# 创建输出目录
if (-not (Test-Path $outputDir)) {
    New-Item -ItemType Directory -Path $outputDir | Out-Null
    Write-Host "创建输出目录: $outputDir" -ForegroundColor Gray
}

# 源文件列表
$sources = @(
    "Program.cs",
    "MainForm.cs",
    "ClientLib.cs",
    "DriverService.cs"
)

# 检查源文件是否存在
$missingFiles = @()
foreach ($file in $sources) {
    $fullPath = Join-Path $projectRoot $file
    if (-not (Test-Path $fullPath)) {
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "错误: 缺少以下源文件:" -ForegroundColor Red
    foreach ($file in $missingFiles) {
        Write-Host "  - $file" -ForegroundColor Red
    }
    pause
    exit 1
}

# 构建编译命令
$sourceFiles = $sources | ForEach-Object { Join-Path $projectRoot $_ }
$referenceAssemblies = @(
    "System.dll",
    "System.Core.dll",
    "System.Windows.Forms.dll",
    "System.Drawing.dll",
    "System.ServiceProcess.dll"
)

$refs = $referenceAssemblies | ForEach-Object { "/reference:$_" }

# 编译命令参数
$arguments = @(
    "/target:winexe"
    "/out:`"$outputExe`""
    "/platform:x64"
    "/optimize+"
    "/warn:4"
    "/define:TRACE"
    "/utf8output"
) + $refs + $sourceFiles

Write-Host "正在编译..." -ForegroundColor Yellow
Write-Host ""

# 执行编译
$process = Start-Process -FilePath $cscPath `
    -ArgumentList $arguments `
    -Wait `
    -PassThru `
    -NoNewWindow

if ($process.ExitCode -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✓ 编译成功!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "可执行文件位置:" -ForegroundColor White
    Write-Host "$outputExe" -ForegroundColor Cyan
    Write-Host ""
    
    # 显示文件信息
    $fileInfo = Get-Item $outputExe
    Write-Host "文件大小: $([math]::Round($fileInfo.Length / 1KB, 2)) KB" -ForegroundColor Gray
    Write-Host "创建时间: $($fileInfo.CreationTime)" -ForegroundColor Gray
    Write-Host ""
    
    # 询问是否运行
    $runNow = Read-Host "是否立即运行程序? (Y/N)"
    if ($runNow -eq "Y" -or $runNow -eq "y") {
        Write-Host ""
        Write-Host "正在启动程序..." -ForegroundColor Yellow
        Start-Process $outputExe -Verb RunAs
    }
} else {
    Write-Host ""
    Write-Host "✗ 编译失败，退出代码: $($process.ExitCode)" -ForegroundColor Red
    Write-Host ""
    Write-Host "请检查错误信息并重试" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "按任意键退出..." -ForegroundColor Gray
pause | Out-Null
