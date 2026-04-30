using System;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMonitorizacionRed
{
    public partial class FrmIntroduccion : Form
    {
        private Timer timer;

        public FrmIntroduccion()
        {
            // Configuración básica
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.ClientSize = new Size(600, 450);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Logo
            PictureBox pictureBoxLogo = new PictureBox();
            pictureBoxLogo.Image = null; // Asigna tu logo si lo tienes
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.Location = new Point(225, 30);
            pictureBoxLogo.Size = new Size(150, 100);
            pictureBoxLogo.BackColor = Color.Transparent;

            // Título
            Label lblTitulo = new Label();
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitulo.Text = "Sistema de Monitorización de Red";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Height = 60;
            lblTitulo.Padding = new Padding(0, 20, 0, 0);

            // Descripción
            Label lblDescripcion = new Label();
            lblDescripcion.Font = new Font("Segoe UI", 10F);
            lblDescripcion.ForeColor = Color.FromArgb(64, 64, 64);
            lblDescripcion.Text = "Bienvenido al sistema de análisis y monitorización de tráfico de red.\n\n" +
                                  "Este sistema permite capturar paquetes en tiempo real, detectar anomalías como escaneo de puertos, fuerza bruta, ICMP flood, picos de tráfico y escaneo vertical.\n\n" +
                                  "Además, genera alertas configurables, almacena los datos en MySQL y ofrece reportes exportables a CSV.\n\n" +
                                  "Utilice la interfaz para seleccionar la interfaz de red, iniciar captura y analizar el tráfico.\n\n" +
                                  "Cargando sistema... Por favor espere.";
            lblDescripcion.TextAlign = ContentAlignment.MiddleCenter;
            lblDescripcion.AutoSize = false;
            lblDescripcion.Location = new Point(50, 160);
            lblDescripcion.Size = new Size(500, 220);
            lblDescripcion.BackColor = Color.Transparent;

            // Agregar controles
            this.Controls.Add(lblDescripcion);
            this.Controls.Add(lblTitulo);
            this.Controls.Add(pictureBoxLogo);

            // Timer para cerrar la pantalla después de 3 segundos
            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += (s, e) => { timer.Stop(); this.Close(); };
            timer.Start();
        }
    }
}