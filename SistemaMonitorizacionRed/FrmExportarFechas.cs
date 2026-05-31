using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMonitorizacionRed
{
    public partial class FrmExportarFechas : Form
    {
        public DateTime FechaDesde { get; private set; }
        public DateTime FechaHasta { get; private set; }
        public bool Aceptado { get; private set; }

        public FrmExportarFechas(string tipoReporte)
        {
            this.Text = $"Exportar {tipoReporte} - Seleccionar Fechas";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ClientSize = new Size(360, 180);

            Label lblDesde = new Label
            {
                Text = "Fecha desde:",
                Location = new Point(20, 25),
                AutoSize = true
            };
            this.Controls.Add(lblDesde);

            DateTimePicker dtpDesde = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(140, 22),
                Width = 180,
                Value = DateTime.Now.AddDays(-7)
            };
            this.Controls.Add(dtpDesde);

            Label lblHasta = new Label
            {
                Text = "Fecha hasta:",
                Location = new Point(20, 65),
                AutoSize = true
            };
            this.Controls.Add(lblHasta);

            DateTimePicker dtpHasta = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(140, 62),
                Width = 180,
                Value = DateTime.Now
            };
            this.Controls.Add(dtpHasta);

            Button btnAceptar = new Button
            {
                Text = "Aceptar",
                Location = new Point(80, 110),
                Size = new Size(90, 30)
            };
            btnAceptar.Click += (sender, e) =>
            {
                FechaDesde = dtpDesde.Value.Date;
                FechaHasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);
                if (FechaDesde > FechaHasta)
                {
                    MessageBox.Show("La fecha 'desde' debe ser anterior o igual a la fecha 'hasta'.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Aceptado = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            this.Controls.Add(btnAceptar);

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(190, 110),
                Size = new Size(90, 30)
            };
            btnCancelar.Click += (sender, e) =>
            {
                Aceptado = false;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(btnCancelar);
        }
    }
}