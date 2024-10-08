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

        // Função para concatenar todos os arrays de bytes
        private byte[] CombineByteArrays(List<byte[]> bytesList)
        {
            int totalLength = 0;
            foreach (var byteArray in bytesList)
            {
                totalLength += byteArray.Length;
            }

            byte[] result = new byte[totalLength];
            int offset = 0;

            foreach (var byteArray in bytesList)
            {
                Buffer.BlockCopy(byteArray, 0, result, offset, byteArray.Length);
                offset += byteArray.Length;
            }

            return result;
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

        public void SaveCNABSyncFile(List<Detalhe444> clientData, string pathSave)
        {
            clientInfo.Clear();

            List<byte[]> bytesFile = new List<byte[]>();
            byte []rawData;

            foreach (Detalhe444 _lineClient in clientData)
            {
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentificacaoRegistro ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.DataCarencia ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.TipoJuros ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Branco1 ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.TaxaJuros ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Coobrigacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.CaracteristicaEspecial ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ModalidadeOperacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NaturezaOperacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.OrigemRecurso ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ClasseRiscoOperacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Zeros1 ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroControleParticipante ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroBanco ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Zeros2 ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentificacaoTituloBanco ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.DigitoNossoNumero ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ValorPago ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.CondicaoEmissaoPapeletaCobranca ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentEmitePapeletaDebitoAutomatico ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.DataLiquidacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentificacaoOperacaoBanco ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IndicadorRateioCredito ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.EnderecamentoAvisoDebitoAutomatico ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Branco2 ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentificacaoOcorrencia ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroDocumento ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.DataVencimentoTitulo ?? string.Empty));

                // Converte a lista para uma string concatenada e depois para bytes
                bytesFile.Add(Encoding.ASCII.GetBytes(string.Join("_", _lineClient.Parcelas)));

                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.BancoEncarregadoCobranca ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.AgenciaDepositaria ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.EspecieTitulo ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Identificacao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.DatasEmissao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.PrimeiraInstrucao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.SegundaInstrucao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.TipoPessoaCedente ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.JurosMora ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroTermoCessao ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ValorPresenteParcela ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ValorAbatimento ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.IdentTipoInscricaoSacado ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.CPF_CNPJ ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Nome ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.EnderecoCompleto ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroNotaFiscalDuplicata ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroSerieNotaFiscalDuplicata ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.CEP ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.Cedente ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.ChaveNota ?? string.Empty));
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.NumeroSequencialRegistro ?? string.Empty));

                // Converte o decimal para string antes de adicionar como bytes
                bytesFile.Add(Encoding.ASCII.GetBytes(_lineClient.TotalParcelasCliente.ToString()));
            }

            // Concatena todos os arrays de bytes em um único array
            byte[] concatenatedBytes = CombineByteArrays(bytesFile);

            // Escreve os bytes concatenados no arquivo especificado
            File.WriteAllBytes(pathSave, concatenatedBytes);
        }

        public (List<Detalhe444>, string) LoadCNABSyncFile(string pathLoad)
        {
            List<Detalhe444> clientData = new List<Detalhe444>();

            // Ler todos os bytes do arquivo
            byte[] fileBytes = File.ReadAllBytes(pathLoad);

            // Converte os bytes para string (presume que todo o conteúdo é ASCII)
            string fileContent = Encoding.ASCII.GetString(fileBytes);
            decimal totalParcelasCNAB = 0;
            // Divide o conteúdo do arquivo em linhas, cada linha representando um registro
            string[] lines = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Crie um novo objeto Detalhe444
                Detalhe444 detalhe = new Detalhe444();

                // Divida a linha em campos individuais (assumindo que os campos foram gravados em ordem)
                string[] fields = line.Split('_'); // Usando '_' como separador, caso isso tenha sido definido

                // Assuma que os campos estão na mesma ordem das propriedades de Detalhe444
                int index = 0;

                // Agora, atribua os valores aos campos do objeto `Detalhe444`
                detalhe.IdentificacaoRegistro = fields.Length > index ? fields[index++] : null;
                detalhe.DataCarencia = fields.Length > index ? fields[index++] : null;
                detalhe.TipoJuros = fields.Length > index ? fields[index++] : null;
                detalhe.Branco1 = fields.Length > index ? fields[index++] : null;
                detalhe.TaxaJuros = fields.Length > index ? fields[index++] : null;
                detalhe.Coobrigacao = fields.Length > index ? fields[index++] : null;
                detalhe.CaracteristicaEspecial = fields.Length > index ? fields[index++] : null;
                detalhe.ModalidadeOperacao = fields.Length > index ? fields[index++] : null;
                detalhe.NaturezaOperacao = fields.Length > index ? fields[index++] : null;
                detalhe.OrigemRecurso = fields.Length > index ? fields[index++] : null;
                detalhe.ClasseRiscoOperacao = fields.Length > index ? fields[index++] : null;
                detalhe.Zeros1 = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroControleParticipante = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroBanco = fields.Length > index ? fields[index++] : null;
                detalhe.Zeros2 = fields.Length > index ? fields[index++] : null;
                detalhe.IdentificacaoTituloBanco = fields.Length > index ? fields[index++] : null;
                detalhe.DigitoNossoNumero = fields.Length > index ? fields[index++] : null;
                detalhe.ValorPago = fields.Length > index ? fields[index++] : null;
                detalhe.CondicaoEmissaoPapeletaCobranca = fields.Length > index ? fields[index++] : null;
                detalhe.IdentEmitePapeletaDebitoAutomatico = fields.Length > index ? fields[index++] : null;
                detalhe.DataLiquidacao = fields.Length > index ? fields[index++] : null;
                detalhe.IdentificacaoOperacaoBanco = fields.Length > index ? fields[index++] : null;
                detalhe.IndicadorRateioCredito = fields.Length > index ? fields[index++] : null;
                detalhe.EnderecamentoAvisoDebitoAutomatico = fields.Length > index ? fields[index++] : null;
                detalhe.Branco2 = fields.Length > index ? fields[index++] : null;
                detalhe.IdentificacaoOcorrencia = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroDocumento = fields.Length > index ? fields[index++] : null;
                detalhe.DataVencimentoTitulo = fields.Length > index ? fields[index++] : null;

                // A propriedade Parcelas é uma lista de strings, separadas por ','
                detalhe.Parcelas = fields.Length > index ? fields[index++].Split(',').ToList() : new List<string>();

                detalhe.BancoEncarregadoCobranca = fields.Length > index ? fields[index++] : null;
                detalhe.AgenciaDepositaria = fields.Length > index ? fields[index++] : null;
                detalhe.EspecieTitulo = fields.Length > index ? fields[index++] : null;
                detalhe.Identificacao = fields.Length > index ? fields[index++] : null;
                detalhe.DatasEmissao = fields.Length > index ? fields[index++] : null;
                detalhe.PrimeiraInstrucao = fields.Length > index ? fields[index++] : null;
                detalhe.SegundaInstrucao = fields.Length > index ? fields[index++] : null;
                detalhe.TipoPessoaCedente = fields.Length > index ? fields[index++] : null;
                detalhe.JurosMora = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroTermoCessao = fields.Length > index ? fields[index++] : null;
                detalhe.ValorPresenteParcela = fields.Length > index ? fields[index++] : null;
                detalhe.ValorAbatimento = fields.Length > index ? fields[index++] : null;
                detalhe.IdentTipoInscricaoSacado = fields.Length > index ? fields[index++] : null;
                detalhe.CPF_CNPJ = fields.Length > index ? fields[index++] : null;
                detalhe.Nome = fields.Length > index ? fields[index++] : null;
                detalhe.EnderecoCompleto = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroNotaFiscalDuplicata = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroSerieNotaFiscalDuplicata = fields.Length > index ? fields[index++] : null;
                detalhe.CEP = fields.Length > index ? fields[index++] : null;
                detalhe.Cedente = fields.Length > index ? fields[index++] : null;
                detalhe.ChaveNota = fields.Length > index ? fields[index++] : null;
                detalhe.NumeroSequencialRegistro = fields.Length > index ? fields[index++] : null;

                // Tente converter o valor para decimal
                detalhe.TotalParcelasCliente = fields.Length > index && decimal.TryParse(fields[index++], out decimal total) ? total : 0;

                // Atualiza o total de parcelas do CNAB
                totalParcelasCNAB += detalhe.TotalParcelasCliente;

                // Adiciona o objeto `Detalhe444` à lista de clientes
                clientData.Add(detalhe);


            }

            return (clientData, totalParcelasCNAB.ToString());
        }
    }
}