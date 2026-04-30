using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMonitorizacionRed
{
    public partial class FrmConfiguracionAlertas : Form
    {
        // Propiedades públicas para transferir valores
        public bool AlertasActivas { get; set; }
        public double FactorSigma { get; set; }

        // Controles del formulario
        private CheckBox chkAlertasActivas;
        private Label lblSigma;
        private NumericUpDown numSigma;
        private Button btnGuardar;
        private Button btnCancelar;

        public FrmConfiguracionAlertas()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.chkAlertasActivas = new CheckBox();
            this.lblSigma = new Label();
            this.numSigma = new NumericUpDown();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.numSigma)).BeginInit();
            this.SuspendLayout();

            // chkAlertasActivas
            this.chkAlertasActivas.Text = "Alertas activas";
            this.chkAlertasActivas.Location = new Point(20, 20);
            this.chkAlertasActivas.Size = new Size(120, 24);
            this.chkAlertasActivas.Checked = true;

            // lblSigma
            this.lblSigma.Text = "Sensibilidad (sigma):";
            this.lblSigma.Location = new Point(20, 60);
            this.lblSigma.Size = new Size(120, 23);

            // numSigma
            this.numSigma.Location = new Point(150, 58);
            this.numSigma.Size = new Size(60, 23);
            this.numSigma.Minimum = 1.0M;
            this.numSigma.Maximum = 5.0M;
            this.numSigma.Increment = 0.1M;
            this.numSigma.DecimalPlaces = 1;
            this.numSigma.Value = 3.0M;

            // btnGuardar
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.FlatStyle = FlatStyle.Flat;
            this.btnGuardar.BackColor = Color.FromArgb(0, 102, 204);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(60, 110);
            this.btnGuardar.Size = new Size(90, 30);
            this.btnGuardar.Click += BtnGuardar_Click;

            // btnCancelar
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.BackColor = Color.FromArgb(108, 117, 125);
            this.btnCancelar.ForeColor = Color.White;
            this.btnCancelar.Location = new Point(170, 110);
            this.btnCancelar.Size = new Size(90, 30);
            this.btnCancelar.Click += BtnCancelar_Click;

            // FrmConfiguracionAlertas
            this.Text = "Configuración de Alertas";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(300, 170);
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.Controls.Add(this.chkAlertasActivas);
            this.Controls.Add(this.lblSigma);
            this.Controls.Add(this.numSigma);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            ((System.ComponentModel.ISupportInitialize)(this.numSigma)).EndInit();
            this.ResumeLayout(false);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            AlertasActivas = chkAlertasActivas.Checked;
            FactorSigma = (double)numSigma.Value;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Método para cargar los valores actuales desde FrmMain
        public void CargarValores()
        {
            chkAlertasActivas.Checked = AlertasActivas;
            numSigma.Value = (decimal)FactorSigma;
        }
    }
}