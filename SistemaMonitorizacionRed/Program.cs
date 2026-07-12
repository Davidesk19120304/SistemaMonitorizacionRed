using System;
using System.Windows.Forms;

namespace SistemaMonitorizacionRed
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Pantalla de introducción (3 segundos)
            using (FrmIntroduccion splash = new FrmIntroduccion())
            {
                splash.ShowDialog();
            }

            // Mostrar login. Si el usuario inicia sesión, FrmLogin abre FrmMain internamente.
            Application.Run(new FrmLogin());
        }
    }
}