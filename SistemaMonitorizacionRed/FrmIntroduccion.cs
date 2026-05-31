using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SistemaMonitorizacionRed
{
    public partial class FrmIntroduccion : Form
    {
        private Timer timer;
        private PictureBox pictureBoxLogo;

        public FrmIntroduccion()
        {
            // Configuración básica del formulario
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.ClientSize = new Size(600, 450);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmIntroduccion";
            this.Text = "Sistema de Monitorización de Red";

            // ==========================================
            // LOGO
            // ==========================================
            pictureBoxLogo = new PictureBox();
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLogo.Width = 150;
            pictureBoxLogo.Height = 100;
            pictureBoxLogo.Left = (this.ClientSize.Width - pictureBoxLogo.Width) / 2;
            pictureBoxLogo.Top = 50;
            pictureBoxLogo.BackColor = Color.Transparent;

            // Cargar logo desde recursos
            CargarLogoClaro();

            // ==========================================
            // TÍTULO
            // ==========================================
            Label lblTitulo = new Label();
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitulo.Text = "Sistema de Monitorización de Red";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Height = 60;
            lblTitulo.Padding = new Padding(0, 20, 0, 0);

            // ==========================================
            // DESCRIPCIÓN
            // ==========================================
            Label lblDescripcion = new Label();
            lblDescripcion.Font = new Font("Segoe UI", 10F);
            lblDescripcion.ForeColor = Color.FromArgb(64, 64, 64);
            lblDescripcion.Text = "Bienvenido al sistema de análisis y monitorización de tráfico de red.\n\n" +
                                  "Este sistema permite capturar paquetes en tiempo real, detectar anomalías como escaneo de puertos, fuerza bruta, ICMP flood, picos de tráfico y escaneo vertical.\n\n" +
                                  "Además, genera alertas configurables, almacena los datos en MySQL y ofrece reportes exportables a Excel, PDF y CSV.\n\n" +
                                  "Utilice la interfaz para seleccionar la interfaz de red, iniciar captura y analizar el tráfico.\n\n" +
                                  "Cargando sistema... Por favor espere.";
            lblDescripcion.TextAlign = ContentAlignment.MiddleCenter;
            lblDescripcion.AutoSize = false;
            lblDescripcion.Left = (this.ClientSize.Width - 500) / 2;
            lblDescripcion.Top = 160;
            lblDescripcion.Width = 500;
            lblDescripcion.Height = 220;
            lblDescripcion.BackColor = Color.Transparent;

            // ==========================================
            // AGREGAR CONTROLES AL FORMULARIO
            // ==========================================
            this.Controls.Add(pictureBoxLogo);
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblDescripcion);

            // ==========================================
            // TIMER PARA CERRAR DESPUÉS DE 3 SEGUNDOS
            // ==========================================
            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Close();
            };
            timer.Start();
        }

        /// <summary>
        /// Carga la imagen del logo desde los recursos del proyecto.
        /// </summary>
        private void CargarLogoClaro()
        {
            try
            {
                // Intentar cargar desde los recursos del proyecto
                pictureBoxLogo.Image = Properties.Resources.LOGO_PEQUENO;
            }
            catch
            {
                // Si falla, buscar en la carpeta Resources como alternativa
                string rutaLogo = Path.Combine(Application.StartupPath, "Resources", "LOGO.png");
                if (File.Exists(rutaLogo))
                    pictureBoxLogo.Image = Image.FromFile(rutaLogo);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmIntroduccion));
            this.SuspendLayout();
            // 
            // FrmIntroduccion
            // 
            this.ClientSize = new System.Drawing.Size(600, 450);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmIntroduccion";
            this.Load += new System.EventHandler(this.FrmIntroduccion_Load);
            this.ResumeLayout(false);

        }

        private void FrmIntroduccion_Load(object sender, EventArgs e)
        {
            // No se necesita lógica adicional
        }
    }
}