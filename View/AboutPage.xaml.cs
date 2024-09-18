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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CNAB_Sync.View
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();

            string version = "1.0.0"; // Defina a versão atual do seu software
            string developerName = "Rodrigo Estape";
            string description = "CNAB Sync é uma ferramenta avançada para leitura e gravação de arquivos CNAB, oferecendo suporte para exportação em formatos XLSX e CSV, além de garantir a segurança dos dados com arquivos proprietários.";
            string copyrightInfo = "© 2024 Rodrigo Estape. Todos os direitos reservados.";

            Txt_About.Text = "CNAB Sync" + Environment.NewLine + Environment.NewLine +
                             "Versão: " + version + Environment.NewLine +
                             "Desenvolvedor: " + developerName + Environment.NewLine + Environment.NewLine +
                             description + Environment.NewLine + Environment.NewLine +
                             copyrightInfo;
        }

        private void Btn_closeAboutPage_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}
