using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SistemaMonitorizacionRed
{
    public partial class HistorialAlertas : Form
    {
        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";

        public HistorialAlertas()
        {
            InitializeComponent();
            // Configurar DataGridView
            dgvAlertas.AutoGenerateColumns = false;
            dgvAlertas.ReadOnly = true;
            dgvAlertas.AllowUserToAddRows = false;
            dgvAlertas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAlertas.MultiSelect = false;
            dgvAlertas.RowHeadersVisible = false;
            dgvAlertas.BackgroundColor = Color.White;
            dgvAlertas.BorderStyle = BorderStyle.Fixed3D;
            dgvAlertas.GridColor = Color.LightGray;
            dgvAlertas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgvAlertas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAlertas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvAlertas.EnableHeadersVisualStyles = false;
            dgvAlertas.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;

            // Configurar columnas
            ConfigurarColumnas();

            // Añadir evento para colorear filas según severidad
            dgvAlertas.RowPrePaint += DgvAlertas_RowPrePaint;

            // Cargar datos al iniciar
            CargarAlertas();
        }

        private void ConfigurarColumnas()
        {
            dgvAlertas.Columns.Clear();

            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
            colId.Name = "id";
            colId.HeaderText = "ID";
            colId.Width = 50;
            colId.DataPropertyName = "id";

            DataGridViewTextBoxColumn colTipo = new DataGridViewTextBoxColumn();
            colTipo.Name = "tipo";
            colTipo.HeaderText = "Tipo";
            colTipo.Width = 120;
            colTipo.DataPropertyName = "tipo";

            DataGridViewTextBoxColumn colDescripcion = new DataGridViewTextBoxColumn();
            colDescripcion.Name = "descripcion";
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.Width = 400;
            colDescripcion.DataPropertyName = "descripcion";

            DataGridViewTextBoxColumn colSeveridad = new DataGridViewTextBoxColumn();
            colSeveridad.Name = "severidad";
            colSeveridad.HeaderText = "Severidad";
            colSeveridad.Width = 100;
            colSeveridad.DataPropertyName = "severidad";

            DataGridViewTextBoxColumn colIP = new DataGridViewTextBoxColumn();
            colIP.Name = "ip_involucrada";
            colIP.HeaderText = "IP Involucrada";
            colIP.Width = 120;
            colIP.DataPropertyName = "ip_involucrada";

            DataGridViewTextBoxColumn colTimestamp = new DataGridViewTextBoxColumn();
            colTimestamp.Name = "timestamp";
            colTimestamp.HeaderText = "Fecha y Hora";
            colTimestamp.Width = 150;
            colTimestamp.DataPropertyName = "timestamp";

            dgvAlertas.Columns.AddRange(new DataGridViewColumn[] { colId, colTipo, colDescripcion, colSeveridad, colIP, colTimestamp });
        }

        private void HistorialAlertas_Load(object sender, EventArgs e)
        {
            // Configurar ComboBox de severidad
            cmbFiltroSeveridad.Items.Clear();
            cmbFiltroSeveridad.Items.Add("Todas");
            cmbFiltroSeveridad.Items.Add("Baja");
            cmbFiltroSeveridad.Items.Add("Media");
            cmbFiltroSeveridad.Items.Add("Alta");
            cmbFiltroSeveridad.Items.Add("Crítica");
            cmbFiltroSeveridad.SelectedIndex = 0;

            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Value = DateTime.Today;
        }

        private void CargarAlertas()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, tipo, descripcion, severidad, ip_involucrada, timestamp FROM alertas WHERE 1=1";

                    if (cmbFiltroSeveridad.SelectedIndex > 0)
                    {
                        string severidad = cmbFiltroSeveridad.SelectedItem.ToString();
                        query += $" AND severidad = '{severidad}'";
                    }

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
                    lblTotal.Text = $"Total: {dt.Rows.Count} alertas";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alertas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento para colorear filas según severidad
        private void DgvAlertas_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow row = dgvAlertas.Rows[e.RowIndex];
            if (row.Cells["severidad"].Value == null) return;

            string severidad = row.Cells["severidad"].Value.ToString();
            switch (severidad)
            {
                case "Baja":
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                case "Media":
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                case "Alta":
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
                case "Crítica":
                    row.DefaultCellStyle.BackColor = Color.DarkRed;
                    row.DefaultCellStyle.ForeColor = Color.White;
                    break;
                default:
                    // Restaurar colores por defecto (alternados)
                    if (e.RowIndex % 2 == 0)
                        row.DefaultCellStyle.BackColor = Color.White;
                    else
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
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

        private void chkFiltrarFecha_CheckedChanged(object sender, EventArgs e)
        {
            dtpFecha.Enabled = chkFiltrarFecha.Checked;
            lblFecha.Visible = chkFiltrarFecha.Checked;
            if (!chkFiltrarFecha.Checked)
            {
                CargarAlertas();
            }
        }

        private void dgvAlertas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAlertas.Rows[e.RowIndex];
                string detalles = $"ID: {row.Cells["id"].Value}\n" +
                                 $"Tipo: {row.Cells["tipo"].Value}\n" +
                                 $"Severidad: {row.Cells["severidad"].Value}\n" +
                                 $"IP: {row.Cells["ip_involucrada"].Value}\n" +
                                 $"Fecha: {row.Cells["timestamp"].Value}\n\n" +
                                 $"Descripción:\n{row.Cells["descripcion"].Value}";
                MessageBox.Show(detalles, "Detalles de la Alerta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvAlertas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // No se necesita acción, pero el evento debe existir
        }
    }
}