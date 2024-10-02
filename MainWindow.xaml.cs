using CNAB_Sync.Model;
using CNAB_Sync.View;
using Microsoft.Win32;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
            
        }

        private void Btn_SaveCNAB_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_ImportREM_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos CNAB|*.REM";
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

        private void Btn_ImportTXT_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos CNAB|*.REM";
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

        private void Btn_ImportXLSX_Click(object sender, RoutedEventArgs e)
        {
            // Limpar todos os dados anteriores do front antes de importar novos
            ClearDataFront();

            // Abrir um diálogo para selecionar o arquivo XLSX
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
            openFileDialog.Title = "Importar Arquivo Excel";

            if (openFileDialog.ShowDialog() == true)
            {
                ImportClass refImportClass = new();
                (clientListFinal, totalParcelasCNAB) = refImportClass.ImportFileXLSX(openFileDialog.FileName);

                // Atualizar a interface com os dados retornados
                ClientesList.ItemsSource = clientListFinal
                    .Where(c => !string.IsNullOrWhiteSpace(c.Nome) && !string.IsNullOrWhiteSpace(c.CPF_CNPJ)) // Filtra clientes válidos
                    .Select(c => $"{c.Nome} ({c.CPF_CNPJ})").ToList();
                Txt_CNABTotal.Text = totalParcelasCNAB; // Total de parcelas do CNAB
            }
        }

        private void Btn_ExportREM_Click(object sender, RoutedEventArgs e)
        {
            // Abrir uma caixa de diálogo para selecionar onde salvar o arquivo
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Arquivos CNAB (*.REM)|*.REM";
            saveFileDialog.Title = "Salvar Arquivo CNAB .REM";

            if (saveFileDialog.ShowDialog() == true)
            {
                // Criar o conteúdo do arquivo CNAB .REM com base nos dados processados
                StringBuilder sb = new StringBuilder();

                // Adicionar o cabeçalho (adaptar para o layout desejado)
                sb.AppendLine("01REMESSA01COBRANCA       00000037920778000129EST GESTAO DE BENS S A        611PAULISTA S A   130624        MX0083561                                                                                                                                                                                                                                                                                                                                 000001");

                // Adicionar as parcelas associadas a cada cliente
                foreach (var cliente in clientes)
                {
                    foreach (var parcela in cliente.Parcelas)
                    {
                        // Aqui você pode recriar as linhas do arquivo .REM usando os dados da parcela e do cliente
                        // Você pode formatar os campos conforme necessário, usando Substring, PadRight, etc.
                        // Exemplo básico (modifique conforme o layout .REM desejado):
                        sb.AppendLine(parcela);  // Use a lógica para recriar as linhas do arquivo .REM
                    }
                }

                // Salvar o arquivo .REM no local selecionado
                File.WriteAllText(saveFileDialog.FileName, sb.ToString());

                MessageBox.Show("Arquivo .REM exportado com sucesso!", "Exportação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Btn_ExportTXT_Click(object sender, RoutedEventArgs e)
        {

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

        // Função auxiliar para obter o valor de uma célula
        private string GetCellValue(WorkbookPart workbookPart, Row row, int cellIndex)
        {
            // Verificar se o índice da célula está dentro do intervalo de células disponíveis
            if (row.Elements<Cell>().Count() > cellIndex)
            {
                Cell cell = row.Elements<Cell>().ElementAt(cellIndex);
                string value = cell.InnerText;

                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    return workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).InnerText;
                }

                return value;
            }

            // Retornar uma string vazia caso a célula não exista
            return string.Empty;
        }
    }
}