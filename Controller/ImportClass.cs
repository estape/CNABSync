using CNAB_Sync.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CNAB_Sync.Controller
{
    internal class ImportClass
    {
        private string GetCellString(WorkbookPart workbookPart, Row row, int cellIndex)
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

        public (List<Detalhe444>, string) ImportFileXLSX(string filename)
        {
            List<Detalhe444> clientInfoOutput = new List<Detalhe444>();
            decimal totalParcelasCNAB = 0; // Inicializa o total de parcelas do CNAB
            string _totalParcelasCliente = "";

            // Abrir o arquivo XLSX para leitura
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filename, false))
            {
                // Selecionar a primeira planilha
                try
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    Sheet sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                    WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

                    // Ler os dados da planilha
                    SheetData? sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Processar cada linha (ignorando a primeira, que são os cabeçalhos)
                    foreach (Row row in sheetData.Elements<Row>().Skip(1)) // Ignorar cabeçalho
                    {
                        Detalhe444 _lineClient = new Detalhe444
                        {
                            CPF_CNPJ = GetCellString(workbookPart, row, 0), // Coluna A: CPF/CNPJ
                            Nome = GetCellString(workbookPart, row, 1), // Coluna B: Nome
                            DatasEmissao = GetCellString(workbookPart, row, 2), // Coluna C: Data de Emissão
                            NumeroDocumento = GetCellString(workbookPart, row, 3), // Coluna D: Número do Documento
                            DataVencimentoTitulo = GetCellString(workbookPart, row, 4), // Coluna E: Data de Vencimento
                        };

                        _totalParcelasCliente = GetCellString(workbookPart, row, 5);
                        _lineClient.TotalParcelasCliente = decimal.Parse(_totalParcelasCliente);

                        // Adiciona o detalhe à lista
                        clientInfoOutput.Add(_lineClient);

                        totalParcelasCNAB += _lineClient.TotalParcelasCliente;
                    }

                    //MessageBox.Show("Arquivo Excel importado com sucesso!", "CNAB Sync - Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Talvez desnecessário a mensagem acima, retirando pode melhorar a dinamica da importação.
                }
                catch (SystemException)
                {
                    MessageBox.Show("Não foi possivel importar a planilha.", "CNAB Sync - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return (clientInfoOutput, totalParcelasCNAB.ToString());
        }
    }
}