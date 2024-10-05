using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNAB_Sync.Model
{
    internal class Detalhe444
    {
        public string IdentificacaoRegistro { get; set; } // Identificação do registro (posição 1 a 1)
        public string DataCarencia { get; set; } // Data de Carência (posição 2 a 7)
        public string TipoJuros { get; set; } // Tipo de Juros (posição 8 a 8)
        public string Branco1 { get; set; } // Branco (posição 9 a 10)
        public string TaxaJuros { get; set; } // Taxa de Juros (posição 11 a 20)
        public string Coobrigacao { get; set; } // Coobrigação (posição 21 a 22)
        public string CaracteristicaEspecial { get; set; } // Característica especial (posição 23 a 24)
        public string ModalidadeOperacao { get; set; } // Modalidade da operação (posição 25 a 28)
        public string NaturezaOperacao { get; set; } // Natureza da operação (posição 29 a 30)
        public string OrigemRecurso { get; set; } // Origem do recurso (posição 31 a 34)
        public string ClasseRiscoOperacao { get; set; } // Classe risco da operação (posição 35 a 36)
        public string Zeros1 { get; set; } // Zeros (posição 37 a 37)
        public string NumeroControleParticipante { get; set; } // Nº de controle do participante (posição 38 a 62)
        public string NumeroBanco { get; set; } // Número do banco (posição 63 a 65)
        public string Zeros2 { get; set; } // Zeros (posição 66 a 70)
        public string IdentificacaoTituloBanco { get; set; } // Identificação do título no banco (posição 71 a 81)
        public string DigitoNossoNumero { get; set; } // Dígito do nosso número (posição 82 a 82)
        public string ValorPago { get; set; } // Valor pago (posição 83 a 92)
        public string CondicaoEmissaoPapeletaCobranca { get; set; } // Condição para emissão da papeleta de cobrança (posição 93 a 93)
        public string IdentEmitePapeletaDebitoAutomatico { get; set; } // Ident. Se emite papeleta para débito automático (posição 94 a 94)
        public string DataLiquidacao { get; set; } // Data da liquidação (posição 95 a 100)
        public string IdentificacaoOperacaoBanco { get; set; } // Identificação da operação do banco (posição 101 a 104)
        public string IndicadorRateioCredito { get; set; } // Indicador rateio crédito (posição 105 a 105)
        public string EnderecamentoAvisoDebitoAutomatico { get; set; } // Endereçamento para aviso do débito automático em conta corrente (posição 106 a 106)
        public string Branco2 { get; set; } // Branco (posição 107 a 108)
        public string IdentificacaoOcorrencia { get; set; } // Identificação ocorrência (posição 109 a 110)
        public string NumeroDocumento { get; set; } // Nº do documento (posição 111 a 120)
        public string DataVencimentoTitulo { get; set; } // Data do vencimento do título (posição 121 a 126)
        public List<string> Parcelas { get; set; } = new List<string>(); // Valor do título (face) (posição 127 a 139)
        public string BancoEncarregadoCobranca { get; set; } // Banco encarregado da cobrança (posição 140 a 142)
        public string AgenciaDepositaria { get; set; } // Agência depositária (posição 143 a 147)
        public string EspecieTitulo { get; set; } // Espécie de título (posição 148 a 149)
        public string Identificacao { get; set; } // Identificação (posição 150 a 150)
        public string? DatasEmissao { get; set; } // Data da emissão do título (posição 151 a 156)
        public string PrimeiraInstrucao { get; set; } // 1ª instrução (posição 157 a 158)
        public string SegundaInstrucao { get; set; } // 2ª instrução (posição 159 a 159)
        public string TipoPessoaCedente { get; set; } // Tipo de pessoa do cedente (posição 160 a 161)
        public string JurosMora { get; set; } // Juros/Mora (posição 162 a 173)
        public string NumeroTermoCessao { get; set; } // Número do termo de cessão (posição 174 a 192)
        public string ValorPresenteParcela { get; set; } // Valor presente da parcela (posição 193 a 205)
        public string ValorAbatimento { get; set; } // Valor do abatimento (posição 206 a 218)
        public string IdentTipoInscricaoSacado { get; set; } // Identificação do tipo de inscrição do sacado (posição 219 a 220)
        public string? CPF_CNPJ { get; set; } //Nº inscrição do sacado (posição 221 a 234)
        public string? Nome { get; set; } // Nome do sacado (posição 235 a 274)
        public string EnderecoCompleto { get; set; } // Endereço completo (posição 275 a 314)
        public string NumeroNotaFiscalDuplicata { get; set; } // Número da nota fiscal da duplicata (posição 315 a 323)
        public string NumeroSerieNotaFiscalDuplicata { get; set; } // Número da série da nota fiscal da duplicata (posição 324 a 326)
        public string CEP { get; set; } // CEP (posição 327 a 334)
        public string Cedente { get; set; } // Cedente (posição 335 a 394)
        public string ChaveNota { get; set; } // Chave da nota (posição 395 a 438)
        public string NumeroSequencialRegistro { get; set; } // Nº sequencial do registro (posição 439 a 444)
        public decimal TotalParcelasCliente { get; set; } = 0; // Para armazenar o total de parcelas do cliente
    }
}