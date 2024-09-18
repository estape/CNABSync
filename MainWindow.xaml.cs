using CNAB_Sync.View;
using Leitor_CNAB.Controller;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leitor_CNAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Btn_OpenCNAB_Click(object sender, RoutedEventArgs e)
        {
            var dialogFile = new Microsoft.Win32.OpenFileDialog();
            dialogFile.FileName = "";
            dialogFile.DefaultExt = ".CNAB";
            dialogFile.Filter = "(.cnab)|*.cnab";
            bool? results = dialogFile.ShowDialog();

            if (results == true)
            {
                #pragma warning disable IDE0059 // Unnecessary assignment of a value
                string filename = dialogFile.FileName;
                #pragma warning restore IDE0059 // Unnecessary assignment of a value
                FileIO.OpenFileCNAB(filename);
            }
        }

        private void Btn_SaveCNAB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ImportREM_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ImportTXT_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ExportREM_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ExportTXT_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ExportXLSX_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ExportCSV_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new Window
            {
                Title = "Sobre",
                Content = new AboutPage(),
                Width = 600,
                Height = 450,
                SizeToContent = SizeToContent.Manual,
                ResizeMode = ResizeMode.NoResize
            };
            aboutWindow.Show();
        }
    }
}