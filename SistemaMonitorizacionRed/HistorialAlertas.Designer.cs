namespace SistemaMonitorizacionRed
{
    partial class HistorialAlertas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvAlertas = new System.Windows.Forms.DataGridView();
            this.cmbFiltroSeveridad = new System.Windows.Forms.ComboBox();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.chkFiltrarFecha = new System.Windows.Forms.CheckBox();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblFiltroSeveridad = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.gbFiltros.SuspendLayout();
            this.SuspendLayout();

            // ========== Panel superior (cabecera) ==========
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1000, 50);
            this.headerPanel.TabIndex = 0;

            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(20, 12);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Text = "📋 Historial de Alertas de Seguridad";

            // ========== GroupBox de filtros ==========
            this.gbFiltros.BackColor = System.Drawing.Color.White;
            this.gbFiltros.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gbFiltros.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.gbFiltros.Location = new System.Drawing.Point(20, 70);
            this.gbFiltros.Size = new System.Drawing.Size(960, 90);
            this.gbFiltros.Text = "Filtros de búsqueda";

            // lblFiltroSeveridad
            this.lblFiltroSeveridad.AutoSize = true;
            this.lblFiltroSeveridad.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblFiltroSeveridad.Location = new System.Drawing.Point(20, 35);
            this.lblFiltroSeveridad.Name = "lblFiltroSeveridad";
            this.lblFiltroSeveridad.Size = new System.Drawing.Size(60, 15);
            this.lblFiltroSeveridad.Text = "Severidad:";

            // cmbFiltroSeveridad
            this.cmbFiltroSeveridad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltroSeveridad.Location = new System.Drawing.Point(90, 32);
            this.cmbFiltroSeveridad.Size = new System.Drawing.Size(130, 23);
            this.cmbFiltroSeveridad.BackColor = System.Drawing.Color.White;

            // chkFiltrarFecha
            this.chkFiltrarFecha.AutoSize = true;
            this.chkFiltrarFecha.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.chkFiltrarFecha.Location = new System.Drawing.Point(250, 34);
            this.chkFiltrarFecha.Size = new System.Drawing.Size(110, 19);
            this.chkFiltrarFecha.Text = "Filtrar por fecha";
            this.chkFiltrarFecha.UseVisualStyleBackColor = true;
            this.chkFiltrarFecha.CheckedChanged += new System.EventHandler(this.chkFiltrarFecha_CheckedChanged);

            // lblFecha
            this.lblFecha.AutoSize = true;
            this.lblFecha.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblFecha.Location = new System.Drawing.Point(380, 36);
            this.lblFecha.Size = new System.Drawing.Size(45, 15);
            this.lblFecha.Text = "Fecha:";
            this.lblFecha.Visible = false;

            // dtpFecha
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFecha.Location = new System.Drawing.Point(430, 32);
            this.dtpFecha.Size = new System.Drawing.Size(110, 23);
            this.dtpFecha.Enabled = false;

            // btnActualizar
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.Location = new System.Drawing.Point(580, 30);
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);

            // btnCerrar
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCerrar.Location = new System.Drawing.Point(700, 30);
            this.btnCerrar.Size = new System.Drawing.Size(100, 30);
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            this.gbFiltros.Controls.Add(this.lblFiltroSeveridad);
            this.gbFiltros.Controls.Add(this.cmbFiltroSeveridad);
            this.gbFiltros.Controls.Add(this.chkFiltrarFecha);
            this.gbFiltros.Controls.Add(this.lblFecha);
            this.gbFiltros.Controls.Add(this.dtpFecha);
            this.gbFiltros.Controls.Add(this.btnActualizar);
            this.gbFiltros.Controls.Add(this.btnCerrar);

            // ========== DataGridView ==========
            this.dgvAlertas.BackgroundColor = System.Drawing.Color.White;
            this.dgvAlertas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvAlertas.GridColor = System.Drawing.Color.LightGray;
            this.dgvAlertas.Location = new System.Drawing.Point(20, 180);
            this.dgvAlertas.Size = new System.Drawing.Size(960, 350);
            this.dgvAlertas.TabIndex = 1;
            this.dgvAlertas.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAlertas_CellContentClick);
            this.dgvAlertas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAlertas_CellDoubleClick);

            // ========== Label total ==========
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblTotal.Location = new System.Drawing.Point(20, 550);
            this.lblTotal.Text = "Total: 0 alertas";

            // ========== Formulario ==========
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.dgvAlertas);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "HistorialAlertas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Historial de Alertas";
            this.Load += new System.EventHandler(this.HistorialAlertas_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvAlertas)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Declaración de controles
        private System.Windows.Forms.DataGridView dgvAlertas;
        private System.Windows.Forms.ComboBox cmbFiltroSeveridad;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.CheckBox chkFiltrarFecha;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Label lblFiltroSeveridad;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.GroupBox gbFiltros;
    }
}