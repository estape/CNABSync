using CNAB_Sync.Model;
using CNAB_Sync.View;
using Microsoft.Win32;
using System.Windows;
using CNAB_Sync.Controller;

namespace Leitor_CNAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Detalhe444> clientes = new List<Detalhe444>(); // OBSOLETO

        List<Detalhe444> clientListFinal = [];
        string totalParcelasCNAB = "";

        private void ClearDataFront()
        {
            // Limpar os campos da interface
            ClientesList.ItemsSource = null;
            Txt_DataEmission.Text = string.Empty;
            Txt_ClientTotal.Text = "R$ 0,00";
            Txt_CNABTotal.Text = "R$ 0,00";
            ParcelasContratoList.ItemsSource = null;
        }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Btn_OpenCNAB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos CNAB|*.REM|Arquivos CNAB|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                // Limpar todos os dados anteriores do front antes de importar novos
                ClearDataFront();

                FileManager refFileNamager = new();
                (clientListFinal, totalParcelasCNAB) = refFileNamager.ProcessingFile(openFileDialog.FileName);

                // Atualizar a interface com os dados retornados
                ClientesList.ItemsSource = clientListFinal
                    .Where(c => !string.IsNullOrWhiteSpace(c.Nome) && !string.IsNullOrWhiteSpace(c.CPF_CNPJ)) // Filtra clientes válidos
                    .Select(c => $"{c.Nome} ({c.CPF_CNPJ})").ToList();
                Txt_CNABTotal.Text = totalParcelasCNAB; // Total de parcelas do CNAB
            }
        }
        
        // Exibir detalhes ao selecionar um cliente na lista
        private void ClientesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientesList.SelectedIndex >= 0)
            {
                var clienteSelecionado = clientListFinal[ClientesList.SelectedIndex];

                // Preencher o campo de texto com a data de emissão
                Txt_DataEmission.Text = clienteSelecionado.DatasEmissao;

                // Preencher a lista de parcelas formatadas do contrato
                ParcelasContratoList.ItemsSource = clienteSelecionado.Parcelas;

                // Atualizar o total de parcelas do cliente na interface
                Txt_ClientTotal.Text = $"R$ {clienteSelecionado.TotalParcelasCliente:N2}";
            }
        }
        private void Btn_ExportXLSX_Click(object sender, RoutedEventArgs e)
        {
            // Abrir uma caixa de diálogo para selecionar onde salvar o arquivo XLSX
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Salvar Arquivo CNAB em Excel";

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportClass refExportClass = new();

                refExportClass.ExportFileXLSX(clientListFinal, saveFileDialog.FileName);
            }
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