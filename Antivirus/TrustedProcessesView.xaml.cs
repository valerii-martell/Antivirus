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
    /// Interaction logic for TrustedProcessesView.xaml
    /// </summary>
    public partial class TrustedProcessesView : Window
    {
        
        public TrustedProcessesView()
        {
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("greyshield.ico");
            this.listView.Items.Clear();
            List<string> trustedProcesses = DbWorker.GetTrustedProcesses();
            foreach (string trustedProcess in trustedProcesses)
            {
                this.listView.Items.Add(new BindingsWorker { MainFilePath = trustedProcess });
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
                    DbWorker.ClearTrustedProcesses();
                    listView.Items.Clear();
                }
            }
            catch { };
        }

        private void contextItemAdd_Click(object sender, RoutedEventArgs e)
        {
            new AddTrustedProcess(this).ShowDialog();
        }


        private void DeleteItem()
        {
            try
            {
                if (MessageBox.Show("Удалить выбраный процесс из списка исключений?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    BindingsWorker bw = listView.SelectedItem as BindingsWorker;
                    DbWorker.DeleteTrustedProcess(bw.MainFilePath);
                    listView.Items.Remove(bw);
                }
            }
            catch { };
        }
    }
}
