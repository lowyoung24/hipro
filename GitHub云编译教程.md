# 🚀 GitHub云编译完整教程

##  目标
在GitHub上免费编译驱动和用户程序，无需安装任何开发工具！

---

## 📋 完整步骤（跟着做就行）

### 第1步：上传代码到GitHub

#### 方法A：使用Git命令（推荐）

打开PowerShell，执行：

```powershell
# 进入项目目录
cd "c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master"

# 初始化Git仓库
git init

# 添加所有文件
git add .

# 提交
git commit -m "Initial commit - hipro project"

# 添加GitHub远程仓库（替换成你的仓库地址）
git remote add origin https://github.com/lowyoung24/hipro.git

# 推送到GitHub
git push -u origin main
```

#### 方法B：使用GitHub Desktop（更简单）

1. 下载 GitHub Desktop：https://desktop.github.com/
2. 安装并登录你的GitHub账号
3. 点击 "Add an Existing Repository"
4. 选择项目文件夹：`c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master`
5. 点击 "Publish repository"
6. 填写仓库名称（如：hipro）
7. 点击 "Publish"

#### 方法C：网页上传（最简单）

1. 在GitHub点击绿色 **"New"** 按钮创建新仓库
2. 仓库名填写：`hipro`
3. 点击 **"Create repository"**
4. 点击 **"uploading an existing file"**
5. 拖入项目文件夹中的所有文件
6. 点击 **"Commit changes"**

---

### 第2步：配置GitHub Actions

#### 2.1 确认工作流文件已上传

确保以下文件已上传到GitHub：
```
.github/workflows/build.yml  ← 这是自动编译配置
```

#### 2.2 启用GitHub Actions

1. 进入你的仓库页面
2. 点击顶部 **"Actions"** 标签
3. 如果是第一次使用，点击 **"I understand my workflows, go ahead and enable them"**

---

### 第3步：触发编译

#### 方法A：自动触发

每次你推送代码到GitHub时，会自动开始编译！

#### 方法B：手动触发（推荐）

1. 进入仓库 → **Actions** 标签
2. 左侧选择 **"Build Windows Driver"**
3. 点击右侧 **"Run workflow"** 按钮
4. 选择分支（main或master）
5. 点击绿色 **"Run workflow"**

---

### 第4步：等待编译完成

#### 查看进度

1. 点击正在运行的工作流
2. 查看每个步骤的状态：
   ```
   ✅ Checkout code          - 下载代码
   ✅ Setup MSBuild          - 安装编译工具
   ✅ Setup Visual Studio    - 安装VS环境
   ✅ Build Driver           - 编译驱动 ← 关键步骤
   ✅ Upload Driver Artifact - 上传驱动文件
   ✅ Build Client           - 编译客户端
   ✅ Upload Client Artifact - 上传客户端文件
   ```

#### 编译时间
- 第一次：约5-8分钟（需要安装环境）
- 后续：约2-3分钟

---

### 第5步：下载编译产物

编译成功后：

1. 点击完成的工作流
2. 在页面底部找到 **"Artifacts"** 区域
3. 你会看到两个文件：
   ```
   📦 hipro-Driver  ← 驱动文件
   📦 hipro-Client          ← 客户端程序
   ```

4. 点击下载按钮
5. 解压ZIP文件

#### 解压后得到：
```
hipro-Driver/
└── hipro.sys  ← 这就是你要的驱动！

hipro-Client/
└── hipro.exe   ← 用户界面程序
```

---

##  详细说明

### 工作流配置文件解析

`.github/workflows/build.yml` 文件的作用：

```yaml
name: Build Windows Driver        # 工作流名称

on:
  push:                           # 触发条件
    branches: [ main, master ]    # 推送到main或master分支时
  workflow_dispatch:              # 允许手动触发

jobs:
  build-driver:
    runs-on: windows-latest       # 使用Windows虚拟机
    
    steps:
    - 下载代码
    - 安装MSBuild（编译工具）
    - 安装Visual Studio环境
    - 编译驱动（.sys文件）
    - 上传驱动文件（保存7天）
    - 编译客户端（.exe文件）
    - 上传客户端文件（保存7天）
```

---

## ⚠️ 注意事项

### 1. GitHub Actions使用限制

**免费额度**：
- 每月2000分钟编译时间
- 每次编译约5-8分钟
- 可以编译约250-400次/月

**足够用吗？**
- ✅ 学习使用：完全够用！
- ✅ 偶尔修改：没问题！
- ❌ 频繁编译：需要注意

### 2. 文件保存时间

- 编译产物保存 **7天**
- 下载后请保存到本地
- 超过7天会自动删除

### 3. 网络问题

如果GitHub访问慢：
- 使用代理
- 或等待网络好的时候
- 或使用GitHub镜像站

---

##  故障排除

### 问题1：编译失败

**症状**：Actions显示红色叉号

**解决**：
1. 点击失败的工作流
2. 查看错误日志
3. 常见问题：
   - 文件缺失 → 重新上传完整代码
   - 语法错误 → 检查代码
   - 环境问题 → 等待重试

### 问题2：找不到Artifacts

**症状**：编译成功但没看到下载链接

**解决**：
1. 确认工作流完全成功（全部绿色✓）
2. 刷新页面
3. 检查是否被自动清理（超过7天）

### 问题3：编译时间太长

**症状**：超过15分钟还在运行

**解决**：
1. 第一次编译需要安装环境，会比较慢
2. 后续编译会快很多
3. 如果卡住，可以取消重试

---

## 💡 小技巧

### 技巧1：只编译驱动

如果只需要驱动，修改 `build.yml`：

```yaml
# 注释掉客户端编译部分
# - name: Build Client
#   run: |
#     ...
```

### 技巧2：自动发布Release

可以配置自动创建Release并附加编译产物，方便下载。

### 技巧3：多平台编译

可以同时编译x64和x86版本：

```yaml
strategy:
  matrix:
    platform: [x64, x86]
```

---

##  完整操作示例

### 从零开始的完整流程

```powershell
# 1. 进入项目目录
cd "c:\Users\User\Downloads\HideProcessesCheat-master\HideProcessesCheat-master"

# 2. 初始化Git
git init

# 3. 添加文件
git add .

# 4. 提交
git commit -m "Add project files"

# 5. 创建GitHub仓库（在网页上）
# 访问: https://github.com/new
# 仓库名: hipro
# 不要勾选"Add a README file"

# 6. 关联远程仓库
git remote add origin https://github.com/lowyoung24/hipro.git

# 7. 推送代码
git branch -M main
git push -u origin main

# 8. 等待自动编译
# 进入GitHub仓库 → Actions → 查看进度

# 9. 下载编译产物
# Actions → 点击完成的工作流 → 下载Artifacts
```

---

## 🎯 快速参考

### 一键操作清单

- [ ] 创建GitHub仓库
- [ ] 上传项目代码
- [ ] 确认 `.github/workflows/build.yml` 存在
- [ ] 启用GitHub Actions
- [ ] 触发编译（手动或自动）
- [ ] 等待编译完成（5-8分钟）
- [ ] 下载Artifacts
- [ ] 解压得到 `.sys` 文件
- [ ] 使用驱动加载程序

### 重要链接

- GitHub主页：https://github.com/lowyoung24
- 你的仓库：https://github.com/lowyoung24/hipro（创建后）
- Actions文档：https://docs.github.com/actions

---

## 📞 需要帮助？

如果遇到任何问题：
1. 截图错误信息
2. 告诉我你卡在哪一步
3. 我会帮你解决！

---

## ✅ 总结

**云编译的优势**：
- ✅ 零安装，无需本地开发环境
- ✅ 完全免费（每月2000分钟）
- ✅ 自动化，推送即编译
- ✅ 云端环境，不会污染本地

**适合人群**：
- 不想安装Visual Studio的用户
- 只是想快速获得编译产物的用户
- 学习GitHub Actions的用户

**现在就开始吧！** 🚀

创建仓库 → 上传代码 → 触发编译 → 下载驱动 → 完成！
