using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposto.Core.Domain
{
    public class NotaFiscalItem
    {
        public int Id { get; set; }
        public int IdNotaFiscal { get; set; }
        public string Cfop { get; set; }
        public string TipoIcms { get; set; }
        public double BaseIcms { get; set; }
        public double AliquotaIcms { get; set; }
        public double ValorIcms { get; set; }
        public string NomeProduto { get; set; }
        public string CodigoProduto { get; set; }

        // 3 – Exercício (Novo recurso)
        public double BaseIpi { get; set; }
        public double AliquotaIpi { get; set; }
        public double ValorIpi { get; set; }

        // 7 – Exercício (Novo recurso)
        public double Desconto { get; set; }
    }
}
