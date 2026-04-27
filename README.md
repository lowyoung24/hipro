# Hide Process Cheat

A kernel-mode driver and a user-mode client with GUI demonstrating process hiding in Windows.

## 🎯 项目概述
本项目包含一个Windows内核驱动程序和带图形界面的用户态客户端，演示如何在Windows系统中隐藏进程。

## ✨ 主要特性

### 内核驱动 (Kernel Driver)
- ✅ 动态计算EPROCESS结构偏移量
- ✅ 从系统活动进程链表中移除进程
- ✅ 对象回调保护（防止终止和内存访问）
- ✅ IOCTL通信接口

### 用户态客户端 (User-mode Client with GUI)
- ✅ 现代化Windows Forms图形界面
- ✅ 实时显示所有运行进程
- ✅ 进程搜索和过滤功能
- ✅ 一键加载/卸载驱动程序
- ✅ 可视化驱动连接状态
- ✅ 管理员权限自动检测
- ✅ 进程隐藏操作确认机制

## 📁 项目结构

```
HideProcessesCheat-master/
├── 驱动部分 (Kernel Driver)
│   ├── Driver.c                    # 驱动主程序
│   ├── common.h                    # 公共头文件
│   ├── callbacks.h                 # 对象回调函数
│   ├── offset.h                    # 偏移量定义
│   └── ObRegisterCallbacks.vcxproj # 驱动项目配置
│
├── 客户端部分 (User-mode Client)
│   ├── Program.cs                  # 应用程序入口
│   ├── MainForm.cs                 # 主界面窗口
│   ├── ClientLib.cs                # 驱动通信库
│   ├── DriverService.cs            # 驱动管理服务
│   └── hipro.csproj    # 客户端项目配置
│
├── build.ps1                       # 快速编译脚本
├── hipro.sln          # Visual Studio解决方案
└── 使用说明.md                     # 详细使用文档
```

## 🚀 快速开始

### 方法一：使用Visual Studio（推荐）

1. **打开解决方案**
   ```
   双击 hipro.sln
   ```

2. **编译项目**
   - 右键点击解决方案 → "生成解决方案"
   - 或按 `Ctrl+Shift+B`

3. **运行客户端**
   - 导航到 `hipro-Client\bin\Release\`
   - 右键点击 `hipro.exe` → "以管理员身份运行"

### 方法二：使用PowerShell脚本

```powershell
# 以管理员身份运行PowerShell
cd c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master

# 执行编译脚本
.\build.ps1
```

### 详细使用说明

请查看 [使用说明.md](./使用说明.md) 获取完整的编译、配置和使用指南。

## ⚙️ 工作原理

1. **驱动加载**: 用户通过GUI选择并加载驱动程序
2. **建立连接**: 客户端自动连接到驱动设备
3. **进程枚举**: 显示所有运行中的进程
4. **隐藏进程**: 
   - 用户选择目标进程
   - 客户端发送PID到驱动
   - 驱动从活动进程链表中移除该进程
5. **结果**: 进程不再出现在任务管理器中

## 🛠️ 技术栈

- **内核驱动**: C, Windows WDK, NT Kernel API
- **客户端**: C#, .NET Framework 4.7.2, Windows Forms
- **开发工具**: Visual Studio 2019/2022, Windows Driver Kit
- **目标系统**: Windows 10/11 x64

## ⚠️ 重要提示

- 🔐 **需要管理员权限**: 程序必须以管理员身份运行
- 📝 **驱动签名**: 需禁用驱动签名强制或使用测试签名
- 🎓 **学习用途**: 本项目仅用于教育和研究目的
- ⚡ **系统稳定**: 避免隐藏关键系统进程

## 📖 学习资源

想了解更多信息？请查阅：
- [使用说明.md](./使用说明.md) - 完整的使用指南
- [Driver.c](./Driver.c) - 驱动源代码
- [MainForm.cs](./MainForm.cs) - UI界面代码
- [ClientLib.cs](./ClientLib.cs) - 驱动通信实现
