using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SistemaMonitorizacionRed
{
    public partial class HistorialAlertas : Form
    {
        // Cadena de conexión a MySQL (igual que en Form1)
        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";

        public HistorialAlertas()
        {
            InitializeComponent();

            // Configurar combo box de severidad
            cmbFiltroSeveridad.Items.Clear();
            cmbFiltroSeveridad.Items.Add("Todas");
            cmbFiltroSeveridad.Items.Add("Baja");
            cmbFiltroSeveridad.Items.Add("Media");
            cmbFiltroSeveridad.Items.Add("Alta");
            cmbFiltroSeveridad.Items.Add("Crítica");
            cmbFiltroSeveridad.SelectedIndex = 0; // "Todas" por defecto

            // Configurar DataGridView
            ConfigurarDataGridView();
        }

        private void ConfigurarDataGridView()
        {
            dgvAlertas.ReadOnly = true;
            dgvAlertas.AllowUserToAddRows = false;
            dgvAlertas.AllowUserToDeleteRows = false;
            dgvAlertas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAlertas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlertas.MultiSelect = false;
        }

        private void HistorialAlertas_Load(object sender, EventArgs e)
        {
            CargarAlertas();
        }

        private void CargarAlertas()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Construir consulta SQL con filtros
                    string query = "SELECT id, tipo, descripcion, severidad, ip_involucrada, timestamp FROM alertas WHERE 1=1";

                    // Filtro por severidad
                    if (cmbFiltroSeveridad.SelectedIndex > 0) // No es "Todas"
                    {
                        string severidad = cmbFiltroSeveridad.SelectedItem.ToString();
                        query += $" AND severidad = '{severidad}'";
                    }

                    // Filtro por fecha (si está activo)
                    if (chkFiltrarFecha.Checked)
                    {
                        string fecha = dtpFecha.Value.ToString("yyyy-MM-dd");
                        query += $" AND DATE(timestamp) = '{fecha}'";
                    }

                    query += " ORDER BY timestamp DESC";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAlertas.DataSource = dt;

                    // Configurar títulos de columnas amigables
                    if (dgvAlertas.Columns.Count > 0)
                    {
                        dgvAlertas.Columns["id"].HeaderText = "ID";
                        dgvAlertas.Columns["tipo"].HeaderText = "Tipo";
                        dgvAlertas.Columns["descripcion"].HeaderText = "Descripción";
                        dgvAlertas.Columns["severidad"].HeaderText = "Severidad";
                        dgvAlertas.Columns["ip_involucrada"].HeaderText = "IP Involucrada";
                        dgvAlertas.Columns["timestamp"].HeaderText = "Fecha y Hora";

                        // Ajustar anchos
                        dgvAlertas.Columns["id"].Width = 50;
                        dgvAlertas.Columns["tipo"].Width = 100;
                        dgvAlertas.Columns["descripcion"].Width = 300;
                        dgvAlertas.Columns["severidad"].Width = 80;
                        dgvAlertas.Columns["ip_involucrada"].Width = 120;
                        dgvAlertas.Columns["timestamp"].Width = 130;

                        // Colorear filas según severidad
                        dgvAlertas.CellFormatting += DgvAlertas_CellFormatting;
                    }

                    lblTotal.Text = $"Total: {dt.Rows.Count} alertas";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alertas: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvAlertas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Colorear filas según severidad
            if (e.RowIndex >= 0 && dgvAlertas.Rows[e.RowIndex].Cells["severidad"].Value != null)
            {
                string severidad = dgvAlertas.Rows[e.RowIndex].Cells["severidad"].Value.ToString();

                switch (severidad)
                {
                    case "Crítica":
                        dgvAlertas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkRed;
                        dgvAlertas.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                        break;
                    case "Alta":
                        dgvAlertas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
                        break;
                    case "Media":
                        dgvAlertas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        break;
                    case "Baja":
                        dgvAlertas.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                        break;
                }
            }
        }

        private void chkFiltrarFecha_CheckedChanged(object sender, EventArgs e)
        {
            // Habilitar/deshabilitar el DateTimePicker según el CheckBox
            dtpFecha.Enabled = chkFiltrarFecha.Checked;
            lblFecha.Visible = chkFiltrarFecha.Checked;

            // Si se desactiva, recargar sin filtro de fecha
            if (!chkFiltrarFecha.Checked)
            {
                CargarAlertas();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarAlertas();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvAlertas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Al hacer doble clic en una alerta, mostrar detalles completos
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAlertas.Rows[e.RowIndex];
                string detalles = $"ID: {row.Cells["id"].Value}\n" +
                                 $"Tipo: {row.Cells["tipo"].Value}\n" +
                                 $"Severidad: {row.Cells["severidad"].Value}\n" +
                                 $"IP: {row.Cells["ip_involucrada"].Value}\n" +
                                 $"Fecha: {row.Cells["timestamp"].Value}\n\n" +
                                 $"Descripción:\n{row.Cells["descripcion"].Value}";

                MessageBox.Show(detalles, "Detalles de la Alerta",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}