using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for ScanFile.xaml
    /// </summary>
    public partial class ScanFile : Window
    {
        private string file;
        private Dictionary<string, string> signatures;
        private Thread scanFileThread;
        private bool isScanFinished = false;
        private FileMalware fileMalwareDialog;
        public ScanFile()
        {
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("greyshield.ico");
        }

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            if(!isScanFinished)
            {
                if (File.Exists(textBoxPath.Text))
                {
                    scanFileThread = new Thread(new ThreadStart(StartScan));
                    scanFileThread.Start();
                    buttonScan.Content = "Прервать сканирование";
                }
                else
                {
                    MessageBox.Show("Указаного файла не существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                scanFileThread.Abort();
                Close();
            }
            
        }

        public void StartScan()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                file = textBoxPath.Text;
                if (checkBoxAllowSignatures.IsChecked.GetValueOrDefault())
                {
                    signatures = Signatures.getOnlineSignatures();
                }
                else
                {
                    signatures = Signatures.getSignatures();
                }
            });

            if (!DbWorker.IsProcessInTrustedProcesses(file))
            {
                try
                {
                    bool isDetected = false;
                    string fileString = Encoding.UTF8.GetString(File.ReadAllBytes(file));
                    foreach (KeyValuePair<string, string> kvp in signatures)
                    {
                        if (fileString.Contains(kvp.Key))
                        {
                            string text =
                                "Файл " + file + " содержит небезопасый код:\n" +
                                kvp.Key + ".\nТип угрозы: " + kvp.Value;
                            Application.Current.Dispatcher.Invoke(delegate {
                                this.IconReplace("redshield.ico");
                                isDetected = true;
                                fileMalwareDialog = new FileMalware(file, text);
                                fileMalwareDialog.ShowDialog();
                            });
                           

                            Application.Current.Dispatcher.Invoke(delegate {
                                this.IconReplace("greenshield.ico");
                                if (fileMalwareDialog.MalwareAction == MalwareActions.Abort)
                                {
                                    isScanFinished = true;
                                    scanFileThread.Abort();
                                }
                            });
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                this.IconReplace("greenshield.ico");
                            });
                        }
                    }

                    if(!isDetected)
                    {
                        MessageBox.Show("Файл не содержит опасных функций и вирусных сигнатур", "Чисто!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }


                    isDetected = false;
                    bool isHash = false;
                    Application.Current.Dispatcher.Invoke(delegate {
                        isHash = checkBoxAllowHash.IsChecked.GetValueOrDefault();
                    });
                    if (isHash)
                    {
                        FileMD5 fileMD5 = new FileMD5(file);
                        if (DbWorker.IsProcessInProcessesMD5(fileMD5.MainFilePath))
                        {
                            if (!DbWorker.CompareProcessesMD5(fileMD5.MainFilePath, fileMD5.Md5))
                            {
                                isDetected = true;
                                FileMD5 oldMD5 = DbWorker.GetProcessMD5(fileMD5.MainFilePath);
                                string text =
                                    "Контрольная сумма файла " + file + " была изменена.\n" +
                                    "Предыдущая контрольная сумма " + oldMD5.Md5 + " была сформирована " + oldMD5.Date.ToString() + ".\n" +
                                    "Новое значение: " + fileMD5.Md5;
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    this.IconReplace("redshield.ico");
                                });
                                HashDetected hashDetectedDialog = new HashDetected(null, fileMD5, text);
                                hashDetectedDialog.ShowDialog();

                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    this.IconReplace("greenshield.ico");
                                    if (hashDetectedDialog.MalwareAction == MalwareActions.Abort)
                                    {
                                        isScanFinished = true;
                                        scanFileThread.Abort();
                                    }
                                });
                            }
                        }
                        else
                        {
                            DbWorker.InsertProcessMD5(fileMD5);
                            MessageBox.Show("Контрольная сумма файла добавлена в базу", "Чисто!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    MessageBox.Show("Сканирование завершено.", "Сканирование завершено!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        buttonScan.Content = "Закрыть";
                    });
                    isScanFinished = true;
                }
                catch { };
            }
        }

        private void buttonChoose_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                textBoxPath.Text = dialog.FileName;
            }
        }
    }
}
