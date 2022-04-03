using System;
using System.Collections.Generic;
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
    /// Interaction logic for ProcessesMD5View.xaml
    /// </summary>
    public partial class ProcessesMD5View : Window
    {
        public ProcessesMD5View()
        {
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("greyshield.ico");
            this.listView.Items.Clear();
            List<FileMD5> processesMD5 = DbWorker.GetProcessesMD5();
            foreach (FileMD5 processMD5 in processesMD5)
            {
                this.listView.Items.Add(processMD5);
            }
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteItem();
            }
        }

        private void contextItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }

        private void contextItemClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Удалить все элементы?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DbWorker.ClearProcessesMD5();
                    listView.Items.Clear();
                }
            }
            catch { };
        }

        private void contextItemAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddMD5(this).ShowDialog();
        }


        private void DeleteItem()
        {
            try
            {
                if (MessageBox.Show("Удалить контрольную сумму для выбраного файла?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    FileMD5 fileMD5 = listView.SelectedItem as FileMD5;
                    DbWorker.DeleteProcessMD5(fileMD5.MainFilePath);
                    listView.Items.Remove(fileMD5);
                }
            }
            catch { };
        }
    }
}
