using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Antivirus
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> signatures;
        private ManagementEventWatcher startWatch;
        private bool isRealTimeProtectionEnabled = false;
        private bool isInternetAviliable = false;
        private VirusDetected virusDetectedDialog;
        private HashDetected hashDetectedDialog;
        private Thread realTimeProtectionThread; 

        public MainWindow()
        {
            signatures = Signatures.getSignatures();            
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("yellowshield.ico");
        }

        private void RealTimeProtection()
        {
            foreach (Process p in Process.GetProcesses())
            {
                try
                {
                    CheckProcess(p);
                }
                catch { };
            }
            
            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();
        }

        private void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                Process p = Process.GetProcessById(Convert.ToInt32(e.NewEvent.Properties["ProcessId"].Value));
                CheckProcess(p);
            }
            catch { };
        }

        private void CheckProcess(Process process)
        {
            if (!DbWorker.IsProcessInTrustedProcesses(process.MainModule.FileName))
            {
                try
                {
                    string fileString = Encoding.UTF8.GetString(File.ReadAllBytes(process.MainModule.FileName));
                    foreach (KeyValuePair<string, string> kvp in signatures)
                    {
                        if (fileString.Contains(kvp.Key))
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                this.IconReplace("redshield.ico");
                                string text =
                                "Модуль " + process.MainModule.FileName + " процесса " + process.ProcessName + " содержит небезопасый код:\n" +
                                kvp.Key + ".\nТип угрозы: " + kvp.Value;
                                virusDetectedDialog = new VirusDetected(process, text);
                                virusDetectedDialog.ShowDialog();
                            });        
                            

                            
                            if (virusDetectedDialog.MalwareAction == MalwareActions.Abort)
                            {
                                try
                                {
                                    startWatch.Stop();
                                }
                                catch { };
                                isRealTimeProtectionEnabled = false;
                                Application.Current.Dispatcher.Invoke(delegate {
                                    buttonProtect.Content = "Включить защиту";
                                    this.IconReplace("yellowshield.ico");
                                });
                                realTimeProtectionThread.Abort();
                            }
                            else if (virusDetectedDialog.MalwareAction == MalwareActions.Delete
                                || virusDetectedDialog.MalwareAction == MalwareActions.Kill
                                || virusDetectedDialog.MalwareAction == MalwareActions.Trust)
                            {
                                Application.Current.Dispatcher.Invoke(delegate {
                                    this.IconReplace("greenshield.ico");
                                });
                                break;
                            }
                            else if (virusDetectedDialog.MalwareAction == MalwareActions.Skip)
                            {
                                Application.Current.Dispatcher.Invoke(delegate {
                                    this.IconReplace("greenshield.ico");
                                });
                            }
                        }
                    }

                    FileMD5 fileMD5 = new FileMD5(process.MainModule.FileName);
                    if (DbWorker.IsProcessInProcessesMD5(fileMD5.MainFilePath))
                    {
                        if (!DbWorker.CompareProcessesMD5(fileMD5.MainFilePath, fileMD5.Md5))
                        {
                            Application.Current.Dispatcher.Invoke(delegate {
                                this.IconReplace("redshield.ico");
                                FileMD5 oldMD5 = DbWorker.GetProcessMD5(fileMD5.MainFilePath);
                                string text =
                                    "Контрольная сумма основного модуля " + process.MainModule.FileName + " процесса " + process.ProcessName + " была изменена.\n" +
                                    "Предыдущая контрольная сумма " + oldMD5.Md5 + " была сформирована " + oldMD5.Date.ToString() + ".\n" +
                                    "Новое значение: " + fileMD5.Md5;
                                hashDetectedDialog = new HashDetected(process, fileMD5, text);
                                hashDetectedDialog.ShowDialog();
                            });  
                            

                            if (hashDetectedDialog.MalwareAction == MalwareActions.Abort)
                            {
                                try
                                {
                                    startWatch.Stop();
                                }
                                catch { };
                                isRealTimeProtectionEnabled = false;
                                Application.Current.Dispatcher.Invoke(delegate {
                                    buttonProtect.Content = "Включить защиту";
                                    this.IconReplace("yellowshield.ico");
                                });
                                realTimeProtectionThread.Abort();
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(delegate {
                                    this.IconReplace("greenshield.ico");
                                });
                                
                            }
                        }
                    }
                    else
                    {
                        DbWorker.InsertProcessMD5(fileMD5);
                    }
                }
                catch { };
            }
        }

        private void buttonProtect_Click(object sender, RoutedEventArgs e)
        {
            if (!isRealTimeProtectionEnabled)
            {
                isRealTimeProtectionEnabled = true;
                buttonProtect.Content = "Отключить защиту";
                this.IconReplace("greenshield.ico");
                realTimeProtectionThread = new Thread(new ThreadStart(RealTimeProtection));
                realTimeProtectionThread.Start();
            }
            else
            {
                try
                {
                    startWatch.Stop();
                }
                catch { };
                realTimeProtectionThread.Abort();
                isRealTimeProtectionEnabled = false;
                buttonProtect.Content = "Включить защиту";
                this.IconReplace("yellowshield.ico");
            }      
        }

        private void buttonScanFile_Click(object sender, RoutedEventArgs e)
        {
            ScanFile scanFile = new ScanFile();
            scanFile.Show();
        }

        private void buttonScanDirectory_Click(object sender, RoutedEventArgs e)
        {
            ScanProcess scanProcess = new ScanProcess();
            scanProcess.Show();
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            stackPanelMain.Visibility = Visibility.Hidden;
            stackPanelSettings.Visibility = Visibility.Visible;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            stackPanelSettings.Visibility = Visibility.Hidden;
            stackPanelMain.Visibility = Visibility.Visible;
        }

        private void buttonInternetAviliable_Click(object sender, RoutedEventArgs e)
        {
            if (!isInternetAviliable)
            {
                try
                {
                    signatures = Signatures.getOnlineSignatures();
                    buttonInternetAviliable.Content = "Запретить доступ к внешней БД";
                    isInternetAviliable = true;
                }
                catch
                {
                    MessageBox.Show("Не удалось подключиться к внешней базе вирусных сигнатур. Проверте интернет-соединение", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                buttonInternetAviliable.Content = "Разрешить доступ к внешней БД";
                isInternetAviliable = false;
            }
            
        }

        private void buttonShowTrustedProcesses_Click(object sender, RoutedEventArgs e)
        {
            TrustedProcessesView trustedProcessesView = new TrustedProcessesView();
            trustedProcessesView.Show();
        }

        private void buttonMD5List_Click(object sender, RoutedEventArgs e)
        {
            ProcessesMD5View processesMD5View = new ProcessesMD5View();
            processesMD5View.Show();
        }
    }
}
