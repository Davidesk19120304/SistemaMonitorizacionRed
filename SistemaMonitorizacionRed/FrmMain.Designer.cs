namespace SistemaMonitorizacionRed
{
    partial class FrmMain
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cmbInterfaces = new System.Windows.Forms.ComboBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnDetener = new System.Windows.Forms.Button();
            this.lblEstadisticas = new System.Windows.Forms.Label();
            this.dgvPaquetes = new System.Windows.Forms.DataGridView();
            this.chartTrafico = new LiveCharts.WinForms.CartesianChart();
            this.lblEstadisticasTiempoReal = new System.Windows.Forms.Label();
            this.BtnConfig = new System.Windows.Forms.Button();
            this.btnVerAlertas = new System.Windows.Forms.Button();
            this.cmbProtocolo = new System.Windows.Forms.ComboBox();
            this.txtFiltroIPOrigen = new System.Windows.Forms.TextBox();
            this.txtFiltroIPDestino = new System.Windows.Forms.TextBox();
            this.btnAplicarFiltro = new System.Windows.Forms.Button();
            this.btnLimpiarFiltros = new System.Windows.Forms.Button();
            this.labelFiltroIPOrigen = new System.Windows.Forms.Label();
            this.labelFiltroIPDestino = new System.Windows.Forms.Label();
            this.lblEstadisticasFiltro = new System.Windows.Forms.Label();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.gbCaptura = new System.Windows.Forms.GroupBox();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.archivoMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarPaquetesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarAlertasMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.salirMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.capturaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.iniciarCapturaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.detenerCapturaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.verMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.verAlertasMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.vistaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.modoOscuroMenu = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.gbCaptura.SuspendLayout();
            this.gbFiltros.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbInterfaces
            // 
            this.cmbInterfaces.BackColor = System.Drawing.Color.White;
            this.cmbInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterfaces.Location = new System.Drawing.Point(17, 26);
            this.cmbInterfaces.Name = "cmbInterfaces";
            this.cmbInterfaces.Size = new System.Drawing.Size(463, 23);
            this.cmbInterfaces.TabIndex = 0;
            // 
            // btnIniciar
            // 
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnIniciar.ForeColor = System.Drawing.Color.White;
            this.btnIniciar.Location = new System.Drawing.Point(304, 66);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(69, 24);
            this.btnIniciar.TabIndex = 1;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnDetener
            // 
            this.btnDetener.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDetener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetener.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDetener.ForeColor = System.Drawing.Color.White;
            this.btnDetener.Location = new System.Drawing.Point(403, 66);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(69, 24);
            this.btnDetener.TabIndex = 2;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = false;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);
            // 
            // lblEstadisticas
            // 
            this.lblEstadisticas.AutoSize = true;
            this.lblEstadisticas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadisticas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.lblEstadisticas.Location = new System.Drawing.Point(14, 71);
            this.lblEstadisticas.Name = "lblEstadisticas";
            this.lblEstadisticas.Size = new System.Drawing.Size(135, 15);
            this.lblEstadisticas.TabIndex = 3;
            this.lblEstadisticas.Text = "Paquetes capturados: 0";
            this.lblEstadisticas.Click += new System.EventHandler(this.lblEstadisticas_Click);
            // 
            // dgvPaquetes
            // 
            this.dgvPaquetes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPaquetes.BackgroundColor = System.Drawing.Color.White;
            this.dgvPaquetes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            this.dgvPaquetes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvPaquetes.EnableHeadersVisualStyles = false;
            this.dgvPaquetes.Location = new System.Drawing.Point(9, 81);
            this.dgvPaquetes.Name = "dgvPaquetes";
            this.dgvPaquetes.Size = new System.Drawing.Size(1080, 208);
            this.dgvPaquetes.TabIndex = 2;
            this.dgvPaquetes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvPaquetes_CellFormatting);
            // 
            // chartTrafico
            // 
            this.chartTrafico.Location = new System.Drawing.Point(10, 344);
            this.chartTrafico.Name = "chartTrafico";
            this.chartTrafico.Size = new System.Drawing.Size(561, 232);
            this.chartTrafico.TabIndex = 5;
            // 
            // lblEstadisticasTiempoReal
            // 
            this.lblEstadisticasTiempoReal.AutoSize = true;
            this.lblEstadisticasTiempoReal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadisticasTiempoReal.Location = new System.Drawing.Point(10, 307);
            this.lblEstadisticasTiempoReal.Name = "lblEstadisticasTiempoReal";
            this.lblEstadisticasTiempoReal.Size = new System.Drawing.Size(231, 15);
            this.lblEstadisticasTiempoReal.TabIndex = 3;
            this.lblEstadisticasTiempoReal.Text = "TCP: 0/s | UDP: 0/s | ICMP: 0/s | Total: 0/s";
            // 
            // BtnConfig
            // 
            this.BtnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.BtnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnConfig.ForeColor = System.Drawing.Color.White;
            this.BtnConfig.Location = new System.Drawing.Point(679, 550);
            this.BtnConfig.Name = "BtnConfig";
            this.BtnConfig.Size = new System.Drawing.Size(129, 26);
            this.BtnConfig.TabIndex = 2;
            this.BtnConfig.Text = "Configuración";
            this.BtnConfig.UseVisualStyleBackColor = false;
            this.BtnConfig.Click += new System.EventHandler(this.BtnConfig_Click);
            // 
            // btnVerAlertas
            // 
            this.btnVerAlertas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnVerAlertas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerAlertas.ForeColor = System.Drawing.Color.White;
            this.btnVerAlertas.Location = new System.Drawing.Point(864, 550);
            this.btnVerAlertas.Name = "btnVerAlertas";
            this.btnVerAlertas.Size = new System.Drawing.Size(129, 26);
            this.btnVerAlertas.TabIndex = 0;
            this.btnVerAlertas.Text = "Ver Historial de Alertas";
            this.btnVerAlertas.UseVisualStyleBackColor = false;
            this.btnVerAlertas.Click += new System.EventHandler(this.btnVerAlertas_Click);
            // 
            // cmbProtocolo
            // 
            this.cmbProtocolo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProtocolo.Items.AddRange(new object[] {
            "Todos",
            "TCP",
            "UDP",
            "ICMP",
            "IGMP"});
            this.cmbProtocolo.Location = new System.Drawing.Point(17, 30);
            this.cmbProtocolo.Name = "cmbProtocolo";
            this.cmbProtocolo.Size = new System.Drawing.Size(103, 23);
            this.cmbProtocolo.TabIndex = 0;
            // 
            // txtFiltroIPOrigen
            // 
            this.txtFiltroIPOrigen.Location = new System.Drawing.Point(154, 30);
            this.txtFiltroIPOrigen.Name = "txtFiltroIPOrigen";
            this.txtFiltroIPOrigen.Size = new System.Drawing.Size(103, 23);
            this.txtFiltroIPOrigen.TabIndex = 2;
            // 
            // txtFiltroIPDestino
            // 
            this.txtFiltroIPDestino.Location = new System.Drawing.Point(283, 30);
            this.txtFiltroIPDestino.Name = "txtFiltroIPDestino";
            this.txtFiltroIPDestino.Size = new System.Drawing.Size(103, 23);
            this.txtFiltroIPDestino.TabIndex = 4;
            // 
            // btnAplicarFiltro
            // 
            this.btnAplicarFiltro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnAplicarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarFiltro.ForeColor = System.Drawing.Color.White;
            this.btnAplicarFiltro.Location = new System.Drawing.Point(403, 26);
            this.btnAplicarFiltro.Name = "btnAplicarFiltro";
            this.btnAplicarFiltro.Size = new System.Drawing.Size(77, 26);
            this.btnAplicarFiltro.TabIndex = 6;
            this.btnAplicarFiltro.Text = "Aplicar";
            this.btnAplicarFiltro.UseVisualStyleBackColor = false;
            this.btnAplicarFiltro.Click += new System.EventHandler(this.btnAplicarFiltro_Click);
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.BackColor = System.Drawing.Color.LightGray;
            this.btnLimpiarFiltros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarFiltros.ForeColor = System.Drawing.Color.Black;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(403, 61);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(77, 26);
            this.btnLimpiarFiltros.TabIndex = 7;
            this.btnLimpiarFiltros.Text = "Limpiar";
            this.btnLimpiarFiltros.UseVisualStyleBackColor = false;
            this.btnLimpiarFiltros.Click += new System.EventHandler(this.btnLimpiarFiltros_Click);
            // 
            // labelFiltroIPOrigen
            // 
            this.labelFiltroIPOrigen.AutoSize = true;
            this.labelFiltroIPOrigen.Location = new System.Drawing.Point(152, 10);
            this.labelFiltroIPOrigen.Name = "labelFiltroIPOrigen";
            this.labelFiltroIPOrigen.Size = new System.Drawing.Size(62, 15);
            this.labelFiltroIPOrigen.TabIndex = 3;
            this.labelFiltroIPOrigen.Text = "IP Origen:";
            // 
            // labelFiltroIPDestino
            // 
            this.labelFiltroIPDestino.AutoSize = true;
            this.labelFiltroIPDestino.Location = new System.Drawing.Point(280, 10);
            this.labelFiltroIPDestino.Name = "labelFiltroIPDestino";
            this.labelFiltroIPDestino.Size = new System.Drawing.Size(67, 15);
            this.labelFiltroIPDestino.TabIndex = 5;
            this.labelFiltroIPDestino.Text = "IP Destino:";
            // 
            // lblEstadisticasFiltro
            // 
            this.lblEstadisticasFiltro.AutoSize = true;
            this.lblEstadisticasFiltro.Location = new System.Drawing.Point(17, 61);
            this.lblEstadisticasFiltro.Name = "lblEstadisticasFiltro";
            this.lblEstadisticasFiltro.Size = new System.Drawing.Size(191, 15);
            this.lblEstadisticasFiltro.TabIndex = 8;
            this.lblEstadisticasFiltro.Text = "TCP: 0 | UDP: 0 | ICMP: 0 | Total: 0";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Location = new System.Drawing.Point(0, 24);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(651, 43);
            this.headerPanel.TabIndex = 101;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(17, 10);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(520, 25);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "📡 Sistema de Análisis y Monitorización de Tráfico de Red";
            this.titleLabel.Click += new System.EventHandler(this.titleLabel_Click);
            // 
            // gbCaptura
            // 
            this.gbCaptura.BackColor = System.Drawing.Color.White;
            this.gbCaptura.Controls.Add(this.cmbInterfaces);
            this.gbCaptura.Controls.Add(this.btnIniciar);
            this.gbCaptura.Controls.Add(this.btnDetener);
            this.gbCaptura.Controls.Add(this.lblEstadisticas);
            this.gbCaptura.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gbCaptura.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.gbCaptura.Location = new System.Drawing.Point(594, 307);
            this.gbCaptura.Name = "gbCaptura";
            this.gbCaptura.Size = new System.Drawing.Size(495, 100);
            this.gbCaptura.TabIndex = 7;
            this.gbCaptura.TabStop = false;
            this.gbCaptura.Text = "Captura de Red";
            // 
            // gbFiltros
            // 
            this.gbFiltros.BackColor = System.Drawing.Color.White;
            this.gbFiltros.Controls.Add(this.cmbProtocolo);
            this.gbFiltros.Controls.Add(this.txtFiltroIPOrigen);
            this.gbFiltros.Controls.Add(this.labelFiltroIPOrigen);
            this.gbFiltros.Controls.Add(this.txtFiltroIPDestino);
            this.gbFiltros.Controls.Add(this.labelFiltroIPDestino);
            this.gbFiltros.Controls.Add(this.btnAplicarFiltro);
            this.gbFiltros.Controls.Add(this.btnLimpiarFiltros);
            this.gbFiltros.Controls.Add(this.lblEstadisticasFiltro);
            this.gbFiltros.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gbFiltros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.gbFiltros.Location = new System.Drawing.Point(594, 415);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(495, 119);
            this.gbFiltros.TabIndex = 6;
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "Filtros de Tráfico";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoMenu,
            this.capturaMenu,
            this.verMenu,
            this.vistaMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1101, 24);
            this.menuStrip.TabIndex = 100;
            // 
            // archivoMenu
            // 
            this.archivoMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportarPaquetesMenu,
            this.exportarAlertasMenu,
            this.salirMenu});
            this.archivoMenu.Name = "archivoMenu";
            this.archivoMenu.Size = new System.Drawing.Size(60, 20);
            this.archivoMenu.Text = "Archivo";
            // 
            // exportarPaquetesMenu
            // 
            this.exportarPaquetesMenu.Name = "exportarPaquetesMenu";
            this.exportarPaquetesMenu.Size = new System.Drawing.Size(192, 22);
            this.exportarPaquetesMenu.Text = "Exportar paquetes CSV";
            this.exportarPaquetesMenu.Click += new System.EventHandler(this.ExportarPaquetesMenu_Click);
            // 
            // exportarAlertasMenu
            // 
            this.exportarAlertasMenu.Name = "exportarAlertasMenu";
            this.exportarAlertasMenu.Size = new System.Drawing.Size(192, 22);
            this.exportarAlertasMenu.Text = "Exportar alertas CSV";
            this.exportarAlertasMenu.Click += new System.EventHandler(this.ExportarAlertasMenu_Click);
            // 
            // salirMenu
            // 
            this.salirMenu.Name = "salirMenu";
            this.salirMenu.Size = new System.Drawing.Size(192, 22);
            this.salirMenu.Text = "Salir";
            this.salirMenu.Click += new System.EventHandler(this.SalirMenu_Click);
            // 
            // capturaMenu
            // 
            this.capturaMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iniciarCapturaMenu,
            this.detenerCapturaMenu});
            this.capturaMenu.Name = "capturaMenu";
            this.capturaMenu.Size = new System.Drawing.Size(61, 20);
            this.capturaMenu.Text = "Captura";
            // 
            // iniciarCapturaMenu
            // 
            this.iniciarCapturaMenu.Name = "iniciarCapturaMenu";
            this.iniciarCapturaMenu.Size = new System.Drawing.Size(158, 22);
            this.iniciarCapturaMenu.Text = "Iniciar captura";
            this.iniciarCapturaMenu.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // detenerCapturaMenu
            // 
            this.detenerCapturaMenu.Name = "detenerCapturaMenu";
            this.detenerCapturaMenu.Size = new System.Drawing.Size(158, 22);
            this.detenerCapturaMenu.Text = "Detener captura";
            this.detenerCapturaMenu.Click += new System.EventHandler(this.btnDetener_Click);
            // 
            // verMenu
            // 
            this.verMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verAlertasMenu});
            this.verMenu.Name = "verMenu";
            this.verMenu.Size = new System.Drawing.Size(35, 20);
            this.verMenu.Text = "Ver";
            // 
            // verAlertasMenu
            // 
            this.verAlertasMenu.Name = "verAlertasMenu";
            this.verAlertasMenu.Size = new System.Drawing.Size(171, 22);
            this.verAlertasMenu.Text = "Historial de alertas";
            this.verAlertasMenu.Click += new System.EventHandler(this.btnVerAlertas_Click);
            // 
            // vistaMenu
            // 
            this.vistaMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modoOscuroMenu});
            this.vistaMenu.Name = "vistaMenu";
            this.vistaMenu.Size = new System.Drawing.Size(44, 20);
            this.vistaMenu.Text = "Vista";
            // 
            // modoOscuroMenu
            // 
            this.modoOscuroMenu.Name = "modoOscuroMenu";
            this.modoOscuroMenu.Size = new System.Drawing.Size(145, 22);
            this.modoOscuroMenu.Text = "Modo oscuro";
            this.modoOscuroMenu.Click += new System.EventHandler(this.ToggleModoOscuro);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1101, 616);
            this.Controls.Add(this.btnVerAlertas);
            this.Controls.Add(this.BtnConfig);
            this.Controls.Add(this.lblEstadisticasTiempoReal);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.chartTrafico);
            this.Controls.Add(this.dgvPaquetes);
            this.Controls.Add(this.gbCaptura);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Monitorización de Red";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.gbCaptura.ResumeLayout(false);
            this.gbCaptura.PerformLayout();
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // Declaración de controles
        private System.Windows.Forms.ComboBox cmbInterfaces;
        private System.Windows.Forms.Button btnIniciar, btnDetener;
        private System.Windows.Forms.Label lblEstadisticas;
        private System.Windows.Forms.DataGridView dgvPaquetes;
        private LiveCharts.WinForms.CartesianChart chartTrafico;
        private System.Windows.Forms.Label lblEstadisticasTiempoReal;
        private System.Windows.Forms.Button BtnConfig, btnVerAlertas;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.GroupBox gbCaptura, gbFiltros;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem archivoMenu, capturaMenu, verMenu, vistaMenu;
        private System.Windows.Forms.ToolStripMenuItem exportarPaquetesMenu, exportarAlertasMenu, salirMenu;
        private System.Windows.Forms.ToolStripMenuItem iniciarCapturaMenu, detenerCapturaMenu;
        private System.Windows.Forms.ToolStripMenuItem verAlertasMenu;
        private System.Windows.Forms.ToolStripMenuItem modoOscuroMenu;
        private System.Windows.Forms.ComboBox cmbProtocolo;
        private System.Windows.Forms.TextBox txtFiltroIPOrigen, txtFiltroIPDestino;
        private System.Windows.Forms.Button btnAplicarFiltro, btnLimpiarFiltros;
        private System.Windows.Forms.Label labelFiltroIPOrigen, labelFiltroIPDestino, lblEstadisticasFiltro;
    }
}