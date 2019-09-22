using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MizoreTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        public string AboutString { get; set; }

        /*
         * 
         * 公用函数
         * 
         * */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadConfig();
            SetSelectionAllOnGotFocus(TxtInAddr);


            LblAbout.Content = "Mizore工具箱是一款整合各种命令行工具并提供友好GUI的快捷工具。\n" +
                "初衷是解决shell下无法快捷编辑命令的问题。\n" +
                "软件在测试阶段，出现问题随时联系 Contact@XSky123.com\n\n" +
                "By Mizore@Voice Memories";



        }


        private void CheckAndRun(string program, string arguments, bool show_folder=true)
        {
            if (Shell.ExistsOnPath(program))
            {
                if (show_folder)
                {
                    ShellWindow newWindow = new ShellWindow(program, arguments, TxtOutAddr.Text);
                    newWindow.Show();
                }
                else
                {
                    ShellWindow newWindow = new ShellWindow(program, arguments);
                    newWindow.Show();
                }
                
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(String.Format("请检查{0}是否存在!", program),
                                                        "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReadConfig()
        {
            /* Youtube-DL */

            ChkProxy.IsChecked = bool.Parse(ConfigurationManager.AppSettings["U2B_EnableProxy"].ToString());
            CmbProxy.SelectedItem = ConfigurationManager.AppSettings["U2B_ProxyType"].ToString();

            TxtInAddr.Text = ConfigurationManager.AppSettings["U2B_InAddr"].ToString();
            TxtOutAddr.Text = ConfigurationManager.AppSettings["U2B_OutAddr"].ToString();

            ChkAllSub.IsChecked = bool.Parse(ConfigurationManager.AppSettings["U2B_ChkAllSub"].ToString());
            ChkAllAutoSub.IsChecked = bool.Parse(ConfigurationManager.AppSettings["U2B_ChkAllAutoSub"].ToString());
            ChkSpecifySub.IsChecked = bool.Parse(ConfigurationManager.AppSettings["U2B_ChkSpecifySub"].ToString());
            TxtSpecifySub.Text = ConfigurationManager.AppSettings["U2B_SpecifySub"].ToString();

            ChkFormat.IsChecked = bool.Parse(ConfigurationManager.AppSettings["U2B_ChkFormat"].ToString());
            TxtFormat.Text = ConfigurationManager.AppSettings["U2B_TxtFormat"].ToString();

        }

        private void SaveConfig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!this.IsLoaded)
                return;
            /* Youtube-DL */
            config.AppSettings.Settings["U2B_InAddr"].Value = TxtInAddr.Text;
            config.AppSettings.Settings["U2B_OutAddr"].Value = TxtOutAddr.Text;

            config.AppSettings.Settings["U2B_EnableProxy"].Value = ChkProxy.IsChecked.ToString();
            config.AppSettings.Settings["U2B_ProxyType"].Value = CmbProxy.SelectedItem.ToString();
            config.AppSettings.Settings["U2B_ProxyAddr"].Value = TxtProxyAddr.Text;
            config.AppSettings.Settings["U2B_ProxyPort"].Value = TxtProxyPort.Text;

            config.AppSettings.Settings["U2B_ChkAllSub"].Value = ChkAllSub.IsChecked.ToString();
            config.AppSettings.Settings["U2B_ChkAllAutoSub"].Value = ChkAllAutoSub.IsChecked.ToString();
            config.AppSettings.Settings["U2B_ChkSpecifySub"].Value = ChkSpecifySub.IsChecked.ToString();
            config.AppSettings.Settings["U2B_SpecifySub"].Value = TxtSpecifySub.Text;

            config.AppSettings.Settings["U2B_ChkFormat"].Value = ChkFormat.IsChecked.ToString();
            config.AppSettings.Settings["U2B_TxtFormat"].Value = TxtFormat.Text;

            /* Save */
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");

        }

        private void SaveConfig(object sender, EventArgs e)
        {
            SaveConfig();
        }
        private void SaveConfig(object sender, DependencyPropertyChangedEventArgs e)
        {
            SaveConfig();
        }

        public void SetSelectionAllOnGotFocus(System.Windows.Controls.TextBox textbox)
        {

            MouseButtonEventHandler _OnPreviewMouseDown = (sender, e) =>
            {
                System.Windows.Controls.TextBox box = e.Source as System.Windows.Controls.TextBox;
                box.Focus();
                e.Handled = true;
            };
            RoutedEventHandler _OnLostFocus = (sender, e) =>
            {
                System.Windows.Controls.TextBox box = e.Source as System.Windows.Controls.TextBox;
                box.PreviewMouseDown += _OnPreviewMouseDown;
            };
            RoutedEventHandler _OnGotFocus = (sender, e) =>
            {
                System.Windows.Controls.TextBox box = e.Source as System.Windows.Controls.TextBox;
                box.SelectAll();
                box.PreviewMouseDown -= _OnPreviewMouseDown;
            };

            textbox.PreviewMouseDown += _OnPreviewMouseDown;
            textbox.LostFocus += _OnLostFocus;
            textbox.GotFocus += _OnGotFocus;
        }
        /*
         * Youtube-DL 控件部分
         */
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();
            TxtOutAddr.Text = m_Dir;
            TxtOutAddr.CaretIndex = TxtOutAddr.Text.Length;
            var rect = TxtOutAddr.GetRectFromCharacterIndex(TxtOutAddr.CaretIndex);
            TxtOutAddr.ScrollToHorizontalOffset(rect.Right);
            SaveConfig();
        }

        private void ChkProxy_Checked(object sender, RoutedEventArgs e)
        {
            if (this.CmbProxy == null || this.CmbProxy.IsEnabled)
                return;
            this.CmbProxy.IsEnabled = true;
            this.TxtProxyAddr.IsEnabled = true;
            this.TxtProxyPort.IsEnabled = true;
            SaveConfig();
        }
        private void ChkProxy_UnChecked(object sender, RoutedEventArgs e)
        {
            if (this.CmbProxy == null || !this.CmbProxy.IsEnabled)
                return;
            this.CmbProxy.IsEnabled = false;
            this.TxtProxyAddr.IsEnabled = false;
            this.TxtProxyPort.IsEnabled = false;
            SaveConfig();
        }
        private void ChkSpecifySub_Checked(object sender, RoutedEventArgs e)
        {
            if (this.TxtSpecifySub == null || this.TxtSpecifySub.IsEnabled)
                return;
            this.TxtSpecifySub.IsEnabled = true;
            SaveConfig();

        }
        private void ChkSpecifySub_UnChecked(object sender, RoutedEventArgs e)
        {
            if (this.TxtSpecifySub == null || !this.TxtSpecifySub.IsEnabled)
                return;
            this.TxtSpecifySub.IsEnabled = false;
            SaveConfig();
        }
        private void ChkFormat_Checked(object sender, RoutedEventArgs e)
        {
            if (this.TxtFormat == null || this.TxtFormat.IsEnabled)
                return;
            this.TxtFormat.IsEnabled = true;
            SaveConfig();

        }
        private void ChkFormat_UnChecked(object sender, RoutedEventArgs e)
        {
            if (this.TxtFormat == null || !this.TxtFormat.IsEnabled)
                return;
            this.TxtFormat.IsEnabled = false;
            SaveConfig();
        }

        private void BtnCheckSubList_Click(object sender, RoutedEventArgs e)
        {

            CheckU2bAndRun("youtube-dl.exe", String.Format("\"{0}\" --list-subs{1} --no-check-certificate", TxtInAddr.Text, proxy_builder()),
                false);
            SaveConfig();
        }

        private void BtnCheckAllFormat_Click(object sender, RoutedEventArgs e)
        {
            CheckU2bAndRun("youtube-dl.exe", String.Format("\"{0}\" -F{1} --no-check-certificate", TxtInAddr.Text, proxy_builder()),
                false);
            SaveConfig();
        }
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            CheckU2bAndRun("youtube-dl.exe", build_arguments());
            SaveConfig();
        }

        /*
         * ffmpeg 控件部分
         */
        private void BtnFFMpegBrowseIn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TxtFFMpegIn.Text = openFileDialog.FileName;
                TxtFFMpegOut.Text = String.Format("{0}_Mizore{1}",
                    System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName),
                    System.IO.Path.GetExtension(openFileDialog.FileName));
            }
        }

        private void BtnGetWAV_Click(object sender, RoutedEventArgs e)
        {
            if(TxtFFMpegIn.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请选择输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Console.WriteLine(wav_builder());
            CheckAndRun("ffmpeg", wav_builder(), false);
        }

        private void BtnGet320KMP4_Click(object sender, RoutedEventArgs e)
        {
            if (TxtFFMpegIn.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请选择输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Console.WriteLine(_320KMP4_builder());
            CheckAndRun("ffmpeg", _320KMP4_builder(), false);
        }

        private void BtnGetCopyMP4_Click(object sender, RoutedEventArgs e)
        {
            if (TxtFFMpegIn.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请选择输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool need_cut = (TimeStrToSecond(TxtCutEndPos.Text) == TimeStrToSecond(TxtCutStartPos.Text));
            //Console.WriteLine(copyMP4_builder(need_cut));
            CheckAndRun("ffmpeg", copyMP4_builder(need_cut), false);
        }

        private void BtnGetCut_Click(object sender, RoutedEventArgs e)
        {
            if (TxtFFMpegIn.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请选择输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(TimeStrToSecond(TxtCutEndPos.Text) == TimeStrToSecond(TxtCutStartPos.Text))
            {
                System.Windows.Forms.MessageBox.Show("请输入剪辑时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Console.WriteLine(cut_builder());
            CheckAndRun("ffmpeg", cut_builder(), false);

        }

        private void BtnStartAdd1H(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "01:00:00";
            else
            {
                timeGroup[0]++;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }

        private void BtnStartAdd1M(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "00:01:00";
            else
            {
                timeGroup[1] = (timeGroup[1] + 1) % 60;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }

        private void BtnStartAdd1S(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "00:00:01";
            else
            {
                timeGroup[2] = (timeGroup[2] + 1) % 60;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }

        private void BtnStartSub1H(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "00:00:00";
            else
            {
                if (timeGroup[0] - 1 < 0)
                    timeGroup[0] = 48;
                else
                    timeGroup[0]--;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }

        private void BtnStartSub1M(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "00:00:00";
            else
            {
                if (timeGroup[1] - 1 < 0)
                    timeGroup[1] = 59;
                else
                    timeGroup[1]--;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }

        private void BtnStartSub1S(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutStartPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutStartPos.Text = "00:00:00";
            else
            {
                if (timeGroup[2] - 1 < 0)
                    timeGroup[2] = 59;
                else
                    timeGroup[2]--;
                TxtCutStartPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeStart();
        }
        private void BtnEndAdd1H(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "01:00:00";
            else
            {
                timeGroup[0]++;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }

        private void BtnEndAdd1M(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "00:01:00";
            else
            {
                timeGroup[1] = (timeGroup[1] + 1) % 60;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }

        private void BtnEndAdd1S(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "00:00:01";
            else
            {
                timeGroup[2] = (timeGroup[2] + 1) % 60;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }

        private void BtnEndSub1H(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "00:00:00";
            else
            {
                if (timeGroup[0] - 1 < 0)
                    timeGroup[0] = 48;
                else
                    timeGroup[0]--;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }

        private void BtnEndSub1M(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "00:00:00";
            else
            {
                if (timeGroup[1] - 1 < 0)
                    timeGroup[1] = 59;
                else
                    timeGroup[1]--;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }
        private void BtnEndSub1S(object sender, RoutedEventArgs e)
        {
            int[] timeGroup = ParseTime(TxtCutEndPos.Text);
            if (timeGroup[0] == -1) // error occurred
                TxtCutEndPos.Text = "00:00:00";
            else
            {
                if (timeGroup[2] - 1 < 0)
                    timeGroup[2] = 59;
                else
                    timeGroup[2]--;
                TxtCutEndPos.Text = TimeGroupToStr(timeGroup);
            }
            TimeGeneralWhenChangeEnd();
        }


        /*
         * 
         * Youtube-DL 参数控制
         * 
         */
        private string build_arguments()
        {
            
            string result = String.Format("\"{0}\"{1}{2}{3}{4} --no-check-certificate", TxtInAddr.Text, proxy_builder(), subtitle_builder(), 
                format_builder(), output_builder());

            return result;
        }
        private string output_builder()
        {
            if (TxtOutAddr.Text != "")
            {
                return String.Format(" -o \"{0}{1}%(title)s.%(ext)s\"", TxtOutAddr.Text, System.IO.Path.DirectorySeparatorChar);
            }
            return "";
        }
        private string proxy_builder()
        {
           
            if (ChkProxy.IsChecked == true)
            {
                string protocol_head;
                switch (CmbProxy.SelectedItem)
                {
                    case "socks5":
                        protocol_head = "socks5";
                        break;
                    case "HTTPS":
                        protocol_head = "https";
                        break;
                    case "HTTP":
                        protocol_head = "http";
                        break;
                    default:
                        protocol_head = "socks5";
                        break;
                }
                return String.Format(" --proxy {0}://{1}:{2}/", protocol_head, TxtProxyAddr.Text, TxtProxyPort.Text);
            }
            return "";
        }
        private string subtitle_builder()
        {
            string cmd = "";
            bool add_sub = false;
            if (ChkAllSub.IsChecked == true)
            {
                cmd += " --all-subs";
                add_sub = true;
            }
            if (ChkAllAutoSub.IsChecked == true) { 
                cmd += " --write-auto-sub";
                add_sub = true;
            }
            if (ChkSpecifySub.IsChecked == true && TxtSpecifySub.Text != "")
            {
                cmd += String.Format(" --sub-lang {0}", TxtSpecifySub.Text);
                add_sub = true;
            }
            if (add_sub)
                cmd += " --convert-subs ass";
            return cmd;
        }
        private string format_builder()
        {
            string cmd = "";
            if (ChkFormat.IsChecked == true && TxtFormat.Text != "")
                cmd += String.Format(" --format {0}", TxtFormat.Text);
            return cmd;
        }

        private void CheckU2bAndRun(string program, string arguments, bool show_folder=true)
        {
            if (TxtInAddr.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("请输入源视频地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CheckAndRun(program, arguments, show_folder);
            }
        }


        /*
         * 
         * ffmpeg 参数控制
         * 
         */
        private string wav_builder()
        {
            string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TxtFFMpegIn.Text),
                System.IO.Path.GetFileNameWithoutExtension(TxtFFMpegOut.Text) + ".wav");
            return String.Format(" -i \"{0}\" \"{1}\"", TxtFFMpegIn.Text, output);
        }

        private string _320KMP4_builder()
        {
            string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TxtFFMpegIn.Text),
                System.IO.Path.GetFileNameWithoutExtension(TxtFFMpegOut.Text) + ".mp4");
            return String.Format(" -i \"{0}\" -vcodec copy -acodec aac -b:a 320k \"{1}\"", TxtFFMpegIn.Text, output);
        }


        private string copyMP4_builder(bool need_cut)
        {
            string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TxtFFMpegIn.Text),
                System.IO.Path.GetFileNameWithoutExtension(TxtFFMpegOut.Text) + ".mp4");
            if (need_cut)
            {
                return String.Format(" -i \"{0}\" -ss {1} -t {2} -c copy \"{3}\"", TxtFFMpegIn.Text, TxtCutStartPos.Text, 
                                                                                   getDiffTimeStr() ,output);
            }
            else {
                return String.Format(" -i \"{0}\" -c copy \"{1}\"", TxtFFMpegIn.Text, output);
            }
        }
        private string cut_builder()
        {
            string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(TxtFFMpegIn.Text),
                TxtFFMpegOut.Text);

            return String.Format(" -i \"{0}\" -ss {1} -t {2} -c copy \"{3}\"", TxtFFMpegIn.Text, TxtCutStartPos.Text,
                                                                               getDiffTimeStr(), output);
        }
        /*
         * 
         * ffmpeg 剪切时间处理
         * 
         */
        int TimeStrToSecond(string TimeStr)
        {
            string[] timeGroup = TimeStr.Split(':');
            if (timeGroup.Length != 3)
            {
                return -1;

            }
            else
                return int.Parse(timeGroup[0]) * 3600 + int.Parse(timeGroup[1]) * 60 + int.Parse(timeGroup[2]);
        }

        int[] ParseTime(string TimeStr)
        {
            string[] timeGroup = TimeStr.Split(':');
            if (timeGroup.Length != 3)
            {
                return new int[] { -1, -1, -1};

            }
            else
                return new int[] { int.Parse(timeGroup[0]), int.Parse(timeGroup[1]), int.Parse(timeGroup[2]) };
        }
        private string TimeGroupToStr(int[] TimeGroup)
        {
            return String.Format("{0:D2}:{1:D2}:{2:D2}", TimeGroup[0], TimeGroup[1], TimeGroup[2]);
        }
       
        private void TimeGeneralWhenChangeStart()
        {
            if (TimeStrToSecond(TxtCutStartPos.Text) > TimeStrToSecond(TxtCutEndPos.Text))
                TxtCutEndPos.Text = TxtCutStartPos.Text;
        }
        private void TimeGeneralWhenChangeEnd()
        {
            if (TimeStrToSecond(TxtCutEndPos.Text) < TimeStrToSecond(TxtCutStartPos.Text))
                TxtCutStartPos.Text = TxtCutEndPos.Text;
        }
       private string getDiffTimeStr()
        {
            int time_diff = TimeStrToSecond(TxtCutEndPos.Text) - TimeStrToSecond(TxtCutStartPos.Text);
            int hour_diff = time_diff / 3600;
            int min_diff = (time_diff - hour_diff * 3600) / 60;
            int sec_diff = time_diff - hour_diff * 3600 - min_diff * 60;
            return String.Format("{0:D2}:{1:D2}:{2:D2}", hour_diff, min_diff, sec_diff);
        }
    }
}
