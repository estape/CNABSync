using CNAB_Sync.Model;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace CNAB_Sync.Controller
{
    class FileManager
    {
        private List<Detalhe444> clientInfo = new List<Detalhe444>();

        public (bool, string) CheckRequiredFields(Detalhe444 fieldRequired)
        {
            // Verifica campos obrigátórios sengundo o padrão CNAB 444, qualquer inconsistencia considerar como falso e encerrar o processo.
            var listFieldRequired = new string[]
            {
            nameof(fieldRequired.IdentificacaoRegistro),
            nameof(fieldRequired.Coobrigacao),
            nameof(fieldRequired.NumeroControleParticipante),
            nameof(fieldRequired.Zeros2),
            nameof(fieldRequired.IdentificacaoOcorrencia),
            nameof(fieldRequired.NumeroDocumento),
            nameof(fieldRequired.DataVencimentoTitulo),
            nameof(fieldRequired.Parcelas),
            nameof(fieldRequired.EspecieTitulo),
            nameof(fieldRequired.DatasEmissao),
            nameof(fieldRequired.TipoPessoaCedente),
            nameof(fieldRequired.NumeroTermoCessao),
            nameof(fieldRequired.ValorPresenteParcela),
            nameof(fieldRequired.IdentTipoInscricaoSacado),
            nameof(fieldRequired.EnderecoCompleto),
            nameof(fieldRequired.CEP),
            nameof(fieldRequired.Cedente),
            nameof(fieldRequired.NumeroSequencialRegistro)
            };

            foreach (var fieldRequiredItem in listFieldRequired)
            {
                // Obtém a propriedade pelo nome
                PropertyInfo propInfo = typeof(Detalhe444).GetProperty(fieldRequiredItem);

                if (propInfo != null)
                {
                    // Obtém o valor da propriedade
                    var value = propInfo.GetValue(fieldRequired)?.ToString();

                    // Verifica se o valor é nulo ou vazio
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        MessageBox.Show(string.Format("Arquivo corrompido, nem todos os campos obrigatórios estão preenchidos no arquivo.\n[Linha 50]\nCampo: {0}", fieldRequiredItem), "CNAB Sync - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return (false, fieldRequiredItem);
                    }
                }
            }

            return (true, "");
        }

        public (List<Detalhe444>, string) ProcessingFile(string filePath)
        {
            try
            {
                clientInfo.Clear();

                decimal totalParcelasCNAB = 0; // Total de parcelas do arquivo CNAB
                long intCpfCnpj = 0;
                Detalhe444 _lineClient;

                foreach (var linha in File.ReadLines(filePath))
                {
                    // Extrair CPF/CNPJ (posições 221 a 234)
                    string cpfCnpj = linha.Substring(220, 14).Trim();

                    // Extrair Nome (posições 235 a 274)
                    string nome = linha.Substring(234, 40).Trim();

                    // Verificar se o CPF/CNPJ e o nome estão preenchidos corretamente
                    if (string.IsNullOrWhiteSpace(cpfCnpj) || string.IsNullOrWhiteSpace(nome))
                    {
                        continue; // Ignorar clientInfo com dados incompletos
                    }

                    // Se o cliente não estiver na lista, tenta adicionar um novo
                    try
                    {
                        intCpfCnpj = Convert.ToInt64(cpfCnpj); // Tenta remover os zeros extra a esquerda, se não for possivel o texto extraido não é um CNPJ ou CPF ou pode estar com formato inválido.
                    }
                    catch (System.FormatException ex)
                    {
                        MessageBox.Show(string.Format("Erro ao importar, arquivo está corrompido.\n\n{0}\n\n[Linha 90]", ex.ToString()), "CNAB Sync - Arquivo corrompido", MessageBoxButton.OK, MessageBoxImage.Error);
                        clientInfo.Clear();
                        return (clientInfo, "");
                    }

                    string cpfCnpjFormatado = intCpfCnpj.ToString();

                    // Verifica se CNPJ está com 13 digitos ou CPF está com 10 digitos ou não, devido a removção de zeros a esquerda anteriormente.
                    if (cpfCnpj.Contains(' '))
                    {
                        MessageBox.Show("Erro ao importar, arquivo corrompido.\n[Linha 100]", "CNAB Sync - Erro Importação", MessageBoxButton.OK, MessageBoxImage.Error);
                        clientInfo.Clear();
                        return (clientInfo, ""); // Arquivo comprometido, interromper processamento
                    }

                    // Verificar se é CNPJ (14 dígitos) ou CPF (11 dígitos)
                    if (cpfCnpjFormatado.Length == 13)
                    {
                        // Formatar como CNPJ: 00.000.000/0000-00
                        cpfCnpjFormatado = "0" + cpfCnpjFormatado;
                        cpfCnpjFormatado = $"CNPJ: {cpfCnpjFormatado.Substring(0, 2)}.{cpfCnpjFormatado.Substring(2, 3)}.{cpfCnpjFormatado.Substring(5, 3)}/{cpfCnpjFormatado.Substring(8, 4)}-{cpfCnpjFormatado.Substring(12, 2)}";
                    }
                    else if (cpfCnpjFormatado.Length == 14)
                    {
                        // Formatar como CNPJ: 00.000.000/0000-00
                        cpfCnpjFormatado = $"CNPJ: {cpfCnpjFormatado.Substring(0, 2)}.{cpfCnpjFormatado.Substring(2, 3)}.{cpfCnpjFormatado.Substring(5, 3)}/{cpfCnpjFormatado.Substring(8, 4)}-{cpfCnpjFormatado.Substring(12, 2)}";
                    }
                    else if (cpfCnpjFormatado.Length == 10)
                    {
                        // Formatar como CPF: 000.000.000-00
                        cpfCnpjFormatado = "0" + cpfCnpjFormatado;
                        cpfCnpjFormatado = $"CPF: {cpfCnpjFormatado.Substring(0, 3)}.{cpfCnpjFormatado.Substring(3, 3)}.{cpfCnpjFormatado.Substring(6, 3)}-{cpfCnpjFormatado.Substring(9, 2)}";
                    }
                    else if (cpfCnpjFormatado.Length == 11)
                    {
                        // Formatar como CPF: 000.000.000-00
                        cpfCnpjFormatado = $"CPF: {cpfCnpjFormatado.Substring(0, 3)}.{cpfCnpjFormatado.Substring(3, 3)}.{cpfCnpjFormatado.Substring(6, 3)}-{cpfCnpjFormatado.Substring(9, 2)}";
                    }
                    else
                    {
                        // Exibir mensagem de erro se não for um CPF ou CNPJ válido
                        MessageBox.Show("Erro ao importar Número inscrição do sacado.\n[Linha 131]", "CNAB Sync - Erro Importação", MessageBoxButton.OK, MessageBoxImage.Error);
                        clientInfo.Clear();
                        return (clientInfo, ""); // Interromper a execução se o CPF/CNPJ for inválido
                    }

                    // Procurar cliente na lista
                    _lineClient = clientInfo.FirstOrDefault(c => c.CPF_CNPJ == cpfCnpjFormatado);

                    if (_lineClient == null)
                    {
                        // Adiciona um novo cliente com o CPF/CNPJ formatado
                        _lineClient = new Detalhe444 {
                            IdentificacaoRegistro = linha.Substring(0, 1),
                            Coobrigacao = linha.Substring(20, 2),
                            NumeroControleParticipante = linha.Substring(37, 25),
                            NumeroBanco = linha.Substring(62, 3), // Sim, para cheque
                            Zeros2 = linha.Substring(65, 5),
                            ValorPago = linha.Substring(82, 10), // Sim, para ocorrências de liquidação
                            DataLiquidacao = linha.Substring(94, 6), // Sim, para ocorrências de liquidação
                            IdentificacaoOcorrencia = linha.Substring(108, 2),
                            NumeroDocumento = linha.Substring(110, 10),
                            DataVencimentoTitulo = linha.Substring(120, 6),
                            EspecieTitulo = linha.Substring(147, 2),
                            TipoPessoaCedente = linha.Substring(159, 2),
                            NumeroTermoCessao = linha.Substring(173, 19),
                            ValorPresenteParcela = linha.Substring(192, 13),
                            IdentTipoInscricaoSacado = linha.Substring(218, 2),
                            CPF_CNPJ = cpfCnpjFormatado,
                            Nome = nome,
                            EnderecoCompleto = linha.Substring(274, 40),
                            NumeroNotaFiscalDuplicata = linha.Substring(314, 9), // Sim, para duplicata
                            CEP = linha.Substring(326, 8),
                            Cedente = linha.Substring(334, 60),
                            ChaveNota = linha.Substring(394, 44), // Sim, para duplicata
                            NumeroSequencialRegistro = linha.Substring(438, 6)
                        };

                        clientInfo.Add(_lineClient);

                    }

                    // Extrair o valor da parcela (posição 127 a 139)
                    string valorParcelaStr = linha.Substring(126, 13).Trim();
                    if (decimal.TryParse(valorParcelaStr, out decimal valorParcela))
                    {
                        valorParcela /= 100; // Convertendo centavos para reais

                        // Somar o valor da parcela ao total do Cliente
                        _lineClient.TotalParcelasCliente += valorParcela;

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
                    _lineClient.DatasEmissao = dataEmissao;

                    // Aciona o CheckRequiredFields para verificar os campos obrigatórios se não são vazios.
                    var (fieldRequiredBool, fieldError) = CheckRequiredFields(_lineClient);
                    if (!fieldRequiredBool)
                    {
                        MessageBox.Show(string.Format("Erro na leitura, o arquivo não cumpre com um dos campos obrigatórios preenchidos, campo: {0}", fieldError), "CNAB Sync - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return (clientInfo, "");
                    }

                    // Adicionar a parcela formatada ao Cliente
                    string dataVencimento = linha.Substring(120, 6);
                    if (dataVencimento.Length == 6)
                    {
                        dataVencimento = $"{dataVencimento.Substring(0, 2)}/{dataVencimento.Substring(2, 2)}/{dataVencimento.Substring(4, 2)}";
                    }
                    string numeroDocumento = linha.Substring(110, 10).Trim();
                    string parcelaFormatada = $"Número CCB: {numeroDocumento} | Vencimento: {dataVencimento} | Parcela a pagar: R$ {valorParcela:N2}";
                    _lineClient.DataVencimentoTitulo = dataVencimento;
                    _lineClient.Parcelas.Add(parcelaFormatada);
                    _lineClient.NumeroDocumento = numeroDocumento;
                }

                return (clientInfo, $"R$ {totalParcelasCNAB:N2}");
            }
            catch(SystemException)
            {
                MessageBox.Show("Arquivo corrompido.\n[Linha 219]", "CNAB Sync - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return (clientInfo, "");
            }
        }
    }
}