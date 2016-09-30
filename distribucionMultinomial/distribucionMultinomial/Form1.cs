using System;
using System.Globalization;
using System.Windows.Forms;
using Deveel.Math;

namespace distribucionMultinomial
{
    public partial class Form1 : Form
    {
        CultureInfo cultInfo;

        public Form1()
        {
            InitializeComponent();

            this.CenterToScreen();

            cultInfo = new CultureInfo("es-AR");
            cultInfo.NumberFormat.NumberDecimalSeparator = ".";
            cultInfo.NumberFormat.NumberGroupSeparator = ",";
        }



        private void TablaDatos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int n=0;
            int k;
            float p;
            float prob = 0;

            foreach (DataGridViewRow row in this.TablaDatos.Rows)
            {
                if(int.TryParse((string)row.Cells[1].Value, out k))
                {
                    n += k;
                }
                if (float.TryParse((string)row.Cells[2].Value, out p))
                {
                    prob += p;
                }

                this.N.Text = n.ToString();
                this.SumP.Text = prob.ToString();
            }

        }

        private void TablaDatos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (this.TablaDatos.Rows[e.RowIndex].IsNewRow)
                return;


            switch(e.ColumnIndex)
            {
                case 1:
                    {
                        validarCeldaK(sender, e);
                        break;
                    }
                case 2:
                    {
                        validarCeldaP(sender, e);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return;


        }

        private void validarCeldaK(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string valor = (string)e.FormattedValue;

            long x;
            if (!long.TryParse(valor, out x))
            {
                this.TablaDatos.Rows[e.RowIndex].ErrorText= "K no es valido. Ingrese solo numeros naturales, por ejemplo: 1 - 56 - 987";
                e.Cancel = true;
            }
            else
            {
                this.TablaDatos.Rows[e.RowIndex].Cells[1].Value = x.ToString();
                this.TablaDatos.InvalidateCell(this.TablaDatos.Rows[e.RowIndex].Cells[1]);
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "";
                this.TablaDatos.RefreshEdit();
            }
        }

        private void validarCeldaP(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string valor = (string)e.FormattedValue;

            float x;

            if (valor.Contains("/"))
            {
                try
                {
                    x = Mate.FractionToDecimal(valor);
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es valido. Utilice ',' para decimales, '/' para fracciones, 'e' o 'E' para exponente. ej: 213,12  8/12 25e-2";
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                if (/*valor.Contains(".") ||*/ !float.TryParse(valor,NumberStyles.Any,cultInfo, out x))
                {
                    this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es valido. Utilice ',' para decimales, '/' para fracciones, 'e' o 'E' para exponente. ej: 213,12  8/12 25e-2";
                    e.Cancel = true;
                    return;
                }
            }
            

            if (x>1 || x < 0)
            {
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es valido. P debe ser un valor entre 0 y 1";
                e.Cancel = true;
            }
            else
            {
                this.TablaDatos.Rows[e.RowIndex].Cells[2].Value = x.ToString(cultInfo);
                this.TablaDatos.InvalidateCell(this.TablaDatos.Rows[e.RowIndex].Cells[2]);
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "";
                this.TablaDatos.RefreshEdit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(!validarModelo())
            {
                MessageBox.Show("La suma de las probabilidades debe ser igual a 1","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            long n;
            long.TryParse(this.N.Text, out n);

            BigDecimal res;
            res = BigDecimal.Parse(Mate.factorial(n).ToString());
            
            long k;
            float p;
            foreach(DataGridViewRow row in this.TablaDatos.Rows)
            {
                if (row.IsNewRow)
                    continue;

                long.TryParse((string)row.Cells[1].Value, out k);
                float.TryParse((string)row.Cells[2].Value, out p);



                res=res.Divide(Mate.factorial(k));
                res=res.Multiply(Math.Pow(p, k));

            }

            CultureInfo ci = new CultureInfo("es-AR");
            ci.NumberFormat.NumberDecimalDigits = 5;

            //this.Resultado.Text = res.ToDouble().ToString("e8");
            
            this.Resultado.Text = res.ToDouble().ToString("G10");
            MessageBox.Show(String.Format(cultInfo, "{0:G10}", res.ToDouble()));

        }

        private bool validarModelo()
        {
            float prob = 0;
            float p;

            foreach (DataGridViewRow row in this.TablaDatos.Rows)
            {
                if (row.IsNewRow)
                    continue;

                float.TryParse((string)row.Cells[2].Value, out p);

                prob += p;

            }

            return prob == 1;

        }


        private void TablaDatos_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            //e.Row.Cells[0].Value = (e.Row.Index+1).ToString();
            e.Row.Cells[1].Value = "0";
            e.Row.Cells[2].Value = "0,0";
        }



        private bool ConfirmarCierre()
        {
            DialogResult r = MessageBox.Show("Esta seguro que desea salir?", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return r == DialogResult.Yes;
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmarCierre())
                e.Cancel = true;
            else
            {
                this.Dispose();
            }
        }
    }
}
