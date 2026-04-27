# Git安装指南

## 📥 下载地址

### 官方下载
- **网址**：https://git-scm.com/download/win
- **自动检测**：打开网页会自动开始下载

### 备用下载
- **GitHub Release**：https://github.com/git-for-windows/git/releases
- **选择版本**：下载最新的 `Git-2.x.x-64-bit.exe`

---

## 🚀 安装步骤

### 1. 运行安装程序
```
双击: Git-2.x.x-64-bit.exe
```

### 2. 选择安装位置
```
默认: C:\Program Files\Git
建议: 保持默认即可
```

### 3. 选择组件（重要！）

在"Select Components"页面，确保勾选：

```
✅ Git Bash Here              - Git命令行工具
✅ Git GUI Here               - Git图形界面
✅ Associate .git* files      - 关联.git文件
✅ Associate .sh files        - 关联.sh文件
✅ Check daily for updates    - 每日检查更新（可选）
```

### 4. 选择默认编辑器

```
选择: Use Vim (the ubiquitous text editor) as Git's default editor
或者: Use Visual Studio Code as Git's default editor（如果你安装了VS Code）
```

### 5. 初始分支名称

```
选择: Let Git decide（推荐）
或者: Override the default branch name for new repositories: main
```

### 6. 调整PATH环境变量（重要！）

```
选择第二个选项: 
✅ Git from the command line and also from 3rd-party software
```

这个选项让你可以在PowerShell和CMD中使用Git命令。

### 7. 选择SSH客户端

```
选择: Use OpenSSH（推荐）
```

### 8. 选择HTTPS传输后端

```
选择: Use the OpenSSL library（默认）
```

### 9. 配置行尾转换（重要！）

```
选择第二个选项:
✅ Checkout Windows-style, commit Unix-style line endings
```

这可以确保在Windows和Linux之间兼容。

### 10. 终端模拟器

```
选择: Use Windows' default console window
```

### 11. 额外选项

```
建议勾选:
✅ Enable file system caching
✅ Enable symbolic links（如果可用）
```

### 12. 实验性功能（可选）

```
不勾选任何实验性选项（除非你明确知道用途）
```

### 13. 点击"Install"开始安装

```
等待时间: 2-5分钟
```

---

## ✅ 验证安装

安装完成后，打开**PowerShell**，执行：

```powershell
# 检查Git版本
git --version

# 应该显示类似:
# git version 2.43.0.windows.1
```

---

## 🔧 配置Git（首次使用）

### 1. 设置用户名

```powershell
git config --global user.name "你的GitHub用户名"
# 例如: git config --global user.name "lowyoung24"
```

### 2. 设置邮箱

```powershell
git config --global user.email "你的邮箱@example.com"
# 例如: git config --global user.email "lowyoung24@github.com"
```

### 3. 验证配置

```powershell
git config --list
```

---

## 🎯 使用Git上传项目

### 方法一：完整流程

```powershell
# 1. 进入项目目录
cd "c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master"

# 2. 初始化Git仓库
git init

# 3. 添加所有文件
git add .

# 4. 提交更改
git commit -m "Initial commit - hipro project"

# 5. 关联GitHub仓库
git remote add origin https://github.com/lowyoung24/hipro.git
# （替换为你的GitHub用户名和仓库名）

# 6. 推送到GitHub
git branch -M main
git push -u origin main
```

### 方法二：如果已有远程仓库

```powershell
# 如果仓库已在GitHub创建
cd "c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master"
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/你的用户名/hipro.git
git branch -M main
git push -u origin main
```

---

## ⚠️ 常见问题

### 问题1：git命令未找到

**症状**：提示"无法将git项识别为cmdlet"

**解决**：
1. 重启PowerShell
2. 重新打开终端
3. 检查安装路径是否添加到PATH

### 问题2：推送时要求输入密码

**症状**：`remote: Support for password authentication was removed`

**解决**：使用GitHub Personal Access Token
1. 访问：https://github.com/settings/tokens
2. 生成新Token（勾选repo权限）
3. 推送时使用Token代替密码

### 问题3：大文件推送失败

**症状**：`fatal: the remote end hung up unexpectedly`

**解决**：
```powershell
# 增加缓冲区大小
git config --global http.postBuffer 524288000
```

### 问题4：分支名称问题

**症状**：推送时分支名称不匹配

**解决**：
```powershell
# 重命名分支
git branch -M main
git push -u origin main
```

---

## 🔐 配置GitHub认证（推荐）

### 方法一：使用Personal Access Token

1. 创建Token：
   - 访问：https://github.com/settings/tokens
   - 点击"Generate new token (classic)"
   - 勾选权限：`repo`, `workflow`
   - 生成并复制Token

2. 使用Token推送：
   ```powershell
   git push https://你的用户名:TOKEN@github.com/你的用户名/hipro.git
   ```

### 方法二：使用Git Credential Manager

```powershell
# 启用凭证管理器
git config --global credential.helper manager
```

---

## 📚 Git常用命令

### 基本操作
```powershell
# 查看状态
git status

# 查看日志
git log --oneline

# 撤销更改
git checkout -- 文件名

# 暂存更改
git stash

# 恢复暂存
git stash pop
```

### 分支管理
```powershell
# 创建分支
git branch 分支名

# 切换分支
git checkout 分支名

# 创建并切换
git checkout -b 分支名

# 查看分支
git branch
```

---

## 🎓 学习资源

### 官方文档
- Git Book: https://git-scm.com/book/zh/v2
- 中文版: https://git-scm.com/book/zh/v2

### 视频教程
- B站搜索："Git教程 从入门到精通"
- YouTube: "Git and GitHub for Beginners"

### 练习平台
- Learn Git Branching: https://learngitbranching.js.org/
- GitHub Learning Lab: https://lab.github.com/

---

## ✅ 安装检查清单

- [ ] Git已下载
- [ ] 安装程序已运行
- [ ] PATH环境变量已配置
- [ ] PowerShell中可以执行`git --version`
- [ ] 已配置用户名和邮箱
- [ ] 可以成功克隆仓库
- [ ] 可以成功推送代码

---

## 🚀 快速开始

### 最简流程（3步）

```powershell
# 1. 下载安装Git
# 从 https://git-scm.com/download/win 下载并安装

# 2. 配置基本信息
git config --global user.name "你的用户名"
git config --global user.email "你的邮箱"

# 3. 上传项目
cd "c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master"
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/lowyoung24/hipro.git
git branch -M main
git push -u origin main
```

---

**安装完成后，告诉我你的GitHub用户名，我帮你完成上传！** 🎉
