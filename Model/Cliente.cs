using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNAB_Sync.Model
{
    internal class Cliente
    {
        public string Nome { get; set; }
        public string CPF_CNPJ { get; set; }
        public List<string> Parcelas { get; set; } = new List<string>();
        public string DatasEmissao { get; set; } // Para armazenar a data de emissão
        public decimal TotalParcelasCliente { get; set; } = 0; // Para armazenar o total de parcelas do cliente
    }
}
