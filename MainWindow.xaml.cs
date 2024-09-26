using CNAB_Sync.Model;
using CNAB_Sync.View;
using Microsoft.Win32;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows;

namespace Leitor_CNAB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Cliente> clientes = new List<Cliente>();

        private void ClearData()
        {
            // Limpar a lista de clientes
            clientes.Clear();

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
                ProcessarArquivo(openFileDialog.FileName);
            }
        }

        // Exibir detalhes ao selecionar um cliente na lista
        private void ClientesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ClientesList.SelectedIndex >= 0)
            {
                var clienteSelecionado = clientes[ClientesList.SelectedIndex];

                // Preencher o campo de texto com a data de emissão (mostrando apenas uma vez)
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
            openFileDialog.Filter = "Arquivos CNAB|*.TXT";
            if (openFileDialog.ShowDialog() == true)
            {
                ProcessarArquivo(openFileDialog.FileName);
            }
        }

        private void Btn_ImportXLSX_Click(object sender, RoutedEventArgs e)
        {
            // Limpar todos os dados anteriores antes de importar novos
            ClearData();

            // Abrir um diálogo para selecionar o arquivo XLSX
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivo Excel (*.xlsx)|*.xlsx";
            openFileDialog.Title = "Importar Arquivo Excel";

            if (openFileDialog.ShowDialog() == true)
            {
                // Abrir o arquivo XLSX para leitura
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(openFileDialog.FileName, false))
                {
                    // Selecionar a primeira planilha
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    Sheet sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                    WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

                    // Ler os dados da planilha
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    decimal totalParcelasCNAB = 0; // Inicializa o total de parcelas do CNAB

                    // Processar cada linha (ignorando a primeira, que são os cabeçalhos)
                    foreach (Row row in sheetData.Elements<Row>().Skip(1)) // Ignorar cabeçalho
                    {
                        string tipoLinha = GetCellValue(workbookPart, row, 2); // Coluna C: Tipo da linha

                        // Ignorar linhas que representam totais
                        if (tipoLinha == "Total Cliente" || tipoLinha == "Total CNAB")
                        {
                            continue; // Ignora essas linhas
                        }

                        string cpfCnpj = GetCellValue(workbookPart, row, 0); // Coluna A: CPF/CNPJ
                        string nome = GetCellValue(workbookPart, row, 1);    // Coluna B: Nome
                        string dataEmissao = GetCellValue(workbookPart, row, 2); // Coluna C: Data de Emissão
                        string valorParcela = GetCellValue(workbookPart, row, 3); // Coluna D: Valor da Parcela
                        string dataVencimento = GetCellValue(workbookPart, row, 5); // Coluna F: Data de Vencimento

                        // Verificar se o cliente já existe na lista
                        var cliente = clientes.FirstOrDefault(c => c.CPF_CNPJ == cpfCnpj);
                        if (cliente == null)
                        {
                            // Adicionar um novo cliente
                            cliente = new Cliente { Nome = nome, CPF_CNPJ = cpfCnpj };
                            clientes.Add(cliente);
                        }

                        // Adicionar a parcela ao cliente
                        cliente.Parcelas.Add($"Valor: {valorParcela} | Data de Vencimento: {dataVencimento}");

                        // Se a data de emissão ainda não foi atribuída, faça isso
                        if (string.IsNullOrWhiteSpace(cliente.DatasEmissao))
                        {
                            cliente.DatasEmissao = dataEmissao;
                        }

                        // Atualizar o total de parcelas do cliente
                        if (decimal.TryParse(valorParcela, out decimal valorDecimal))
                        {
                            cliente.TotalParcelasCliente += valorDecimal;

                            // Atualizar o total de parcelas do CNAB
                            totalParcelasCNAB += valorDecimal;
                        }
                    }

                    // Atualizar o campo de total de parcelas do CNAB na interface
                    Txt_CNABTotal.Text = $"R$ {totalParcelasCNAB:N2}";

                    MessageBox.Show("Arquivo Excel importado com sucesso!", "Importação", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Atualizar a lista de clientes na interface
                    ClientesList.ItemsSource = clientes.Select(c => $"{c.Nome} ({c.CPF_CNPJ})").ToList();
                }
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
                // Criar o arquivo Excel usando OpenXML
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(saveFileDialog.FileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());

                    // Criar uma nova planilha
                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Adicionar a planilha ao documento
                    Sheet sheet = new Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "CNAB Data"
                    };
                    sheets.Append(sheet);

                    // Preencher a planilha com dados
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Adicionar cabeçalhos
                    Row headerRow = new Row();
                    headerRow.Append(
                        new Cell() { CellValue = new CellValue("CPF/CNPJ"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Nome"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Data de Emissão"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Valor da Parcela"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Número do Documento"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Data de Vencimento"), DataType = CellValues.String }
                    );
                    sheetData.Append(headerRow);

                    // Adicionar os dados dos clientes
                    decimal totalParcelasCNAB = 0;
                    foreach (var cliente in clientes)
                    {
                        if (cliente != null)  // Verificação se cliente é nulo
                        {
                            string dataEmissao = cliente.DatasEmissao;  // Pegar a primeira data de emissão
                            foreach (var parcela in cliente.Parcelas)
                            {
                                string valorParcela = "136,16";  // Exemplo: valor da parcela (ajuste conforme necessário)
                                string dataVencimento = "15/09/23";  // Exemplo: extraído da parcela (ajuste conforme necessário)

                                // Adicionar a linha com os dados da parcela
                                Row dataRow = new Row();
                                dataRow.Append(
                                    new Cell() { CellValue = new CellValue(cliente.CPF_CNPJ ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(cliente.Nome ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(dataEmissao ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(valorParcela), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(""), DataType = CellValues.String }, // Número do Documento
                                    new Cell() { CellValue = new CellValue(dataVencimento), DataType = CellValues.String }
                                );
                                sheetData.Append(dataRow);

                                // Atualizar o total de parcelas para o cliente e CNAB
                                if (decimal.TryParse(valorParcela, out decimal parcelaDecimal))
                                {
                                    cliente.TotalParcelasCliente += parcelaDecimal;
                                    totalParcelasCNAB += parcelaDecimal;
                                }

                                // Deixar dataEmissao em branco para as próximas parcelas do mesmo cliente
                                dataEmissao = null;
                            }
                        }
                    }

                    // Adicionar linha com o total de parcelas do cliente
                    foreach (var cliente in clientes)
                    {
                        Row totalClienteRow = new Row();
                        totalClienteRow.Append(
                            new Cell() { CellValue = new CellValue(cliente.CPF_CNPJ), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(cliente.Nome), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue("Total Cliente"), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(cliente.TotalParcelasCliente.ToString("N2")), DataType = CellValues.String }
                        );
                        sheetData.Append(totalClienteRow);
                    }

                    // Adicionar linha com o total geral de parcelas do CNAB
                    Row totalCNABRow = new Row();
                    totalCNABRow.Append(
                        new Cell() { CellValue = new CellValue("Total CNAB"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(""), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue(totalParcelasCNAB.ToString("N2")), DataType = CellValues.String }
                    );
                    sheetData.Append(totalCNABRow);

                    // Salvar o arquivo
                    workbookPart.Workbook.Save();
                    MessageBox.Show("Arquivo Excel exportado com sucesso!", "Exportação", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

        private void ProcessarArquivo(string filePath)
        {
            ClearData();
            
            clientes.Clear();
            decimal totalParcelasCNAB = 0; // Total de parcelas do arquivo CNAB

            foreach (var linha in File.ReadLines(filePath))
            {
                // Extrair CPF/CNPJ (posições 221 a 234)
                string cpfCnpj = linha.Substring(220, 14).Trim();

                // Extrair Nome (posições 235 a 274)
                string nome = linha.Substring(234, 40).Trim();

                // Verificar se o CPF/CNPJ e o nome estão preenchidos corretamente
                if (string.IsNullOrWhiteSpace(cpfCnpj) || string.IsNullOrWhiteSpace(nome))
                {
                    continue; // Ignorar clientes com dados incompletos
                }

                // Procurar cliente na lista
                var cliente = clientes.FirstOrDefault(c => c.CPF_CNPJ == cpfCnpj);

                if (cliente == null)
                {
                    // Se o cliente não estiver na lista, adiciona um novo
                    cliente = new Cliente { Nome = nome, CPF_CNPJ = cpfCnpj };
                    clientes.Add(cliente);
                }

                // Extrair o valor da parcela (posição 127 a 139)
                string valorParcelaStr = linha.Substring(126, 13).Trim();
                if (decimal.TryParse(valorParcelaStr, out decimal valorParcela))
                {
                    valorParcela /= 100; // Convertendo centavos para reais

                    // Somar o valor da parcela ao total do cliente
                    cliente.TotalParcelasCliente += valorParcela;

                    // Somar o valor da parcela ao total do CNAB
                    totalParcelasCNAB += valorParcela;
                }

                // Preencher a lista de datas de emissão (posição 151 a 156)
                string dataEmissao = linha.Substring(150, 6).Trim();
                if (dataEmissao.Length == 6)
                {
                    // Formatar como DD/MM/AA
                    dataEmissao = $"{dataEmissao.Substring(0, 2)}/{dataEmissao.Substring(2, 2)}/{dataEmissao.Substring(4, 2)}";
                }
                cliente.DatasEmissao = dataEmissao;

                // Adicionar a parcela formatada ao cliente
                string dataVencimento = linha.Substring(120, 6);
                if (dataVencimento.Length == 6)
                {
                    dataVencimento = $"{dataVencimento.Substring(0, 2)}/{dataVencimento.Substring(2, 2)}/{dataVencimento.Substring(4, 2)}";
                }
                string numeroDocumento = linha.Substring(110, 10).Trim();
                string parcelaFormatada = $"Número CCB: {numeroDocumento} | Vencimento: {dataVencimento} | Parcela a pagar: R$ {valorParcela:N2}";
                cliente.Parcelas.Add(parcelaFormatada);
            }

            // Atualizar a lista de clientes na interface, removendo clientes inválidos
            ClientesList.ItemsSource = clientes
                .Where(c => !string.IsNullOrWhiteSpace(c.Nome) && !string.IsNullOrWhiteSpace(c.CPF_CNPJ)) // Filtra clientes válidos
                .Select(c => $"{c.Nome} ({c.CPF_CNPJ})").ToList();

            // Atualizar o total de parcelas do CNAB na interface
            Txt_CNABTotal.Text = $"R$ {totalParcelasCNAB:N2}";
        }

        // Função auxiliar para obter o valor de uma célula
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