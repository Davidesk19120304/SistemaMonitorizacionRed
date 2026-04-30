using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SistemaMonitorizacionRed
{
    public partial class FrmToast : Form
    {
        // Administrador estático de toasts activos
        private static List<FrmToast> toastsActivos = new List<FrmToast>();
        private static readonly object lockObj = new object();

        private Timer timer;
        private Label lblMensaje;
        private int duracionMs;

        public FrmToast(string mensaje, Color colorFondo, int duracionMs = 3000)
        {
            InitializeComponent();
            this.BackColor = colorFondo;
            lblMensaje.Text = mensaje;
            this.duracionMs = duracionMs;

            // Registrar este toast en la lista y calcular posición
            lock (lockObj)
            {
                toastsActivos.Add(this);
                ReposicionarToasts();
            }

            // Configurar timer para auto-cierre
            timer = new Timer();
            timer.Interval = duracionMs;
            timer.Tick += (s, e) => this.Close();
            timer.Start();
        }

        private void InitializeComponent()
        {
            this.lblMensaje = new Label();
            this.SuspendLayout();

            // lblMensaje
            this.lblMensaje.AutoSize = false;
            this.lblMensaje.Dock = DockStyle.Fill;
            this.lblMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblMensaje.ForeColor = Color.White;
            this.lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
            this.lblMensaje.Padding = new Padding(10);

            // FrmToast
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Size = new Size(350, 60);
            this.Controls.Add(this.lblMensaje);
            this.ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // La posición se calculará al registrarse (ya se hizo en el constructor)
            // Reforzar que esté por encima
            this.TopMost = true;
        }

        /// <summary>
        /// Reposiciona todos los toasts activos en la esquina superior derecha,
        /// apilándolos verticalmente (escalera hacia abajo).
        /// </summary>
        private static void ReposicionarToasts()
        {
            int offsetY = 20; // margen superior inicial
            int rightMargin = 20; // margen derecho
            Screen sc = Screen.PrimaryScreen;
            int baseX = sc.WorkingArea.Right - rightMargin;

            lock (lockObj)
            {
                // Ordenar por orden de creación (el más antiguo primero)
                // Usamos el índice en la lista (orden de inserción)
                for (int i = 0; i < toastsActivos.Count; i++)
                {
                    var toast = toastsActivos[i];
                    if (toast == null || toast.IsDisposed) continue;

                    int y = offsetY + i * (toast.Height + 5); // +5 de separación
                    toast.Location = new Point(baseX - toast.Width, y);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Al cerrarse, eliminar de la lista y reubicar los restantes
            lock (lockObj)
            {
                toastsActivos.Remove(this);
                ReposicionarToasts();
            }
            timer?.Dispose();
        }

        // Método estático para mostrar notificación (comodidad)
        public static void Mostrar(string mensaje, Color colorFondo, int duracionMs = 3000)
        {
            // Asegurar ejecución en el hilo de la UI
            if (Application.OpenForms.Count > 0)
            {
                var form = Application.OpenForms[0];
                if (form.InvokeRequired)
                {
                    form.Invoke(new Action(() => new FrmToast(mensaje, colorFondo, duracionMs).Show()));
                }
                else
                {
                    new FrmToast(mensaje, colorFondo, duracionMs).Show();
                }
            }
            else
            {
                new FrmToast(mensaje, colorFondo, duracionMs).Show();
            }
        }
    }
}