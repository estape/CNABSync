using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Leitor_CNAB.Controller
{
    class FileIO
    {
        public static void OpenFileCNAB(string filename)
        {
            MessageBox.Show("Arquivo aberto com sucesso", "Funcionou", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
