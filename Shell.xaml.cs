using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MizoreTools
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class ShellWindow : Window
    {
        private string FileName;
        private string Arguments;
        private string Folder;
        private bool WillOpenFolder;
        public ShellWindow(string filename, string arguments, string open_folder)
        {
            this.FileName = filename;
            this.Arguments = arguments;
            if (open_folder == "")
                this.Folder = ".";
            else
                this.Folder = open_folder;
            this.WillOpenFolder = true;
            InitializeComponent();


        }
        public ShellWindow(string filename, string arguments)
        {
            this.FileName = filename;
            this.Arguments = arguments;
            this.WillOpenFolder = false;
            InitializeComponent();


        }
        public void RunShell(object sender, EventArgs e)
        {

            Console.WriteLine(String.Format("{0} {1}", this.FileName, this.Arguments));
            TxtShell.AppendText("\n");
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = this.FileName;
                p.StartInfo.Arguments = this.Arguments;
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                                                  //p.StandardInput.WriteLine("exit");
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += flushShellTxt;
                p.ErrorDataReceived += flushShellTxt;
                p.Exited += FinishProcess;
                p.Start();//启动程序
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                // p.StandardInput.AutoFlush = true;



                //TxtShell.Text = output;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new Exception("Error occurred when running command.");
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (this.WillOpenFolder)
            {
                System.Diagnostics.Process.Start("explorer.exe", Folder);
            }
        }

        private void flushShellTxt(object sender, DataReceivedEventArgs e)
        {
            Action action1 = () =>
            {
                TxtShell.AppendText(e.Data);
                TxtShell.AppendText("\n");
            };
            TxtShell.Dispatcher.BeginInvoke(action1);
        }

        private void FinishProcess(object sender, EventArgs e)
        {
            Action action1 = () =>
            {
                okButton.IsEnabled = true;
            };
            okButton.Dispatcher.BeginInvoke(action1);
            
        }

        private void TxtShell_TextChanged(object sender, EventArgs e)
        {
            TxtShell.SelectionStart = TxtShell.Text.Length; //Set the current caret position at the end
            TxtShell.ScrollToEnd(); //Now scroll it automatically
        }

    }
}
