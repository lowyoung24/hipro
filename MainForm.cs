using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace hipro_Client
{
    public class MainForm : Form
    {
        private DriverClient _driverClient;
        private DataGridView _processGrid;
        private Button _btnRefresh;
        private Button _btnHideProcess;
        private Button _btnLoadDriver;
        private Button _btnUnloadDriver;
        private Label _lblStatus;
        private Label _lblDriverStatus;
        private TextBox _txtSearch;
        private StatusStrip _statusStrip;
        private ToolStripStatusLabel _toolStripStatus;

        public MainForm()
        {
            _driverClient = new DriverClient();
            InitializeComponents();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        private void InitializeComponents()
        {
            // 窗体设置
            this.Text = "Hipro";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            // 顶部面板
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.Padding = new Padding(10);

            // 驱动状态标签
            _lblDriverStatus = new Label();
            _lblDriverStatus.Text = "驱动状态: 未加载";
            _lblDriverStatus.ForeColor = Color.Red;
            _lblDriverStatus.AutoSize = true;
            _lblDriverStatus.Location = new Point(10, 10);

            // 连接状态标签
            _lblStatus = new Label();
            _lblStatus.Text = "连接状态: 未连接";
            _lblStatus.ForeColor = Color.Red;
            _lblStatus.AutoSize = true;
            _lblStatus.Location = new Point(10, 35);

            // 搜索框
            _txtSearch = new TextBox();
            _txtSearch.Location = new Point(200, 10);
            _txtSearch.Size = new Size(200, 25);
            _txtSearch.TextChanged += TxtSearch_TextChanged;

            // 刷新按钮
            _btnRefresh = new Button();
            _btnRefresh.Text = "刷新进程列表";
            _btnRefresh.Location = new Point(420, 8);
            _btnRefresh.Size = new Size(120, 30);
            _btnRefresh.Click += BtnRefresh_Click;

            // 加载驱动按钮
            _btnLoadDriver = new Button();
            _btnLoadDriver.Text = "加载驱动";
            _btnLoadDriver.Location = new Point(560, 8);
            _btnLoadDriver.Size = new Size(100, 30);
            _btnLoadDriver.BackColor = Color.LightGreen;
            _btnLoadDriver.Click += BtnLoadDriver_Click;

            // 卸载驱动按钮
            _btnUnloadDriver = new Button();
            _btnUnloadDriver.Text = "卸载驱动";
            _btnUnloadDriver.Location = new Point(680, 8);
            _btnUnloadDriver.Size = new Size(100, 30);
            _btnUnloadDriver.BackColor = Color.LightCoral;
            _btnUnloadDriver.Enabled = false;
            _btnUnloadDriver.Click += BtnUnloadDriver_Click;

            topPanel.Controls.AddRange(new Control[] {
                _lblDriverStatus, _lblStatus, _txtSearch, 
                _btnRefresh, _btnLoadDriver, _btnUnloadDriver
            });

            // 进程列表网格
            _processGrid = new DataGridView();
            _processGrid.Dock = DockStyle.Fill;
            _processGrid.AllowUserToAddRows = false;
            _processGrid.AllowUserToDeleteRows = false;
            _processGrid.ReadOnly = true;
            _processGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _processGrid.MultiSelect = false;
            _processGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _processGrid.BackgroundColor = Color.White;
            _processGrid.GridColor = Color.LightGray;
            _processGrid.RowHeadersVisible = false;

            // 添加列
            _processGrid.Columns.Add("PID", "进程ID");
            _processGrid.Columns.Add("ProcessName", "进程名称");
            _processGrid.Columns.Add("FullPath", "完整路径");
            
            _processGrid.Columns["PID"].Width = 100;
            _processGrid.Columns["ProcessName"].Width = 200;
            _processGrid.Columns["FullPath"].Width = 500;

            // 底部面板
            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 60;
            bottomPanel.Padding = new Padding(10);

            // 隐藏进程按钮
            _btnHideProcess = new Button();
            _btnHideProcess.Text = "隐藏选中进程";
            _btnHideProcess.Location = new Point(10, 15);
            _btnHideProcess.Size = new Size(150, 35);
            _btnHideProcess.BackColor = Color.Orange;
            _btnHideProcess.Font = new Font(_btnHideProcess.Font, FontStyle.Bold);
            _btnHideProcess.Click += BtnHideProcess_Click;
            _btnHideProcess.Enabled = false;

            bottomPanel.Controls.Add(_btnHideProcess);

            // 状态栏
            _statusStrip = new StatusStrip();
            _toolStripStatus = new ToolStripStatusLabel();
            _toolStripStatus.Text = "就绪";
            _statusStrip.Items.Add(_toolStripStatus);

            // 添加控件到窗体
            this.Controls.Add(_processGrid);
            this.Controls.Add(bottomPanel);
            this.Controls.Add(topPanel);
            this.Controls.Add(_statusStrip);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateDriverStatus();
            RefreshProcessList();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_driverClient != null)
                _driverClient.Dispose();
        }

        private void UpdateDriverStatus()
        {
            bool isLoaded = DriverService.IsDriverLoaded();
            
            if (isLoaded)
            {
                _lblDriverStatus.Text = "驱动状态: 已加载 ✓";
                _lblDriverStatus.ForeColor = Color.Green;
                _btnLoadDriver.Enabled = false;
                _btnUnloadDriver.Enabled = true;

                // 尝试连接
                try
                {
                    if (!_driverClient.IsConnected)
                    {
                        _driverClient.Connect();
                    }
                    _lblStatus.Text = "连接状态: 已连接 ✓";
                    _lblStatus.ForeColor = Color.Green;
                    _btnHideProcess.Enabled = true;
                }
                catch (Exception ex)
                {
                    _lblStatus.Text = "连接状态: 失败 ✗";
                    _lblStatus.ForeColor = Color.Red;
                    ShowError(string.Format("连接驱动失败: {0}", ex.Message));
                }
            }
            else
            {
                _lblDriverStatus.Text = "驱动状态: 未加载 ✗";
                _lblDriverStatus.ForeColor = Color.Red;
                _lblStatus.Text = "连接状态: 未连接";
                _lblStatus.ForeColor = Color.Gray;
                _btnLoadDriver.Enabled = true;
                _btnUnloadDriver.Enabled = false;
                _btnHideProcess.Enabled = false;
            }
        }

        private void RefreshProcessList()
        {
            try
            {
                _processGrid.Rows.Clear();
                
                Process[] processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    try
                    {
                        string processName = process.ProcessName;
                        string fullPath = "";
                        
                        try
                        {
                            if (process.MainModule != null)
                                fullPath = process.MainModule.FileName;
                            else
                                fullPath = "无法访问";
                        }
                        catch
                        {
                            fullPath = "需要管理员权限";
                        }

                        int rowIndex = _processGrid.Rows.Add(
                            process.Id,
                            processName,
                            fullPath
                        );

                        // 标记系统进程
                        if (processName.ToLower() == "system" || processName.ToLower() == "idle")
                        {
                            _processGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                    }
                    catch
                    {
                        // 跳过无法访问的进程
                    }
                }

                _toolStripStatus.Text = string.Format("共找到 {0} 个进程", processes.Length);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("刷新进程列表失败: {0}", ex.Message));
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = _txtSearch.Text.ToLower();
            
            foreach (DataGridViewRow row in _processGrid.Rows)
            {
                string processName = row.Cells["ProcessName"].Value.ToString().ToLower();
                string pid = row.Cells["PID"].Value.ToString();
                
                if (processName.Contains(searchText) || pid.Contains(searchText))
                {
                    row.Visible = true;
                }
                else
                {
                    row.Visible = false;
                }
            }
        }

        private void BtnLoadDriver_Click(object sender, EventArgs e)
        {
            try
            {
                // 使用文件对话框选择驱动文件
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "驱动文件 (*.sys)|*.sys|所有文件 (*.*)|*.*";
                    dialog.Title = "选择驱动程序文件";
                    
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        _toolStripStatus.Text = "正在加载驱动...";
                        Application.DoEvents();

                        bool success = DriverService.LoadDriver(dialog.FileName);
                        
                        if (success)
                        {
                            ShowSuccess("驱动加载成功！");
                            UpdateDriverStatus();
                        }
                        else
                        {
                            ShowError("驱动加载失败");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("加载驱动失败: {0}\n\n请确保以管理员身份运行此程序。", ex.Message));
            }
        }

        private void BtnUnloadDriver_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "确定要卸载驱动程序吗？",
                    "确认",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    _toolStripStatus.Text = "正在卸载驱动...";
                    Application.DoEvents();

                    _driverClient.Disconnect();
                    bool success = DriverService.UnloadDriver();

                    if (success)
                    {
                        ShowSuccess("驱动卸载成功！");
                        UpdateDriverStatus();
                    }
                    else
                    {
                        ShowError("驱动卸载失败");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("卸载驱动失败: {0}", ex.Message));
            }
        }

        private void BtnHideProcess_Click(object sender, EventArgs e)
        {
            if (_processGrid.SelectedRows.Count == 0)
            {
                ShowWarning("请先选择一个进程！");
                return;
            }

            DataGridViewRow selectedRow = _processGrid.SelectedRows[0];
            uint processId = Convert.ToUInt32(selectedRow.Cells["PID"].Value);
            string processName = selectedRow.Cells["ProcessName"].Value.ToString();

            DialogResult result = MessageBox.Show(
                string.Format("确定要隐藏进程 \"{0}\" (PID: {1}) 吗？\n\n" +
                "注意：隐藏后该进程将不会在任务管理器中显示！",
                processName, processId),
                "确认隐藏进程",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    _toolStripStatus.Text = string.Format("正在隐藏进程 {0}...", processName);
                    Application.DoEvents();

                    bool success = _driverClient.HideProcess(processId);

                    if (success)
                    {
                        ShowSuccess(string.Format("进程 \"{0}\" 已成功隐藏！", processName));
                        RefreshProcessList();
                    }
                    else
                    {
                        ShowError("隐藏进程失败");
                    }
                }
                catch (Exception ex)
                {
                    ShowError(string.Format("隐藏进程失败: {0}", ex.Message));
                }
            }
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
