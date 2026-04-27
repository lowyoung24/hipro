using System;
using System.Windows.Forms;

namespace hipro_Client
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // 检查管理员权限
            if (!IsAdministrator())
            {
                DialogResult result = MessageBox.Show(
                    "此程序需要管理员权限才能运行！\n\n是否以管理员身份重新启动？",
                    "需要管理员权限",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    RestartAsAdministrator();
                }
                else
                {
                    Application.Exit();
                }
                return;
            }

            Application.Run(new MainForm());
        }

        /// <summary>
        /// 检查当前是否以管理员身份运行
        /// </summary>
        static bool IsAdministrator()
        {
            try
            {
                System.Security.Principal.WindowsIdentity identity = 
                    System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = 
                    new System.Security.Principal.WindowsPrincipal(identity);
                
                return principal.IsInRole(
                    System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 以管理员身份重新启动程序
        /// </summary>
        static void RestartAsAdministrator()
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo = 
                    new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas"; // 以管理员身份运行

                System.Diagnostics.Process.Start(startInfo);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("无法以管理员身份启动: {0}", ex.Message),
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Application.Exit();
            }
        }
    }
}
