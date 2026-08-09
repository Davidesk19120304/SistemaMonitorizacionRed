using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;

namespace SistemaMonitorizacionRed
{
    public partial class HistorialAlertas : Form
    {
        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";
        public bool ModoOscuro { get; set; } = false;
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

            // Hacer que los cambios en fecha y severidad actualicen automáticamente
            dtpFecha.ValueChanged += (s, ev) => CargarAlertas();
            cmbFiltroSeveridad.SelectedIndexChanged += (s, ev) => CargarAlertas();

            // Cargar todas las alertas al abrir (sin filtro de fecha)
            chkFiltrarFecha.Checked = false;
            CargarAlertas();
            if (ModoOscuro)
                AplicarTemaOscuro();
            else
                AplicarTemaClaro();
        }
        private void AplicarTemaOscuro()
        {
            this.BackColor = Color.FromArgb(30, 30, 35);
            this.ForeColor = Color.WhiteSmoke;

            // DataGridView
            dgvAlertas.BackgroundColor = Color.FromArgb(50, 50, 55);
            dgvAlertas.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgvAlertas.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvAlertas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(75, 75, 85);
            dgvAlertas.GridColor = Color.FromArgb(80, 80, 85);
            // Cambiar solo el encabezado del DataGridView
            dgvAlertas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 70, 75);
            dgvAlertas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAlertas.EnableHeadersVisualStyles = false;

            // ComboBox y CheckBox
            cmbFiltroSeveridad.BackColor = Color.FromArgb(70, 70, 75);
            cmbFiltroSeveridad.ForeColor = Color.White;
            chkFiltrarFecha.ForeColor = Color.WhiteSmoke;
            lblTotal.ForeColor = Color.WhiteSmoke;
            // Oscurecer el GroupBox de filtros
            gbFiltros.BackColor = Color.FromArgb(40, 40, 45);
            gbFiltros.ForeColor = Color.WhiteSmoke;
            // Labels de filtros
            lblFiltroSeveridad.ForeColor = Color.WhiteSmoke;
            lblFecha.ForeColor = Color.WhiteSmoke;
            // Cambiar el headerPanel a color oscuro
            headerPanel.BackColor = Color.FromArgb(45, 45, 48);
            titleLabel.ForeColor = Color.WhiteSmoke;
        }
        private void AplicarTemaClaro()
        {
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.ForeColor = Color.Black;

            dgvAlertas.BackgroundColor = Color.White;
            dgvAlertas.DefaultCellStyle.BackColor = Color.White;
            dgvAlertas.DefaultCellStyle.ForeColor = Color.Black;
            dgvAlertas.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvAlertas.GridColor = Color.LightGray;
            // Restaurar el encabezado del DataGridView al azul corporativo
            dgvAlertas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgvAlertas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAlertas.EnableHeadersVisualStyles = false;

            cmbFiltroSeveridad.BackColor = Color.White;
            cmbFiltroSeveridad.ForeColor = Color.Black;
            chkFiltrarFecha.ForeColor = Color.Black;
            lblTotal.ForeColor = Color.Black;
            gbFiltros.BackColor = Color.White;
            gbFiltros.ForeColor = Color.FromArgb(0, 51, 102);
            lblFiltroSeveridad.ForeColor = Color.Black;
            lblFecha.ForeColor = Color.Black;
            // Restaurar el headerPanel a color azul corporativo
            headerPanel.BackColor = Color.FromArgb(0, 102, 204);
            titleLabel.ForeColor = Color.White;
        }
        private void CargarAlertas()
        {
            string cacheKey = $"alertas_{cmbFiltroSeveridad.SelectedIndex}_{dtpFecha.Value:yyyyMMdd}_{chkFiltrarFecha.Checked}";

            DataTable dt = CacheHelper.GetOrSet(cacheKey, () =>
            {
                DataTable result = new DataTable();
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT id, tipo, descripcion, severidad, ip_involucrada, \"timestamp\" FROM alertas WHERE 1=1";

                        if (cmbFiltroSeveridad.SelectedIndex > 0)
                            query += " AND severidad = @severidad";
                        if (chkFiltrarFecha.Checked)
                            query += " AND DATE(\"timestamp\") = @fecha";
                        query += " ORDER BY \"timestamp\" DESC";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            if (cmbFiltroSeveridad.SelectedIndex > 0)
                                cmd.Parameters.AddWithValue("@severidad", cmbFiltroSeveridad.SelectedItem.ToString());
                            if (chkFiltrarFecha.Checked)
                                cmd.Parameters.AddWithValue("@fecha", dtpFecha.Value.ToString("yyyy-MM-dd"));

                            using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                            {
                                adapter.Fill(result);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error cargando alertas: " + ex.Message);
                }
                return result;
            }, TimeSpan.FromSeconds(30));

            dgvAlertas.DataSource = dt;
            lblTotal.Text = $"Total: {dt.Rows.Count} alertas";
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