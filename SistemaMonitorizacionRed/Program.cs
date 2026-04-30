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

            // Mostrar pantalla de carga (se cierra automáticamente después de 3 segundos)
            using (FrmIntroduccion splash = new FrmIntroduccion())
            {
                splash.ShowDialog(); // Bloquea hasta que se cierre el splash
            }

            // Ahora mostrar el login
            using (FrmLogin login = new FrmLogin())
            {
                login.ShowDialog();

                if (login.LoginExitoso)
                {
                    // Iniciar la aplicación principal
                    Application.Run(new FrmMain(login.UsuarioActual, login.RolActual));
                }
                // Si el login falla, la aplicación termina
            }
        }
    }
}