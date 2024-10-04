using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNAB_Sync.Model
{
    internal class Detalhe444
    {
        /// <summary>
        /// Identificação do registro (posição 1 a 1). (Obrigatório).
        /// </summary>
        public required string IdentificacaoRegistro { get; set; }

        /// <summary>
        /// Data de Carência (posição 2 a 7).
        /// </summary>
        public string? DataCarencia { get; set; }

        /// <summary>
        /// Tipo de Juros (posição 8 a 8).
        /// </summary>
        public string? TipoJuros { get; set; }

        /// <summary>
        /// Branco (posição 9 a 10).
        /// </summary>
        public string? Branco1 { get; set; }

        /// <summary>
        /// Taxa de Juros (posição 11 a 20).
        /// </summary>
        public string? TaxaJuros { get; set; }

        /// <summary>
        /// Coobrigação (posição 21 a 22). (Obrigatório).
        /// </summary>
        public required string Coobrigacao { get; set; }

        /// <summary>
        /// Característica especial (posição 23 a 24).
        /// </summary>
        public string? CaracteristicaEspecial { get; set; }

        /// <summary>
        /// Modalidade da operação (posição 25 a 28).
        /// </summary>
        public string? ModalidadeOperacao { get; set; }

        /// <summary>
        /// Natureza da operação (posição 29 a 30).
        /// </summary>
        public string? NaturezaOperacao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? OrigemRecurso { get; set; } // Origem do recurso (posição 31 a 34)

        /// <summary>
        /// 
        /// </summary>
        public string? ClasseRiscoOperacao { get; set; } // Classe risco da operação (posição 35 a 36)

        /// <summary>
        /// 
        /// </summary>
        public string? Zeros1 { get; set; } // Zeros (posição 37 a 37)

        /// <summary>
        /// Nº de controle do participante (posição 38 a 62) (Obrigatório)
        /// </summary>
        public required string NumeroControleParticipante { get; set; }

        /// <summary>
        /// Número do banco (posição 63 a 65). (Obrigatório).
        /// </summary>
        public required string NumeroBanco { get; set; }

        /// <summary>
        /// Zeros (posição 66 a 70). (Obrigatório).
        /// </summary>
        public required string Zeros2 { get; set; }

        /// <summary>
        /// Identificação do título no banco (posição 71 a 81).
        /// </summary>
        public string? IdentificacaoTituloBanco { get; set; }

        /// <summary>
        /// Dígito do nosso número (posição 82 a 82).
        /// </summary>
        public string? DigitoNossoNumero { get; set; }

        /// <summary>
        /// Valor pago (posição 83 a 92). (Obrigatório).
        /// </summary>
        public required string ValorPago { get; set; }

        /// <summary>
        /// Condição para emissão da papeleta de cobrança (posição 93 a 93).
        /// </summary>
        public string? CondicaoEmissaoPapeletaCobranca { get; set; }

        /// <summary>
        /// Identifica se emite papeleta para débito automático (posição 94 a 94).
        /// </summary>
        public string? IdentEmitePapeletaDebitoAutomatico { get; set; }

        /// <summary>
        /// Data da liquidação (posição 95 a 100). (Obrigatório).
        /// </summary>
        public required string DataLiquidacao { get; set; }

        /// <summary>
        /// Identificação da operação do banco (posição 101 a 104).
        /// </summary>
        public string? IdentificacaoOperacaoBanco { get; set; }

        /// <summary>
        /// Indicador rateio crédito (posição 105 a 105).
        /// </summary>
        public string? IndicadorRateioCredito { get; set; }

        /// <summary>
        /// Endereçamento para aviso do débito automático em conta corrente (posição 106 a 106).
        /// </summary>
        public string? EnderecamentoAvisoDebitoAutomatico { get; set; }

        /// <summary>
        /// Branco (posição 107 a 108)
        /// </summary>
        public string? Branco2 { get; set; }

        /// <summary>
        /// Identificação ocorrência (posição 109 a 110). (Obrigatório).
        /// </summary>
        public required string IdentificacaoOcorrencia { get; set; }

        /// <summary>
        /// Nº do documento (posição 111 a 120). (Obrigatório).
        /// </summary>
        public required string NumeroDocumento { get; set; }

        /// <summary>
        /// Data do vencimento do título (posição 121 a 126). (Obrigatório).
        /// </summary>
        public required string DataVencimentoTitulo { get; set; }

        /// <summary>
        /// Valor do título (face) (posição 127 a 139). (Obrigatório).
        /// </summary>
        public required List<string> Parcelas { get; set; } = [];

        /// <summary>
        /// Banco encarregado da cobrança (posição 140 a 142).
        /// </summary>
        public string? BancoEncarregadoCobranca { get; set; }

        /// <summary>
        /// Agência depositária (posição 143 a 147).
        /// </summary>
        public string? AgenciaDepositaria { get; set; }

        /// <summary>
        /// Espécie de título (posição 148 a 149). (Obrigatório).
        /// </summary>
        public required string EspecieTitulo { get; set; }

        /// <summary>
        /// Identificação (posição 150 a 150)
        /// </summary>
        public string? Identificacao { get; set; }

        /// <summary>
        /// Data da emissão do título (posição 151 a 156). (Obrigatório).
        /// </summary>
        public required string DatasEmissao { get; set; }

        /// <summary>
        /// 1ª instrução (posição 157 a 158).
        /// </summary>
        public string? PrimeiraInstrucao { get; set; }

        /// <summary>
        /// 2ª instrução (posição 159 a 159).
        /// </summary>
        public string? SegundaInstrucao { get; set; }

        /// <summary>
        /// Tipo de pessoa do cedente (posição 160 a 161). (Obrigatório).
        /// </summary>
        public required string TipoPessoaCedente { get; set; }

        /// <summary>
        /// Juros/Mora (posição 162 a 173).
        /// </summary>
        public string? JurosMora { get; set; }

        /// <summary>
        /// Número do termo de cessão (posição 174 a 192). (Obrigatório).
        /// </summary>
        public required string NumeroTermoCessao { get; set; }

        /// <summary>
        /// Valor presente da parcela (posição 193 a 205). (Obrigatório).
        /// </summary>
        public required string ValorPresenteParcela { get; set; }

        /// <summary>
        /// Valor do abatimento (posição 206 a 218).
        /// </summary>
        public string? ValorAbatimento { get; set; }

        /// <summary>
        /// Identificação do tipo de inscrição do sacado (posição 219 a 220). (Obrigatório).
        /// </summary>
        public required string IdentTipoInscricaoSacado { get; set; }

        /// <summary>
        /// Nº inscrição do sacado (posição 221 a 234). (Obrigatório).
        /// </summary>
        public required string CPF_CNPJ { get; set; }

        /// <summary>
        /// Nome do sacado (posição 235 a 274). (Obrigatório).
        /// </summary>
        public required string Nome { get; set; }

        /// <summary>
        /// Endereço completo (posição 275 a 314). (Obrigatório).
        /// </summary>
        public required string EnderecoCompleto { get; set; }

        /// <summary>
        /// Número da nota fiscal da duplicata (posição 315 a 323). (Obrigatório).
        /// </summary>
        public required string NumeroNotaFiscalDuplicata { get; set; }

        /// <summary>
        /// Número da série da nota fiscal da duplicata (posição 324 a 326).
        /// </summary>
        public string? NumeroSerieNotaFiscalDuplicata { get; set; }

        /// <summary>
        /// CEP (posição 327 a 334). (Obrigatório).
        /// </summary>
        public required string CEP { get; set; }

        /// <summary>
        /// Cedente (posição 335 a 394). (Obrigátório).
        /// </summary>
        public required string Cedente { get; set; }

        /// <summary>
        /// Chave da nota (posição 395 a 438). (Obrigatório).
        /// </summary>
        public required string ChaveNota { get; set; }

        /// <summary>
        /// Nº sequencial do registro (posição 439 a 444). (Obrigatório).
        /// </summary>
        public required string NumeroSequencialRegistro { get; set; }

        /// <summary>
        /// Para armazenar o total de parcelas do cliente.
        /// </summary>
        public decimal TotalParcelasCliente { get; set; } = 0;
    }
}