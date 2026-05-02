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
            this.lblMensaje = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMensaje
            // 
            this.lblMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMensaje.ForeColor = System.Drawing.Color.White;
            this.lblMensaje.Location = new System.Drawing.Point(0, 0);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Padding = new System.Windows.Forms.Padding(10);
            this.lblMensaje.Size = new System.Drawing.Size(350, 60);
            this.lblMensaje.TabIndex = 0;
            this.lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMensaje.Click += new System.EventHandler(this.lblMensaje_Click);
            // 
            // FrmToast
            // 
            this.ClientSize = new System.Drawing.Size(350, 60);
            this.Controls.Add(this.lblMensaje);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmToast";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
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

        private void lblMensaje_Click(object sender, EventArgs e)
        {

        }
    }
}