using CNAB_Sync.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System.Windows;

namespace CNAB_Sync.Controller
{
    internal class ImportClass
    {
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

        public (List<Detalhe444>, string) ImportFileXLSX(string filename)
        {
            List<Detalhe444> clientInfoOutput = new List<Detalhe444>();
            decimal totalParcelasCNAB = 0; // Inicializa o total de parcelas do CNAB

            // Abrir o arquivo XLSX para leitura
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filename, false))
            {
                // Selecionar a primeira planilha
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                Sheet sheet = workbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

                // Ler os dados da planilha
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Processar cada linha (ignorando a primeira, que são os cabeçalhos)
                foreach (Row row in sheetData.Elements<Row>().Skip(1)) // Ignorar cabeçalho
                {
                    Detalhe444 detalhe = new Detalhe444
                    {
                        CPF_CNPJ = GetCellValue(workbookPart, row, 0), // Coluna A: CPF/CNPJ
                        Nome = GetCellValue(workbookPart, row, 1), // Coluna B: Nome
                        DatasEmissao = GetCellValue(workbookPart, row, 2), // Coluna C: Data de Emissão
                        NumeroDocumento = GetCellValue(workbookPart, row, 3), // Coluna D: Número do Documento
                        DataVencimentoTitulo = GetCellValue(workbookPart, row, 4) // Coluna E: Data de Vencimento
                    };

                    // Adiciona o detalhe à lista
                    clientInfoOutput.Add(detalhe);

                    // Adicionar o cálculo das parcelas para o total
                    if (decimal.TryParse(GetCellValue(workbookPart, row, 3), out decimal valorParcela))
                    {
                        totalParcelasCNAB += valorParcela;
                    }
                }

                // Lógica adicional para processar clientInfoOutput se necessário

                MessageBox.Show("Arquivo Excel importado com sucesso!", "Importação", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return (clientInfoOutput, totalParcelasCNAB.ToString());
        }
    }
}