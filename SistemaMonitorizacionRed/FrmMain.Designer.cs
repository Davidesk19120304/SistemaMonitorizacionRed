namespace SistemaMonitorizacionRed
{
    partial class FrmMain
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem herramientasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configuracionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarPaquetesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarAlertasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alertasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem historialAlertasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vistaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modoOscuroToolStripMenuItem;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox gbDiagnostico;
        private System.Windows.Forms.Panel panelScrollDiagnostico;
        private CircularProgressBar.CircularProgressBar CpbVelocidadEnlace2;
        private System.Windows.Forms.Label lblVelocidadValor2;
        private CircularProgressBar.CircularProgressBar CpbLatencia2;
        private System.Windows.Forms.Label lblLatenciaValor2;
        private CircularProgressBar.CircularProgressBar CpbBandaWifi2;
        private System.Windows.Forms.Label lblBandaValor2;
        private System.Windows.Forms.GroupBox gbCaptura;
        private System.Windows.Forms.ComboBox cmbInterfaces;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.Label lblEstadisticas;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.ComboBox cmbProtocolo;
        private System.Windows.Forms.ComboBox cmbFiltroIPOrigen;
        private System.Windows.Forms.Label labelFiltroIPOrigen;
        private System.Windows.Forms.ComboBox cmbFiltroIPDestino;
        private System.Windows.Forms.Label labelFiltroIPDestino;
        private System.Windows.Forms.Button btnAplicarFiltro;
        private System.Windows.Forms.Button btnLimpiarFiltros;
        private System.Windows.Forms.Label lblEstadisticasFiltro;
        private System.Windows.Forms.Label lblEstadisticasTiempoReal;
        private System.Windows.Forms.DataGridView dgvPaquetes;
        private LiveCharts.WinForms.CartesianChart chartTrafico;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.herramientasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuracionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarPaquetesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarAlertasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alertasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historialAlertasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vistaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modoOscuroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gbDiagnostico = new System.Windows.Forms.GroupBox();
            this.panelScrollDiagnostico = new System.Windows.Forms.Panel();
            this.CpbVelocidadEnlace2 = new CircularProgressBar.CircularProgressBar();
            this.lblVelocidadValor2 = new System.Windows.Forms.Label();
            this.CpbLatencia2 = new CircularProgressBar.CircularProgressBar();
            this.lblLatenciaValor2 = new System.Windows.Forms.Label();
            this.CpbBandaWifi2 = new CircularProgressBar.CircularProgressBar();
            this.lblBandaValor2 = new System.Windows.Forms.Label();
            this.gbCaptura = new System.Windows.Forms.GroupBox();
            this.cmbInterfaces = new System.Windows.Forms.ComboBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnDetener = new System.Windows.Forms.Button();
            this.lblEstadisticas = new System.Windows.Forms.Label();
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.cmbProtocolo = new System.Windows.Forms.ComboBox();
            this.cmbFiltroIPOrigen = new System.Windows.Forms.ComboBox();
            this.labelFiltroIPOrigen = new System.Windows.Forms.Label();
            this.cmbFiltroIPDestino = new System.Windows.Forms.ComboBox();
            this.labelFiltroIPDestino = new System.Windows.Forms.Label();
            this.btnAplicarFiltro = new System.Windows.Forms.Button();
            this.btnLimpiarFiltros = new System.Windows.Forms.Button();
            this.lblEstadisticasTiempoReal = new System.Windows.Forms.Label();
            this.lblEstadisticasFiltro = new System.Windows.Forms.Label();
            this.dgvPaquetes = new System.Windows.Forms.DataGridView();
            this.chartTrafico = new LiveCharts.WinForms.CartesianChart();
            this.menuStrip.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbDiagnostico.SuspendLayout();
            this.panelScrollDiagnostico.SuspendLayout();
            this.gbCaptura.SuspendLayout();
            this.gbFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.herramientasToolStripMenuItem,
            this.exportarToolStripMenuItem,
            this.alertasToolStripMenuItem,
            this.vistaToolStripMenuItem,
            this.archivoToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1222, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.salirToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.archivoToolStripMenuItem.Text = "Cerrar sesion";
            this.archivoToolStripMenuItem.Click += new System.EventHandler(this.archivoToolStripMenuItem_Click_1);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.SalirMenu_Click);
            // 
            // herramientasToolStripMenuItem
            // 
            this.herramientasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configuracionToolStripMenuItem});
            this.herramientasToolStripMenuItem.Name = "herramientasToolStripMenuItem";
            this.herramientasToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.herramientasToolStripMenuItem.Text = "Herramientas";
            // 
            // configuracionToolStripMenuItem
            // 
            this.configuracionToolStripMenuItem.Name = "configuracionToolStripMenuItem";
            this.configuracionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.configuracionToolStripMenuItem.Text = "Configuración";
            this.configuracionToolStripMenuItem.Click += new System.EventHandler(this.BtnConfig_Click);
            // 
            // exportarToolStripMenuItem
            // 
            this.exportarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportarPaquetesToolStripMenuItem,
            this.exportarAlertasToolStripMenuItem});
            this.exportarToolStripMenuItem.Name = "exportarToolStripMenuItem";
            this.exportarToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.exportarToolStripMenuItem.Text = "Exportar";
            // 
            // exportarPaquetesToolStripMenuItem
            // 
            this.exportarPaquetesToolStripMenuItem.Name = "exportarPaquetesToolStripMenuItem";
            this.exportarPaquetesToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.exportarPaquetesToolStripMenuItem.Text = "Paquetes...";
            this.exportarPaquetesToolStripMenuItem.Click += new System.EventHandler(this.ExportarPaquetesMenu_Click);
            // 
            // exportarAlertasToolStripMenuItem
            // 
            this.exportarAlertasToolStripMenuItem.Name = "exportarAlertasToolStripMenuItem";
            this.exportarAlertasToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.exportarAlertasToolStripMenuItem.Text = "Alertas...";
            this.exportarAlertasToolStripMenuItem.Click += new System.EventHandler(this.ExportarAlertasMenu_Click);
            // 
            // alertasToolStripMenuItem
            // 
            this.alertasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historialAlertasToolStripMenuItem});
            this.alertasToolStripMenuItem.Name = "alertasToolStripMenuItem";
            this.alertasToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.alertasToolStripMenuItem.Text = "Alertas";
            // 
            // historialAlertasToolStripMenuItem
            // 
            this.historialAlertasToolStripMenuItem.Name = "historialAlertasToolStripMenuItem";
            this.historialAlertasToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.historialAlertasToolStripMenuItem.Text = "Historial de Alertas";
            this.historialAlertasToolStripMenuItem.Click += new System.EventHandler(this.btnVerAlertas_Click);
            // 
            // vistaToolStripMenuItem
            // 
            this.vistaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modoOscuroToolStripMenuItem});
            this.vistaToolStripMenuItem.Name = "vistaToolStripMenuItem";
            this.vistaToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.vistaToolStripMenuItem.Text = "Vista";
            // 
            // modoOscuroToolStripMenuItem
            // 
            this.modoOscuroToolStripMenuItem.Name = "modoOscuroToolStripMenuItem";
            this.modoOscuroToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.modoOscuroToolStripMenuItem.Text = "Modo oscuro";
            this.modoOscuroToolStripMenuItem.Click += new System.EventHandler(this.ToggleModoOscuro);
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 24);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1222, 70);
            this.headerPanel.TabIndex = 1;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(175, 16);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(246, 30);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Monitorización de Red";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 100);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1210, 526);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gbDiagnostico);
            this.tabPage1.Controls.Add(this.gbCaptura);
            this.tabPage1.Controls.Add(this.gbFiltros);
            this.tabPage1.Controls.Add(this.lblEstadisticasTiempoReal);
            this.tabPage1.Controls.Add(this.lblEstadisticasFiltro);
            this.tabPage1.Controls.Add(this.dgvPaquetes);
            this.tabPage1.Controls.Add(this.chartTrafico);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1202, 500);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Monitorización";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click_1);
            // 
            // gbDiagnostico
            // 
            this.gbDiagnostico.Controls.Add(this.panelScrollDiagnostico);
            this.gbDiagnostico.Location = new System.Drawing.Point(680, 6);
            this.gbDiagnostico.Name = "gbDiagnostico";
            this.gbDiagnostico.Size = new System.Drawing.Size(514, 252);
            this.gbDiagnostico.TabIndex = 20;
            this.gbDiagnostico.TabStop = false;
            this.gbDiagnostico.Text = "Diagnóstico";
            // 
            // panelScrollDiagnostico
            // 
            this.panelScrollDiagnostico.AutoScroll = true;
            this.panelScrollDiagnostico.Controls.Add(this.CpbVelocidadEnlace2);
            this.panelScrollDiagnostico.Controls.Add(this.lblVelocidadValor2);
            this.panelScrollDiagnostico.Controls.Add(this.CpbLatencia2);
            this.panelScrollDiagnostico.Controls.Add(this.lblLatenciaValor2);
            this.panelScrollDiagnostico.Controls.Add(this.CpbBandaWifi2);
            this.panelScrollDiagnostico.Controls.Add(this.lblBandaValor2);
            this.panelScrollDiagnostico.Location = new System.Drawing.Point(10, 19);
            this.panelScrollDiagnostico.Name = "panelScrollDiagnostico";
            this.panelScrollDiagnostico.Size = new System.Drawing.Size(498, 227);
            this.panelScrollDiagnostico.TabIndex = 0;
            // 
            // CpbVelocidadEnlace2
            // 
            this.CpbVelocidadEnlace2.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.CpbVelocidadEnlace2.AnimationSpeed = 500;
            this.CpbVelocidadEnlace2.BackColor = System.Drawing.Color.Transparent;
            this.CpbVelocidadEnlace2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.CpbVelocidadEnlace2.ForeColor = System.Drawing.Color.Black;
            this.CpbVelocidadEnlace2.InnerColor = System.Drawing.Color.White;
            this.CpbVelocidadEnlace2.InnerMargin = 2;
            this.CpbVelocidadEnlace2.InnerWidth = -1;
            this.CpbVelocidadEnlace2.Location = new System.Drawing.Point(16, 65);
            this.CpbVelocidadEnlace2.MarqueeAnimationSpeed = 2000;
            this.CpbVelocidadEnlace2.Maximum = 1000;
            this.CpbVelocidadEnlace2.Name = "CpbVelocidadEnlace2";
            this.CpbVelocidadEnlace2.OuterColor = System.Drawing.Color.LightGray;
            this.CpbVelocidadEnlace2.OuterMargin = -25;
            this.CpbVelocidadEnlace2.OuterWidth = 26;
            this.CpbVelocidadEnlace2.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.CpbVelocidadEnlace2.ProgressWidth = 25;
            this.CpbVelocidadEnlace2.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.CpbVelocidadEnlace2.Size = new System.Drawing.Size(147, 144);
            this.CpbVelocidadEnlace2.StartAngle = 270;
            this.CpbVelocidadEnlace2.SubscriptColor = System.Drawing.Color.Gray;
            this.CpbVelocidadEnlace2.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.CpbVelocidadEnlace2.SubscriptText = "";
            this.CpbVelocidadEnlace2.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.CpbVelocidadEnlace2.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.CpbVelocidadEnlace2.SuperscriptText = "°C";
            this.CpbVelocidadEnlace2.TabIndex = 0;
            this.CpbVelocidadEnlace2.Text = "0 Mbps";
            this.CpbVelocidadEnlace2.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.CpbVelocidadEnlace2.Value = 68;
            // 
            // lblVelocidadValor2
            // 
            this.lblVelocidadValor2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblVelocidadValor2.Location = new System.Drawing.Point(58, 33);
            this.lblVelocidadValor2.Name = "lblVelocidadValor2";
            this.lblVelocidadValor2.Size = new System.Drawing.Size(70, 15);
            this.lblVelocidadValor2.TabIndex = 1;
            this.lblVelocidadValor2.Text = "Velocidad";
            this.lblVelocidadValor2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpbLatencia2
            // 
            this.CpbLatencia2.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.CpbLatencia2.AnimationSpeed = 500;
            this.CpbLatencia2.BackColor = System.Drawing.Color.Transparent;
            this.CpbLatencia2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.CpbLatencia2.ForeColor = System.Drawing.Color.Black;
            this.CpbLatencia2.InnerColor = System.Drawing.Color.White;
            this.CpbLatencia2.InnerMargin = 2;
            this.CpbLatencia2.InnerWidth = -1;
            this.CpbLatencia2.Location = new System.Drawing.Point(187, 65);
            this.CpbLatencia2.MarqueeAnimationSpeed = 2000;
            this.CpbLatencia2.Maximum = 500;
            this.CpbLatencia2.Name = "CpbLatencia2";
            this.CpbLatencia2.OuterColor = System.Drawing.Color.LightGray;
            this.CpbLatencia2.OuterMargin = -25;
            this.CpbLatencia2.OuterWidth = 26;
            this.CpbLatencia2.ProgressColor = System.Drawing.Color.Green;
            this.CpbLatencia2.ProgressWidth = 25;
            this.CpbLatencia2.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.CpbLatencia2.Size = new System.Drawing.Size(149, 144);
            this.CpbLatencia2.StartAngle = 270;
            this.CpbLatencia2.SubscriptColor = System.Drawing.Color.Gray;
            this.CpbLatencia2.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.CpbLatencia2.SubscriptText = "";
            this.CpbLatencia2.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.CpbLatencia2.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.CpbLatencia2.SuperscriptText = "°C";
            this.CpbLatencia2.TabIndex = 2;
            this.CpbLatencia2.Text = "0 ms";
            this.CpbLatencia2.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.CpbLatencia2.Value = 68;
            // 
            // lblLatenciaValor2
            // 
            this.lblLatenciaValor2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblLatenciaValor2.Location = new System.Drawing.Point(224, 30);
            this.lblLatenciaValor2.Name = "lblLatenciaValor2";
            this.lblLatenciaValor2.Size = new System.Drawing.Size(70, 15);
            this.lblLatenciaValor2.TabIndex = 3;
            this.lblLatenciaValor2.Text = "Latencia";
            this.lblLatenciaValor2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CpbBandaWifi2
            // 
            this.CpbBandaWifi2.AnimationFunction = WinFormAnimation.KnownAnimationFunctions.Liner;
            this.CpbBandaWifi2.AnimationSpeed = 500;
            this.CpbBandaWifi2.BackColor = System.Drawing.Color.Transparent;
            this.CpbBandaWifi2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.CpbBandaWifi2.ForeColor = System.Drawing.Color.Black;
            this.CpbBandaWifi2.InnerColor = System.Drawing.Color.White;
            this.CpbBandaWifi2.InnerMargin = 2;
            this.CpbBandaWifi2.InnerWidth = -1;
            this.CpbBandaWifi2.Location = new System.Drawing.Point(352, 65);
            this.CpbBandaWifi2.MarqueeAnimationSpeed = 2000;
            this.CpbBandaWifi2.Maximum = 3;
            this.CpbBandaWifi2.Name = "CpbBandaWifi2";
            this.CpbBandaWifi2.OuterColor = System.Drawing.Color.LightGray;
            this.CpbBandaWifi2.OuterMargin = -25;
            this.CpbBandaWifi2.OuterWidth = 26;
            this.CpbBandaWifi2.ProgressColor = System.Drawing.Color.Orange;
            this.CpbBandaWifi2.ProgressWidth = 25;
            this.CpbBandaWifi2.SecondaryFont = new System.Drawing.Font("Microsoft Sans Serif", 36F);
            this.CpbBandaWifi2.Size = new System.Drawing.Size(143, 142);
            this.CpbBandaWifi2.StartAngle = 270;
            this.CpbBandaWifi2.SubscriptColor = System.Drawing.Color.Gray;
            this.CpbBandaWifi2.SubscriptMargin = new System.Windows.Forms.Padding(10, -35, 0, 0);
            this.CpbBandaWifi2.SubscriptText = "";
            this.CpbBandaWifi2.SuperscriptColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
            this.CpbBandaWifi2.SuperscriptMargin = new System.Windows.Forms.Padding(10, 35, 0, 0);
            this.CpbBandaWifi2.SuperscriptText = "°C";
            this.CpbBandaWifi2.TabIndex = 4;
            this.CpbBandaWifi2.Text = "2.4 GHz";
            this.CpbBandaWifi2.TextMargin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.CpbBandaWifi2.Value = 3;
            // 
            // lblBandaValor2
            // 
            this.lblBandaValor2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Bold);
            this.lblBandaValor2.Location = new System.Drawing.Point(392, 33);
            this.lblBandaValor2.Name = "lblBandaValor2";
            this.lblBandaValor2.Size = new System.Drawing.Size(70, 15);
            this.lblBandaValor2.TabIndex = 5;
            this.lblBandaValor2.Text = "Banda Wi-Fi";
            this.lblBandaValor2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.gbCaptura.Location = new System.Drawing.Point(6, 6);
            this.gbCaptura.Name = "gbCaptura";
            this.gbCaptura.Size = new System.Drawing.Size(246, 120);
            this.gbCaptura.TabIndex = 0;
            this.gbCaptura.TabStop = false;
            this.gbCaptura.Text = "Captura";
            // 
            // cmbInterfaces
            // 
            this.cmbInterfaces.BackColor = System.Drawing.Color.White;
            this.cmbInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterfaces.Location = new System.Drawing.Point(13, 26);
            this.cmbInterfaces.Name = "cmbInterfaces";
            this.cmbInterfaces.Size = new System.Drawing.Size(224, 23);
            this.cmbInterfaces.TabIndex = 0;
            // 
            // btnIniciar
            // 
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnIniciar.ForeColor = System.Drawing.Color.White;
            this.btnIniciar.Location = new System.Drawing.Point(13, 77);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(69, 26);
            this.btnIniciar.TabIndex = 2;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // btnDetener
            // 
            this.btnDetener.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnDetener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetener.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDetener.ForeColor = System.Drawing.Color.White;
            this.btnDetener.Location = new System.Drawing.Point(88, 76);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(66, 26);
            this.btnDetener.TabIndex = 3;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = false;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);
            // 
            // lblEstadisticas
            // 
            this.lblEstadisticas.AutoSize = true;
            this.lblEstadisticas.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblEstadisticas.Location = new System.Drawing.Point(10, 54);
            this.lblEstadisticas.Name = "lblEstadisticas";
            this.lblEstadisticas.Size = new System.Drawing.Size(127, 13);
            this.lblEstadisticas.TabIndex = 1;
            this.lblEstadisticas.Text = "Paquetes capturados: 0";
            // 
            // gbFiltros
            // 
            this.gbFiltros.BackColor = System.Drawing.Color.White;
            this.gbFiltros.Controls.Add(this.cmbProtocolo);
            this.gbFiltros.Controls.Add(this.cmbFiltroIPOrigen);
            this.gbFiltros.Controls.Add(this.labelFiltroIPOrigen);
            this.gbFiltros.Controls.Add(this.cmbFiltroIPDestino);
            this.gbFiltros.Controls.Add(this.labelFiltroIPDestino);
            this.gbFiltros.Controls.Add(this.btnAplicarFiltro);
            this.gbFiltros.Controls.Add(this.btnLimpiarFiltros);
            this.gbFiltros.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gbFiltros.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.gbFiltros.Location = new System.Drawing.Point(258, 6);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(416, 120);
            this.gbFiltros.TabIndex = 1;
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "Filtros de Tráfico";
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
            this.cmbProtocolo.Location = new System.Drawing.Point(13, 26);
            this.cmbProtocolo.Name = "cmbProtocolo";
            this.cmbProtocolo.Size = new System.Drawing.Size(134, 23);
            this.cmbProtocolo.TabIndex = 0;
            // 
            // cmbFiltroIPOrigen
            // 
            this.cmbFiltroIPOrigen.Location = new System.Drawing.Point(16, 77);
            this.cmbFiltroIPOrigen.Name = "cmbFiltroIPOrigen";
            this.cmbFiltroIPOrigen.Size = new System.Drawing.Size(131, 23);
            this.cmbFiltroIPOrigen.TabIndex = 1;
            // 
            // labelFiltroIPOrigen
            // 
            this.labelFiltroIPOrigen.AutoSize = true;
            this.labelFiltroIPOrigen.Location = new System.Drawing.Point(13, 53);
            this.labelFiltroIPOrigen.Name = "labelFiltroIPOrigen";
            this.labelFiltroIPOrigen.Size = new System.Drawing.Size(62, 15);
            this.labelFiltroIPOrigen.TabIndex = 2;
            this.labelFiltroIPOrigen.Text = "IP Origen:";
            // 
            // cmbFiltroIPDestino
            // 
            this.cmbFiltroIPDestino.Location = new System.Drawing.Point(174, 76);
            this.cmbFiltroIPDestino.Name = "cmbFiltroIPDestino";
            this.cmbFiltroIPDestino.Size = new System.Drawing.Size(129, 23);
            this.cmbFiltroIPDestino.TabIndex = 3;
            // 
            // labelFiltroIPDestino
            // 
            this.labelFiltroIPDestino.AutoSize = true;
            this.labelFiltroIPDestino.Location = new System.Drawing.Point(171, 53);
            this.labelFiltroIPDestino.Name = "labelFiltroIPDestino";
            this.labelFiltroIPDestino.Size = new System.Drawing.Size(67, 15);
            this.labelFiltroIPDestino.TabIndex = 4;
            this.labelFiltroIPDestino.Text = "IP Destino:";
            // 
            // btnAplicarFiltro
            // 
            this.btnAplicarFiltro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.btnAplicarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarFiltro.ForeColor = System.Drawing.Color.White;
            this.btnAplicarFiltro.Location = new System.Drawing.Point(324, 43);
            this.btnAplicarFiltro.Name = "btnAplicarFiltro";
            this.btnAplicarFiltro.Size = new System.Drawing.Size(76, 24);
            this.btnAplicarFiltro.TabIndex = 5;
            this.btnAplicarFiltro.Text = "Aplicar";
            this.btnAplicarFiltro.UseVisualStyleBackColor = false;
            this.btnAplicarFiltro.Click += new System.EventHandler(this.btnAplicarFiltro_Click);
            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.BackColor = System.Drawing.Color.LightGray;
            this.btnLimpiarFiltros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarFiltros.ForeColor = System.Drawing.Color.Black;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(324, 84);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(76, 24);
            this.btnLimpiarFiltros.TabIndex = 6;
            this.btnLimpiarFiltros.Text = "Limpiar";
            this.btnLimpiarFiltros.UseVisualStyleBackColor = false;
            this.btnLimpiarFiltros.Click += new System.EventHandler(this.btnLimpiarFiltros_Click);
            // 
            // lblEstadisticasTiempoReal
            // 
            this.lblEstadisticasTiempoReal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEstadisticasTiempoReal.AutoSize = true;
            this.lblEstadisticasTiempoReal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadisticasTiempoReal.Location = new System.Drawing.Point(16, 146);
            this.lblEstadisticasTiempoReal.Name = "lblEstadisticasTiempoReal";
            this.lblEstadisticasTiempoReal.Size = new System.Drawing.Size(231, 15);
            this.lblEstadisticasTiempoReal.TabIndex = 0;
            this.lblEstadisticasTiempoReal.Text = "TCP: 0/s | UDP: 0/s | ICMP: 0/s | Total: 0/s";
            // 
            // lblEstadisticasFiltro
            // 
            this.lblEstadisticasFiltro.AutoSize = true;
            this.lblEstadisticasFiltro.Location = new System.Drawing.Point(325, 148);
            this.lblEstadisticasFiltro.Name = "lblEstadisticasFiltro";
            this.lblEstadisticasFiltro.Size = new System.Drawing.Size(161, 13);
            this.lblEstadisticasFiltro.TabIndex = 7;
            this.lblEstadisticasFiltro.Text = "TCP:0 | UDP:0 | ICMP:0 | Total:0";
            // 
            // dgvPaquetes
            // 
            this.dgvPaquetes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPaquetes.BackgroundColor = System.Drawing.Color.White;
            this.dgvPaquetes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvPaquetes.EnableHeadersVisualStyles = false;
            this.dgvPaquetes.Location = new System.Drawing.Point(4, 169);
            this.dgvPaquetes.Name = "dgvPaquetes";
            this.dgvPaquetes.Size = new System.Drawing.Size(670, 325);
            this.dgvPaquetes.TabIndex = 4;
            this.dgvPaquetes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvPaquetes_CellFormatting);
            // 
            // chartTrafico
            // 
            this.chartTrafico.Location = new System.Drawing.Point(680, 298);
            this.chartTrafico.Name = "chartTrafico";
            this.chartTrafico.Size = new System.Drawing.Size(514, 196);
            this.chartTrafico.TabIndex = 23;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(248)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1222, 638);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(774, 482);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Monitorización de Red";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.gbDiagnostico.ResumeLayout(false);
            this.panelScrollDiagnostico.ResumeLayout(false);
            this.gbCaptura.ResumeLayout(false);
            this.gbCaptura.PerformLayout();
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}