using Imposto.Core.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imposto.Core.Domain;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        private Pedido pedido;

        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;                       
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();

            //  6 – Exercício (Correção de BUG)
            cbEstadoDestino.SelectedIndex = 24;
            cbEstadoOrigem.SelectedIndex = 24;
        }

        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }   
        }

        private object GetTablePedidos()
        {
            DataTable table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));            
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));            

            return table;
        }

        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            pedido = new Pedido();

            NotaFiscalService service = new NotaFiscalService();
            pedido.EstadoOrigem = cbEstadoOrigem.Text.Substring(0, 2);
            pedido.EstadoDestino = cbEstadoDestino.Text.Substring(0, 2);
            pedido.NomeCliente = textBoxNomeCliente.Text;

            DataTable table = (DataTable)dataGridViewPedidos.DataSource;

            if (table.Rows.Count < 1)
            {
                MessageBox.Show("Informe pelo menos 1 produto");
                return;
            }

            if (textBoxNomeCliente.Text.Trim().Length < 1)
            {
                MessageBox.Show("Informe o nome do cliente");
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                pedido.ItensDoPedido.Add(
                    new PedidoItem()
                    {
                        Brinde = DBNull.Value.Equals(row["Brinde"]) ? false : Convert.ToBoolean(row["Brinde"]),
                        CodigoProduto = row["Codigo do produto"].ToString(),
                        NomeProduto = row["Nome do produto"].ToString(),
                        ValorItemPedido = Convert.ToDouble(row["Valor"].ToString())
                    });
            }

            if (service.GerarNotaFiscal(pedido))
            { 
                MessageBox.Show("Operação efetuada com sucesso");
                cbEstadoDestino.SelectedIndex = 24;
                cbEstadoOrigem.SelectedIndex = 24;
                dataGridViewPedidos.DataSource = GetTablePedidos();
                textBoxNomeCliente.Text = "";
            }
            else
                MessageBox.Show("Erro na geração da Nota Fiscal");
        }

        //  6 – Exercício (Correção de BUG)
        private void dataGridViewPedidos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Formato Incorreto");
        }
    }
}
