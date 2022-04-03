using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Antivirus
{
    /// <summary>
    /// Interaction logic for HashDetected.xaml
    /// </summary>
    public partial class HashDetected : Window
    {
        public MalwareActions MalwareAction { get; set; }
        private Process process;
        private FileMD5 fileMD5;

        public HashDetected(Process process, FileMD5 fileMD5, string text)
        {
            InitializeComponent();
            textBlock.Text = text;
            this.process = process;
            this.fileMD5 = fileMD5;
            MalwareAction = MalwareActions.Skip;
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("redshield.ico");
        }

        private void buttonSkip_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Skip;
            Close();
        }

        private void buttonAbort_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Abort;
            Close();
        }

        private void buttonTrust_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Trust;
            if(process!=null)
            {
                DbWorker.InsertTrustedProcess(process.MainModule.FileName);
            }
            else
            {
                DbWorker.InsertTrustedProcess(fileMD5.MainFilePath);
            }            
            Close();
        }

        private void buttonKill_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Kill;
            try
            {
                if (process != null)
                {
                    process.Kill();
                }
                else
                {
                    string processName = fileMD5.MainFilePath.Substring(fileMD5.MainFilePath.LastIndexOf('\\'));
                    Process[] processes = Process.GetProcessesByName(processName);
                    foreach(Process process in processes)
                    {
                        process.Kill();
                    }
                }
                File.Delete(fileMD5.MainFilePath);
                DbWorker.DeleteProcessMD5(fileMD5.MainFilePath);
            }
            catch
            {
                MessageBox.Show("Данный файл не может быть удален или связанные с ним процессы не могут быть остановлены", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Delete;
            DbWorker.UpdateProcessMD5(fileMD5);
            Close();
        }
    }
}
