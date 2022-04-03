using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ScanProcess.xaml
    /// </summary>
    /// //378.167
    /// //220
    public partial class ScanProcess : Window
    {
        private Dictionary<string, string> signatures;
        private List<string> files;
        Thread scanThread;
        private FileMalware fileMalwareDialog;
        private HashDetected hashDetectedDialog;
        public ScanProcess()
        {
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("greyshield.ico");
            this.Height = 220;
        }

        private void buttonChoose_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                textBoxPath.Text = dialog.SelectedPath;
            }
        }

        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                files = new List<string>(Directory.GetFiles(textBoxPath.Text, "*.*", SearchOption.AllDirectories)
                    .Where(s =>
                    s.EndsWith(".exe") ||
                    s.EndsWith(".ini") ||
                    s.EndsWith(".bat") ||
                    s.EndsWith(".dll") ||
                    s.EndsWith(".mp3") ||
                    s.EndsWith(".jpg")));
                //files = Directory.GetFiles(textBoxPath.Text, "*.*", SearchOption.AllDirectories);
                if (!files.Any())
                {
                    MessageBox.Show("Выбраная директория не содержит файлов", "Пусто!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    panelChoose.Visibility = Visibility.Hidden;
                    panelScan.Visibility = Visibility.Visible;
                    this.Height = 378;
                    scanThread = new Thread(new ThreadStart(StartScan));
                    scanThread.Start();
                }
            }
            catch
            {
                MessageBox.Show("Неправильный маршрут", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void StartScan()
        {
            Application.Current.Dispatcher.Invoke(delegate {
                textBlockLog.Text += "Начато сканирование директории: " + textBoxPath.Text;
                if (checkBoxAllowSignatures.IsChecked.GetValueOrDefault())
                {
                    signatures = Signatures.getOnlineSignatures();
                }
                else
                {
                    signatures = Signatures.getSignatures();
                }
                progressBar.Maximum = files.Count();
                progressBar.Minimum = 0;
            });
            
            foreach (string file in files)
            {
                if (!DbWorker.IsProcessInTrustedProcesses(file))
                {
                    try
                    {
                        if (file.EndsWith(".exe"))
                        {
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
                                        textBlockLog.Text += "\n" + text;
                                    });                 
                                    
                                    fileMalwareDialog = new FileMalware(file, text);
                                    fileMalwareDialog.ShowDialog();

                                    Application.Current.Dispatcher.Invoke(delegate {
                                        this.IconReplace("greenshield.ico");
                                        if (fileMalwareDialog.MalwareAction == MalwareActions.Trust)
                                        {
                                            textBlockLog.Text += "\nФайл добавлен в исключения.";
                                        }
                                        else if (fileMalwareDialog.MalwareAction == MalwareActions.Abort)
                                        {
                                            textBlockLog.Text += "\nСканирование прервано.";
                                            scanThread.Abort();
                                        }
                                        else if (fileMalwareDialog.MalwareAction == MalwareActions.Skip)
                                        {
                                            textBlockLog.Text += "\nФайл пропущен, сканирование прододжено.";
                                        }
                                        else if (fileMalwareDialog.MalwareAction == MalwareActions.Delete)
                                        {
                                            textBlockLog.Text += "\nФайл удален.";
                                        }
                                        else if (fileMalwareDialog.MalwareAction == MalwareActions.Kill)
                                        {
                                            textBlockLog.Text += "\nФайл удален, связаные с ним процессы завершены.";
                                        }
                                    });
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(delegate {
                                        this.IconReplace("greenshield.ico");
                                        textBlockLog.Text += "\nФайл " + file + " не содержит небезопасных функций и сигнатур .";
                                    });
                                }
                            }
                        }

                        bool isHash = false;
                        Application.Current.Dispatcher.Invoke(delegate {
                            isHash = checkBoxAllowHash.IsChecked.GetValueOrDefault();
                        });
                        if(isHash)
                        {
                            FileMD5 fileMD5 = new FileMD5(file);
                            if (DbWorker.IsProcessInProcessesMD5(fileMD5.MainFilePath))
                            {
                                if (!DbWorker.CompareProcessesMD5(fileMD5.MainFilePath, fileMD5.Md5))
                                {
                                    FileMD5 oldMD5 = DbWorker.GetProcessMD5(fileMD5.MainFilePath);
                                    string text =
                                        "Контрольная сумма файла " + file + " была изменена.\n" +
                                        "Предыдущая контрольная сумма " + oldMD5.Md5 + " была сформирована " + oldMD5.Date.ToString() + ".\n" +
                                        "Новое значение: " + fileMD5.Md5;
                                    Application.Current.Dispatcher.Invoke(delegate {
                                        this.IconReplace("redshield.ico");
                                        textBlockLog.Text += "\n" + text;
                                        hashDetectedDialog = new HashDetected(null, fileMD5, text);
                                        hashDetectedDialog.ShowDialog();
                                    });

                                    Application.Current.Dispatcher.Invoke(delegate {
                                        this.IconReplace("greenshield.ico");
                                        if (hashDetectedDialog.MalwareAction == MalwareActions.Abort)
                                        {
                                            textBlockLog.Text += "\nСканирование прервано.";
                                            scanThread.Abort();
                                        }
                                        else if (hashDetectedDialog.MalwareAction == MalwareActions.Skip)
                                        {
                                            textBlockLog.Text += "\nФайл пропущен, сканирование прододжено.";
                                        }
                                        else if (hashDetectedDialog.MalwareAction == MalwareActions.Delete)
                                        {
                                            textBlockLog.Text += "\nКонтрольна сумма файла обновлена.";
                                        }
                                        else if (hashDetectedDialog.MalwareAction == MalwareActions.Kill)
                                        {
                                            textBlockLog.Text += "\nФайл удален, связаные с ним процессы завершены.";
                                        }
                                        else if (hashDetectedDialog.MalwareAction == MalwareActions.Trust)
                                        {
                                            textBlockLog.Text += "\nФайл добавлен в исключения.";
                                        }
                                    });
                                }
                            }
                            else
                            {
                                DbWorker.InsertProcessMD5(fileMD5);
                            }
                        }
                    }
                    catch { };
                }
                Application.Current.Dispatcher.Invoke(delegate {
                    progressBar.Value++;
                });
            }
            Application.Current.Dispatcher.Invoke(delegate {
                textBlockLog.Text += "\nСканирование завершено";
                buttonFinish.Content = "Закончить";
            });
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                scanThread.Abort();
            }
            catch { };
            Close();
        }
    }
}
