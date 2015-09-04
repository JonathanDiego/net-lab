using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Imposto.Core.Domain;

// 2 – Exercício (Novo recurso)

namespace Imposto.Core.Data
{
    public class NotaFiscalRepository
    {
        private string strconexao;
        private SqlConnection conexao;
        
        public NotaFiscalRepository()
        {
            strconexao = "server = .\\sqlexpress;integrated security = true;Initial Catalog=teste";
            conexao = new SqlConnection(strconexao);
            conexao.Open();            
        }

        ~NotaFiscalRepository()
        {
            //conexao.Close();
        }

        public DataSet Executar(String comando)
        {
            SqlCommand cmd = new SqlCommand(comando, conexao);
            //cmd.ExecuteNonQuery();            
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(ds);
            
            return ds;
        }

        public int GetUltimaNotaFiscal()
        {
            DataSet ds = Executar("select MAX([NumeroNotaFiscal]) as NumeroNotaFiscal from NotaFiscal");

            return Convert.ToInt32(ds.Tables[0].Rows[0]["NumeroNotaFiscal"].ToString());           
        }

        public bool GravaNotaFiscal(NotaFiscal nf)
        {
            string comando = "";
            comando = " declare @id int" +
                      " set @id = 0" +
                      " exec P_NOTA_FISCAL" +
                      " @pId=@id OUTPUT," +
                      " @pNumeroNotaFiscal = " + nf.NumeroNotaFiscal.ToString() + "," +
                      " @pSerie = " + nf.Serie.ToString() + "," +
                      " @pNomeCliente = \"" + nf.NomeCliente.ToString() + "\"," +
                      " @pEstadoDestino = \"" + nf.EstadoDestino.ToString() + "\"," +
                      " @pEstadoOrigem = \"" + nf.EstadoOrigem.ToString() + "\"";


            foreach (NotaFiscalItem item in nf.ItensDaNotaFiscal)
            {
                comando +=" exec P_NOTA_FISCAL_ITEM " +
                          " @pId = 0," +
                          " @pIdNotaFiscal = @id," +
                          " @pCfop = " + item.Cfop.ToString().Replace(',','.') + "," +
                          " @pTipoIcms = " + item.TipoIcms.ToString().Replace(',', '.') + "," +
                          " @pBaseIcms = " + item.BaseIcms.ToString().Replace(',', '.') + "," +
                          " @pAliquotaIcms = " + item.AliquotaIcms.ToString().Replace(',', '.') + "," +
                          " @pValorIcms = " + item.ValorIcms.ToString().Replace(',', '.') + "," +
                          " @pNomeProduto = " + ("\"" + item.NomeProduto.ToString()) + "\"," +
                          " @pCodigoProduto = " + ("\"" + item.CodigoProduto.ToString()) + "\"," +
                          " @pBaseIpi = " + item.BaseIpi.ToString().Replace(',', '.') + "," +
                          " @pAliquotaIpi = " + item.AliquotaIpi.ToString().Replace(',', '.') + "," +
                          " @pValorIpi = " + item.ValorIpi.ToString().Replace(',', '.') + "," +
                          " @pDesconto = " + item.Desconto.ToString().Replace(',', '.');
            }

            Executar(comando);

            return true;
        }
        
    }
}
