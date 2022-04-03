using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddTrustedProcess.xaml
    /// </summary>
    public partial class AddTrustedProcess : Window
    {
        private TrustedProcessesView root;
        public AddTrustedProcess(TrustedProcessesView root)
        {
            this.root = root;
            InitializeComponent();
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
            this.IconReplace("greyshield.ico");
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(textBox.Text))
            {
                DbWorker.InsertTrustedProcess(textBox.Text);
                root.listView.Items.Add(new BindingsWorker { MainFilePath = textBox.Text });
                Close();
            }
            else
            {
                MessageBox.Show("Указаного файла не существует", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonChoose_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dialog.ShowDialog(); 
            if (result == true)
            {
                textBox.Text = dialog.FileName;
            }
        }
    }
}
