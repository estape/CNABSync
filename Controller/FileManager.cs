using CNAB_Sync.Model;
using System.IO;
using System.Windows;

namespace CNAB_Sync.Controller
{
    class FileManager
    {
        private List<Detalhe444> clientInfo = new List<Detalhe444>();

        public (List<Detalhe444>, string) ProcessingFile(string filePath)
        {
            clientInfo.Clear();

            decimal totalParcelasCNAB = 0; // Total de parcelas do arquivo CNAB
            long intCpfCnpj = 0;

            foreach (var linha in File.ReadLines(filePath))
            {
                try
                {
                    Detalhe444 _client = new()
                    {
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
                        Parcelas = [linha.Substring(126, 13)],
                        EspecieTitulo = linha.Substring(147, 2),
                        DatasEmissao = linha.Substring(150, 6),
                        TipoPessoaCedente = linha.Substring(159, 2),
                        NumeroTermoCessao = linha.Substring(173, 19),
                        ValorPresenteParcela = linha.Substring(192, 13),
                        IdentTipoInscricaoSacado = linha.Substring(218, 2),
                        CPF_CNPJ = linha.Substring(220, 14).Trim(),
                        Nome = linha.Substring(234, 40).Trim(),
                        EnderecoCompleto = linha.Substring(274, 40),
                        NumeroNotaFiscalDuplicata = linha.Substring(314, 9), // Sim, para duplicata
                        CEP = linha.Substring(326, 8),
                        Cedente = linha.Substring(334, 60),
                        ChaveNota = linha.Substring(394, 44), // Sim, para duplicata
                        NumeroSequencialRegistro = linha.Substring(438, 6)
                    };

                    // Verificar se o CPF/CNPJ e o nome estão preenchidos corretamente
                    if (string.IsNullOrWhiteSpace(_client.CPF_CNPJ) || string.IsNullOrWhiteSpace(_client.Nome))
                    {
                        continue; // Ignorar clientInfo com dados incompletos
                    }

                    // Se o cliente não estiver na lista, tenta adicionar um novo
                    try
                    {
                        intCpfCnpj = Convert.ToInt64(_client.CPF_CNPJ);
                    }
                    catch (SystemException)
                    {
                        MessageBox.Show("Erro ao importar, não foi possivel importar o Número inscrição do sacado.", "CNAB Sync - Erro Importação", MessageBoxButton.OK, MessageBoxImage.Error);
                        clientInfo.Clear();
                        return (clientInfo, "");
                    }

                    string cpfCnpjFormatado = intCpfCnpj.ToString();

                    // Verificar se há espaços dentro dos caracteres reservados
                    if (_client.CPF_CNPJ.Contains(' '))
                    {
                        MessageBox.Show("Erro ao importar, não foi possivel importar o Número inscrição do sacado.", "CNAB Sync - Erro Importação", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        MessageBox.Show("Erro ao importar, não foi possivel importar o Número inscrição do sacado.", "CNAB Sync - Erro Importação", MessageBoxButton.OK, MessageBoxImage.Error);
                        clientInfo.Clear();
                        return (clientInfo, ""); // Interromper a execução se o CPF/CNPJ for inválido
                    }

                    // -------------- adaptar o restante do código abaixo para a nova lógica --------------


                    // Procurar cliente na lista
                    _client = clientInfo.FirstOrDefault(c => c.CPF_CNPJ == cpfCnpjFormatado);

                    if (_client == null)
                    {
                        // Adiciona um novo cliente com o CPF/CNPJ formatado
                        _client = { Nome = nome, CPF_CNPJ = cpfCnpjFormatado };
                        clientInfo.Add(_client);
                    }

                    // Extrair o valor da parcela (posição 127 a 139)
                    string valorParcelaStr = linha.Substring(126, 13).Trim();
                    if (decimal.TryParse(valorParcelaStr, out decimal valorParcela))
                    {
                        valorParcela /= 100; // Convertendo centavos para reais

                        // Somar o valor da parcela ao total do Cliente
                        _client.TotalParcelasCliente += valorParcela;

                        // Somar o valor da parcela ao total do CNAB
                        totalParcelasCNAB += valorParcela;
                    }

                    // **OBSOLETO** Preencher a lista de datas de emissão (posição 151 a 156)
                    string dataEmissao = linha.Substring(150, 6).Trim();
                    if (dataEmissao.Length == 6)
                    {
                        // Formatar como DD/MM/AA
                        dataEmissao = $"{dataEmissao.Substring(0, 2)}/{dataEmissao.Substring(2, 2)}/{dataEmissao.Substring(4, 2)}";
                    }
                    _client.DatasEmissao = dataEmissao;

                    // Adicionar a parcela formatada ao Cliente
                    string dataVencimento = linha.Substring(120, 6);
                    if (dataVencimento.Length == 6)
                    {
                        dataVencimento = $"{dataVencimento.Substring(0, 2)}/{dataVencimento.Substring(2, 2)}/{dataVencimento.Substring(4, 2)}";
                    }
                    string numeroDocumento = linha.Substring(110, 10).Trim();
                    string parcelaFormatada = $"Número CCB: {numeroDocumento} | Vencimento: {dataVencimento} | Parcela a pagar: R$ {valorParcela:N2}";
                    _client.DataVencimentoTitulo = dataVencimento;
                    _client.Parcelas.Add(parcelaFormatada);
                    _client.NumeroDocumento = numeroDocumento;
                }
                catch(FormatException ex)
                {
                    MessageBox.Show("Arquivo corrompido", "CNAB Sync - Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return (clientInfo, $"R$ {totalParcelasCNAB:N2}");
        }
    }
}
