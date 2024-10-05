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
                    sheets.Append(sheet);

                    // Preencher a planilha com dados
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                    // Adicionar cabeçalhos
                    Row headerRow = new Row();
                    headerRow.Append(
                        new Cell() { CellValue = new CellValue("CPF/CNPJ"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Nome"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Data de Emissão"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Número do Documento"), DataType = CellValues.String },
                        new Cell() { CellValue = new CellValue("Data de Vencimento"), DataType = CellValues.String }
                    );
                    sheetData.Append(headerRow);

                    // Adicionar os dados dos clientes
                    decimal totalParcelasCNAB = 0;
                    foreach (var cliente in clientInfo)
                    {
                        if (cliente != null)  // Verificação se cliente é nulo
                        {
                            foreach (var parcela in cliente.Parcelas)
                            {
                                string valorParcela = "";  // Exemplo: valor da parcela (ajuste conforme necessário)
                                string dataVencimento = "";  // Exemplo: extraído da parcela (ajuste conforme necessário)

                                // Adicionar a linha com os dados da parcela
                                Row dataRow = new Row();
                                dataRow.Append(
                                    new Cell() { CellValue = new CellValue(cliente.CPF_CNPJ ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(cliente.Nome ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(cliente.DatasEmissao ?? string.Empty), DataType = CellValues.String },
                                    new Cell() { CellValue = new CellValue(cliente.NumeroDocumento), DataType = CellValues.String }, // Número do Documento
                                    new Cell() { CellValue = new CellValue(cliente.DataVencimentoTitulo), DataType = CellValues.String }
                                );
                                sheetData.Append(dataRow);

                                // Atualizar o total de parcelas para o cliente e CNAB
                                if (decimal.TryParse(valorParcela, out decimal parcelaDecimal))
                                {
                                    cliente.TotalParcelasCliente += parcelaDecimal;
                                    totalParcelasCNAB += parcelaDecimal;
                                }
                            }
                        }
                    }

                    // Adicionar linha com o total de parcelas do cliente
                    foreach (var cliente in clientInfo)
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
            catch (IOException IOex)
            {
                MessageBox.Show(string.Format("Erro ao exportar em Excel:\n{0}", IOex.Message), "CNAB Sync - Erro exportação Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}