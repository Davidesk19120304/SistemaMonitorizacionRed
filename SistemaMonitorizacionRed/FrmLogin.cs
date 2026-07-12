using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SistemaMonitorizacionRed
{
    public partial class FrmLogin : Form
    {
        private Label lblTitulo;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnIngresar;
        private Button btnCancelar;
        private Panel panelPIN;
        private Label lblMensajePIN;
        private TextBox txtPIN;
        private Button btnValidarPIN;
        private Panel panelLogin;

        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";
        private string usuarioLogeado;
        private bool esNuevoPIN = false;

        public FrmLogin()
        {
            InitializeComponent();
            this.btnIngresar.Click += BtnIngresar_Click;
            this.btnCancelar.Click += BtnCancelar_Click;
            this.btnValidarPIN.Click += BtnValidarPIN_Click;
            this.AcceptButton = btnIngresar;
            this.CancelButton = btnCancelar;
            InicializarBaseDeDatos();
        }

        private void InitializeComponent()
        {
            this.lblTitulo = new Label();
            this.lblUsuario = new Label();
            this.txtUsuario = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnIngresar = new Button();
            this.btnCancelar = new Button();
            this.panelLogin = new Panel();
            this.panelPIN = new Panel();
            this.lblMensajePIN = new Label();
            this.txtPIN = new TextBox();
            this.btnValidarPIN = new Button();
            this.panelLogin.SuspendLayout();
            this.panelPIN.SuspendLayout();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.lblTitulo.Dock = DockStyle.Top;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(380, 50);
            this.lblTitulo.Text = "Iniciar Sesión";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // panelLogin
            this.panelLogin.Controls.Add(this.lblUsuario);
            this.panelLogin.Controls.Add(this.txtUsuario);
            this.panelLogin.Controls.Add(this.lblPassword);
            this.panelLogin.Controls.Add(this.txtPassword);
            this.panelLogin.Controls.Add(this.btnIngresar);
            this.panelLogin.Controls.Add(this.btnCancelar);
            this.panelLogin.Location = new System.Drawing.Point(10, 60);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(360, 200);

            // lblUsuario
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblUsuario.Location = new System.Drawing.Point(20, 20);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(80, 25);
            this.lblUsuario.Text = "Usuario:";

            // txtUsuario
            this.txtUsuario.Location = new System.Drawing.Point(110, 20);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(200, 20);

            // lblPassword
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblPassword.Location = new System.Drawing.Point(20, 60);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(80, 25);
            this.lblPassword.Text = "Contraseña:";

            // txtPassword
            this.txtPassword.Location = new System.Drawing.Point(110, 60);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.UseSystemPasswordChar = true;

            // btnIngresar
            this.btnIngresar.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnIngresar.FlatStyle = FlatStyle.Flat;
            this.btnIngresar.ForeColor = System.Drawing.Color.White;
            this.btnIngresar.Location = new System.Drawing.Point(60, 120);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(100, 35);
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.UseVisualStyleBackColor = false;

            // btnCancelar
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.btnCancelar.FlatStyle = FlatStyle.Flat;
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(180, 120);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 35);
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;

            // panelPIN
            this.panelPIN.Controls.Add(this.lblMensajePIN);
            this.panelPIN.Controls.Add(this.txtPIN);
            this.panelPIN.Controls.Add(this.btnValidarPIN);
            this.panelPIN.Location = new System.Drawing.Point(10, 60);
            this.panelPIN.Name = "panelPIN";
            this.panelPIN.Size = new System.Drawing.Size(360, 200);
            this.panelPIN.Visible = false;

            // lblMensajePIN
            this.lblMensajePIN.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblMensajePIN.Location = new System.Drawing.Point(20, 20);
            this.lblMensajePIN.Name = "lblMensajePIN";
            this.lblMensajePIN.Size = new System.Drawing.Size(320, 40);
            this.lblMensajePIN.Text = "Ingrese su PIN de seguridad:";

            // txtPIN
            this.txtPIN.Location = new System.Drawing.Point(110, 70);
            this.txtPIN.Name = "txtPIN";
            this.txtPIN.Size = new System.Drawing.Size(100, 20);
            this.txtPIN.MaxLength = 4;
            this.txtPIN.UseSystemPasswordChar = true;

            // btnValidarPIN
            this.btnValidarPIN.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnValidarPIN.FlatStyle = FlatStyle.Flat;
            this.btnValidarPIN.ForeColor = System.Drawing.Color.White;
            this.btnValidarPIN.Location = new System.Drawing.Point(110, 120);
            this.btnValidarPIN.Name = "btnValidarPIN";
            this.btnValidarPIN.Size = new System.Drawing.Size(120, 35);
            this.btnValidarPIN.Text = "Validar PIN";
            this.btnValidarPIN.UseVisualStyleBackColor = false;

            // FrmLogin
            this.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
            this.ClientSize = new System.Drawing.Size(380, 280);
            this.Controls.Add(this.panelLogin);
            this.Controls.Add(this.panelPIN);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Inicio de Sesión";

            this.panelLogin.ResumeLayout(false);
            this.panelPIN.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void InicializarBaseDeDatos()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string createTable = @"
                CREATE TABLE IF NOT EXISTS usuarios (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    usuario VARCHAR(50) UNIQUE NOT NULL,
                    contraseña VARCHAR(64) NOT NULL,
                    pin VARCHAR(64) NULL,
                    rol VARCHAR(20) DEFAULT 'admin'
                )";
                    using (MySqlCommand cmd = new MySqlCommand(createTable, conn))
                        cmd.ExecuteNonQuery();

                    string checkAdmin = "SELECT COUNT(*) FROM usuarios";
                    using (MySqlCommand cmd = new MySqlCommand(checkAdmin, conn))
                    {
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            string insertAdmin = "INSERT INTO usuarios (usuario, contraseña, rol) VALUES ('admin', @hash, 'admin')";
                            using (MySqlCommand cmdInsert = new MySqlCommand(insertAdmin, conn))
                            {
                                cmdInsert.Parameters.AddWithValue("@hash", SHA256("admin"));
                                cmdInsert.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo inicializar la base de datos: {ex.Message}",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string entrada = txtUsuario.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrEmpty(entrada) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Ingrese usuario y contraseña.");
                return;
            }

            string hash = SHA256(pass);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT usuario, pin FROM usuarios 
                                 WHERE usuario = @entrada 
                                 AND contraseña = @hash";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@entrada", entrada);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuarioLogeado = reader["usuario"].ToString();
                            string pinHash = reader["pin"]?.ToString();

                            if (string.IsNullOrEmpty(pinHash))
                            {
                                esNuevoPIN = true;
                                panelLogin.Visible = false;
                                panelPIN.Visible = true;
                                this.AcceptButton = btnValidarPIN;
                                lblMensajePIN.Text = "No tiene un PIN de seguridad. Ingrese un PIN de 4 dígitos y presione Validar PIN.";
                            }
                            else
                            {
                                esNuevoPIN = false;
                                panelLogin.Visible = false;
                                panelPIN.Visible = true;
                                this.AcceptButton = btnValidarPIN;
                                lblMensajePIN.Text = "Ingrese su PIN de seguridad:";
                            }
                            txtPIN.Text = "";
                            txtPIN.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Credenciales incorrectas.");
                        }
                    }
                }
            }
        }
        private void BtnValidarPIN_Click(object sender, EventArgs e)
        {
            string pin = txtPIN.Text.Trim();
            if (pin.Length != 4 || !int.TryParse(pin, out _))
            {
                MessageBox.Show("El PIN debe ser exactamente 4 dígitos numéricos.");
                return;
            }

            string hash = SHA256(pin);

            if (esNuevoPIN)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE usuarios SET pin = @hash WHERE usuario = @usr";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hash);
                        cmd.Parameters.AddWithValue("@usr", usuarioLogeado);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("PIN configurado correctamente.");
                AbrirSistema(usuarioLogeado);
            }
            else
            {
                string hashGuardado;
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT pin FROM usuarios WHERE usuario = @usr";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usr", usuarioLogeado);
                        hashGuardado = cmd.ExecuteScalar()?.ToString();
                    }
                }

                if (hash == hashGuardado)
                {
                    AbrirSistema(usuarioLogeado);
                }
                else
                {
                    MessageBox.Show("PIN incorrecto.");
                    txtPIN.Clear();
                    txtPIN.Focus();
                }
            }
        }
        private void AbrirSistema(string usuario)
        {
            this.Hide();
            FrmMain frm = new FrmMain(usuario, "admin");
            frm.ShowDialog();
            this.Close();
        }
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.AcceptButton = btnIngresar;
            this.Close();
        }
        private string SHA256(string texto)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}