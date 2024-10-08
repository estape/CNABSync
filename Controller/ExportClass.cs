using CNAB_Sync.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Windows;

namespace CNAB_Sync.Controller
{
    internal class ExportClass
    {
        public void ExportFileXLSX(List<Detalhe444> clientInfo, string pathExport)
        {
            decimal totalParcelasCNAB = 0;

            try
            {
                // Criar o arquivo Excel usando OpenXML
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(pathExport, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
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
                    if (sheets != null)
                    {
                        sheets.Append(sheet);
                    }


                    // Preencher a planilha com dados
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Adicionar cabeçalhos
                    Row headerRow = new Row();
                    headerRow.Append(
                        new Cell() { CellValue = new CellValue("CPF/CNPJ"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Nome"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Total parcelas de cliente"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Número de parcelas"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Ultima data de vencimento"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Total CNAB"), DataType = CellValues.String }
                    );
                    
                    if (sheetData != null)
                    {
                        sheetData.Append(headerRow);
                    }

                    // Adicionar linhas com o total de parcelas do cliente
                    foreach (var _lineClient in clientInfo)
                    {
                        totalParcelasCNAB += _lineClient.TotalParcelasCliente;

                        string _localTotalParcelas = $"R$ {_lineClient.TotalParcelasCliente.ToString()}";
                        string _localTotalParcelasCNAB = $"R$ {totalParcelasCNAB:N2}";



                        Row clientInfoSet = new Row();
                        clientInfoSet.Append(
                            new Cell() { CellValue = new CellValue(_lineClient.CPF_CNPJ ?? string.Empty), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(_lineClient.Nome ?? string.Empty), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(_localTotalParcelas), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(_lineClient.Parcelas.Count.ToString()), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(_lineClient.DataVencimentoTitulo ?? string.Empty), DataType = CellValues.String },
                            new Cell() { CellValue = new CellValue(_localTotalParcelasCNAB), DataType = CellValues.String }

                        );
                        
                        if (sheetData != null)
                        {
                            sheetData.Append(clientInfoSet);
                        }
                    }
                    // Salvar o arquivo
                    workbookPart.Workbook.Save();
                }
            }
            catch (IOException IOex)
            {
                MessageBox.Show(string.Format("Não foi possível exportar em Excel:\n{0}", IOex.Message), "CNAB Sync - Exportação Excel", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
