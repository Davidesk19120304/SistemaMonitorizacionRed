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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConfiguracionAlertas));
            this.chkAlertasActivas = new System.Windows.Forms.CheckBox();
            this.lblSigma = new System.Windows.Forms.Label();
            this.numSigma = new System.Windows.Forms.NumericUpDown();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numSigma)).BeginInit();
            this.SuspendLayout();
            // 
            // chkAlertasActivas
            // 
            this.chkAlertasActivas.Checked = true;
            this.chkAlertasActivas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlertasActivas.Location = new System.Drawing.Point(20, 20);
            this.chkAlertasActivas.Name = "chkAlertasActivas";
            this.chkAlertasActivas.Size = new System.Drawing.Size(120, 24);
            this.chkAlertasActivas.TabIndex = 0;
            this.chkAlertasActivas.Text = "Alertas activas";
            // 
            // lblSigma
            // 
            this.lblSigma.Location = new System.Drawing.Point(20, 60);
            this.lblSigma.Name = "lblSigma";
            this.lblSigma.Size = new System.Drawing.Size(120, 23);
            this.lblSigma.TabIndex = 1;
            this.lblSigma.Text = "Sensibilidad (sigma):";
            // 
            // numSigma
            // 
            this.numSigma.DecimalPlaces = 1;
            this.numSigma.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSigma.Location = new System.Drawing.Point(150, 58);
            this.numSigma.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            65536});
            this.numSigma.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.numSigma.Name = "numSigma";
            this.numSigma.Size = new System.Drawing.Size(60, 20);
            this.numSigma.TabIndex = 2;
            this.numSigma.Value = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(60, 110);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(90, 30);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.BtnGuardar_Click);  // <--- ¡Faltaba esto!
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(170, 110);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(90, 30);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.BtnCancelar_Click); // <--- ¡Faltaba esto!
            // 
            // FrmConfiguracionAlertas
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(300, 170);
            this.Controls.Add(this.chkAlertasActivas);
            this.Controls.Add(this.lblSigma);
            this.Controls.Add(this.numSigma);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfiguracionAlertas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuración de Alertas";
            this.Load += new System.EventHandler(this.FrmConfiguracionAlertas_Load);
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

        /// <summary>
        /// Carga los valores actuales (provenientes de FrmMain) en los controles.
        /// </summary>
        public void CargarValores()
        {
            chkAlertasActivas.Checked = AlertasActivas;
            numSigma.Value = (decimal)FactorSigma;
        }

        private void FrmConfiguracionAlertas_Load(object sender, EventArgs e)
        {
            // No se necesita lógica adicional en este caso
        }
    }
}