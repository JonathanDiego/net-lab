using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Imposto.Core.Data;

namespace Imposto.Core.Domain
{
    public class NotaFiscal
    {
        public int Id { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int Serie { get; set; }
        public string NomeCliente { get; set; }

        public string EstadoDestino { get; set; }
        public string EstadoOrigem { get; set; }

        // alterado de IEnumerable para List
        public List<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();            
        }

        public bool EmitirNotaFiscal(Pedido pedido)
        {
            string nomeXML;            
            this.Serie = new Random().Next(Int32.MaxValue);
            this.NomeCliente = pedido.NomeCliente;

            // 5 – Exercício (Correção de BUG)
            // os atributos EstadoOrigem e EstadoDestino estavam invertidos na atribuição, gerando o BUG
            this.EstadoDestino = pedido.EstadoDestino;
            this.EstadoOrigem = pedido.EstadoOrigem;

            foreach (PedidoItem itemPedido in pedido.ItensDoPedido)
            {
                NotaFiscalItem notaFiscalItem = new NotaFiscalItem();
                notaFiscalItem.Cfop = "0.000";

                // 8 – Exercício (Melhoria técnica)
                if (this.EstadoOrigem == "SP" || this.EstadoOrigem == "MG")
                {
                    switch (this.EstadoDestino)
                    {
                        case "RJ": notaFiscalItem.Cfop = "6.000"; break;
                        case "PE": notaFiscalItem.Cfop = "6.001"; break;
                        case "MG": notaFiscalItem.Cfop = "6.002"; break;
                        case "PB": notaFiscalItem.Cfop = "6.003"; break;
                        case "PR": notaFiscalItem.Cfop = "6.004"; break;
                        case "PI": notaFiscalItem.Cfop = "6.005"; break;
                        case "RO": notaFiscalItem.Cfop = "6.006"; break;
                        case "SE": notaFiscalItem.Cfop = "6.007"; break;
                        case "TO": notaFiscalItem.Cfop = "6.008"; break;
                        //case "SE": notaFiscalItem.Cfop = "6.009"; break; havia um SE em duplicidade aqui, que pelas condições das cadeias de if/else, nunca cairia
                        case "PA": notaFiscalItem.Cfop = "6.010"; break;
                    }                    
                }

                if (this.EstadoDestino == this.EstadoOrigem)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                }
                else
                {
                    notaFiscalItem.TipoIcms = "10";
                    notaFiscalItem.AliquotaIcms = 0.17;
                }

                notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido;
                notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms*notaFiscalItem.AliquotaIcms;

                if (itemPedido.Brinde)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                    notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;
                }

                notaFiscalItem.Desconto = 0.00;

                if (this.EstadoDestino == "SP")
                    notaFiscalItem.Desconto = 0.10;
                if (this.EstadoDestino == "RJ")
                    notaFiscalItem.Desconto = 0.10;
                if (this.EstadoDestino == "MG")
                    notaFiscalItem.Desconto = 0.10;
                if (this.EstadoDestino == "ES")
                    notaFiscalItem.Desconto = 0.10;

                /* 3 – Exercício (Novo recurso)
                Base de cálculo de IPI: Igual ao valor total do produto.
                Alíquota de IPI: Se for brinde alíquota é igual a 0% se não brinde alíquota é igual a 10%. 
                Valor de IPI: Base de cálculo * Alíquota de IPI.
                */
                notaFiscalItem.BaseIpi = itemPedido.ValorItemPedido;
                

                if (itemPedido.Brinde)
                    notaFiscalItem.AliquotaIpi = 0.10;
                else
                    notaFiscalItem.AliquotaIpi = 0.00;

                notaFiscalItem.ValorIpi = notaFiscalItem.BaseIpi * notaFiscalItem.AliquotaIpi;
                notaFiscalItem.NomeProduto = itemPedido.NomeProduto;
                notaFiscalItem.CodigoProduto = itemPedido.CodigoProduto;


                // inserção do item na estrutura da classe
                ItensDaNotaFiscal.Add(notaFiscalItem);
            }

            NotaFiscalRepository nfr = new NotaFiscalRepository();

            // estava fixo em 99999
            // alterado para um número sequencial
            this.NumeroNotaFiscal = nfr.GetUltimaNotaFiscal() + 1;

            // 1 – Exercício (Novo recurso)
            nomeXML = "XML/NF" + NumeroNotaFiscal.ToString() + ".xml";

            FileStream arquivo = new FileStream(nomeXML, FileMode.Create);
            XmlSerializer geraXML = new XmlSerializer(this.GetType());
            geraXML.Serialize(arquivo, this);
            arquivo.Close();                

            return nfr.GravaNotaFiscal(this);                
        }
    }
}
