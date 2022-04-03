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
    /// Interaction logic for VirusDetected.xaml
    /// </summary>
    public partial class VirusDetected : Window
    {
        public MalwareActions MalwareAction { get; set; }
        private Process process;
        
        public VirusDetected(Process process, string text)
        {
            InitializeComponent();
            textBlock.Text = text;
            this.process = process;
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

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Delete;
            try
            {
                process.Kill();
                File.Delete(process.MainModule.FileName);
            }
            catch
            {
                MessageBox.Show("Данный процес не может быть остановлен либо его файл не может быть удален", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void buttonKill_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Kill;
            try
            {
                process.Kill();
            }
            catch
            {
                MessageBox.Show("Данный процес не может быть остановлен", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Close();
        }

        private void buttonTrust_Click(object sender, RoutedEventArgs e)
        {
            MalwareAction = MalwareActions.Trust;
            DbWorker.InsertTrustedProcess(process.MainModule.FileName);
            Close();
        }
    }
}
