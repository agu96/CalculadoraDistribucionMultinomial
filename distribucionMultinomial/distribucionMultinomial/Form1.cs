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
            ActNyP();
        }

        private void ActNyP()
        {
            long n = 0;
            long k;
            double p;
            double prob = 0;

            foreach (DataGridViewRow row in this.TablaDatos.Rows)
            {
                if (long.TryParse((string)row.Cells[1].Value, NumberStyles.Any, cultInfo, out k))
                {
                    n += k;
                }
                if (double.TryParse((string)row.Cells[2].Value, NumberStyles.Any, cultInfo, out p))
                {
                    prob += p;
                }

                this.N.Text = n.ToString("G10", cultInfo);
                this.SumP.Text = prob.ToString("G10", cultInfo);
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
            if (!long.TryParse(valor,NumberStyles.Any,cultInfo, out x))
            {
                this.TablaDatos.Rows[e.RowIndex].ErrorText= "K no es válido. Ingrese un número naturales, por ejemplo: 1 - 56 - 987";
                e.Cancel = true;
            }
            else if(x < 0)
            {
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "K no es válido. K no puede ser negativo";
                e.Cancel = true;
            }
            else
            {
                this.TablaDatos.Rows[e.RowIndex].Cells[1].Value = x.ToString("G10", cultInfo);
                this.TablaDatos.InvalidateCell(this.TablaDatos.Rows[e.RowIndex].Cells[1]);
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "";
                this.TablaDatos.RefreshEdit();
            }
        }

        private void validarCeldaP(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string valor = (string)e.FormattedValue;

            double x;

            if (valor.Contains("/"))
            {
                try
                {
                    x = Mate.FractionToDecimal(valor);
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es válido. Utilice '.' para decimales, '/' para fracciones, 'e' o 'E' para exponente. ej: 213,12  8/12 25e-2";
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                if (!double.TryParse(valor,NumberStyles.Any,cultInfo, out x))
                {
                    this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es válido. Utilice '.' para decimales, '/' para fracciones, 'e' o 'E' para exponente. ej: 0.12  8/12 25e-2";
                    e.Cancel = true;
                    return;
                }
            }
            

            if (x>1 || x < 0)
            {
                this.TablaDatos.Rows[e.RowIndex].ErrorText = "P no es válido. P debe ser un valor entre 0 y 1";
                e.Cancel = true;
            }
            else
            {
                this.TablaDatos.Rows[e.RowIndex].Cells[2].Value = x.ToString("G10", cultInfo);
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
            long.TryParse(this.N.Text,NumberStyles.Any,cultInfo, out n);

            BigDecimal res;
            res = BigDecimal.Parse(Mate.factorial(n).ToString());
            
            long k;
            double p;
            foreach(DataGridViewRow row in this.TablaDatos.Rows)
            {
                if (row.IsNewRow)
                    continue;

                long.TryParse((string)row.Cells[1].Value, NumberStyles.Any, cultInfo, out k);
                double.TryParse((string)row.Cells[2].Value, NumberStyles.Any, cultInfo, out p);



                res=res.Divide(Mate.factorial(k));
                res=res.Multiply(Math.Pow(p, k));

            }
            
            this.Resultado.Text = res.ToDouble().ToString("G10",cultInfo);

        }

        private bool validarModelo()
        {
            double prob = 0;
            double p;

            foreach (DataGridViewRow row in this.TablaDatos.Rows)
            {
                if (row.IsNewRow)
                    continue;

                double.TryParse((string)row.Cells[2].Value, NumberStyles.Any, cultInfo, out p);

                prob += p;

            }

            return prob == 1D;

        }


        private void TablaDatos_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[1].Value = "0";
            e.Row.Cells[2].Value = "0.0";
        }



        private bool ConfirmarCierre()
        {
            DialogResult r = MessageBox.Show("¿Está seguro que desea salir?", "Confirmar Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

        private void TablaDatos_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ActNyP();
        }
    }
}
