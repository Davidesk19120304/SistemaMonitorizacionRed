namespace SistemaMonitorizacionRed
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            // ========================================================================
            // CONTROLES DE CAPTURA
            // ========================================================================
            this.cmbInterfaces = new System.Windows.Forms.ComboBox();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.btnDetener = new System.Windows.Forms.Button();
            this.lblEstadisticas = new System.Windows.Forms.Label();

            // ========================================================================
            // CONTROLES DE FILTRO
            // ========================================================================
            this.cmbProtocolo = new System.Windows.Forms.ComboBox();
            this.txtIPOrigen = new System.Windows.Forms.TextBox();
            this.txtIPDestino = new System.Windows.Forms.TextBox();
            this.btnAplicarFiltro = new System.Windows.Forms.Button();
            this.btnLimpiarFiltros = new System.Windows.Forms.Button();
            this.lblEstadisticasFiltro = new System.Windows.Forms.Label();
            this.labelOrigen = new System.Windows.Forms.Label();
            this.labelDestino = new System.Windows.Forms.Label();

            // ========================================================================
            // VISUALIZACIÓN DE DATOS
            // ========================================================================
            this.dgvPaquetes = new System.Windows.Forms.DataGridView();
            this.chartTrafico = new LiveCharts.WinForms.CartesianChart();
            this.lblEstadisticasTiempoReal = new System.Windows.Forms.Label();

            // ========================================================================
            // CONTROLES DE ALERTAS (configuración)
            // ========================================================================
            this.chkAlertasActivas = new System.Windows.Forms.CheckBox();
            this.txtUmbralEscaneo = new System.Windows.Forms.TextBox();
            this.txtUmbralTrafico = new System.Windows.Forms.TextBox();
            this.btnGuardarConfig = new System.Windows.Forms.Button();
            this.lblUltimaAlerta = new System.Windows.Forms.Label();
            this.btnVerAlertas = new System.Windows.Forms.Button();
            this.labelUmbralEscaneo = new System.Windows.Forms.Label();
            this.labelUmbralTrafico = new System.Windows.Forms.Label();

            // Controles para fuerza bruta
            this.txtUmbralFuerzaBruta = new System.Windows.Forms.TextBox();
            this.txtVentanaFuerzaBruta = new System.Windows.Forms.TextBox();
            this.lblUmbralFuerzaBruta = new System.Windows.Forms.Label();
            this.lblVentanaFuerzaBruta = new System.Windows.Forms.Label();

            // Controles para ICMP flood
            this.lblUmbralICMPFlood = new System.Windows.Forms.Label();
            this.txtUmbralICMPFlood = new System.Windows.Forms.TextBox();
            this.chkICMPFloodActivo = new System.Windows.Forms.CheckBox();

            // Controles para escaneo vertical
            this.lblUmbralEscaneoVertical = new System.Windows.Forms.Label();
            this.txtUmbralEscaneoVertical = new System.Windows.Forms.TextBox();
            this.lblVentanaEscaneoVertical = new System.Windows.Forms.Label();
            this.txtVentanaEscaneoVertical = new System.Windows.Forms.TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).BeginInit();
            this.SuspendLayout();

            // ========================================================================
            // cmbInterfaces (selección de interfaz de red)
            // ========================================================================
            this.cmbInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterfaces.FormattingEnabled = true;
            this.cmbInterfaces.Location = new System.Drawing.Point(20, 20);
            this.cmbInterfaces.Name = "cmbInterfaces";
            this.cmbInterfaces.Size = new System.Drawing.Size(250, 21);
            this.cmbInterfaces.TabIndex = 0;

            // ========================================================================
            // btnIniciar
            // ========================================================================
            this.btnIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciar.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnIniciar.ForeColor = System.Drawing.Color.White;
            this.btnIniciar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnIniciar.Location = new System.Drawing.Point(280, 18);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(100, 25);
            this.btnIniciar.TabIndex = 1;
            this.btnIniciar.Text = "Iniciar Captura";
            this.btnIniciar.UseVisualStyleBackColor = false;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);

            // ========================================================================
            // btnDetener
            // ========================================================================
            this.btnDetener.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDetener.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnDetener.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0);
            this.btnDetener.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDetener.Location = new System.Drawing.Point(390, 18);
            this.btnDetener.Name = "btnDetener";
            this.btnDetener.Size = new System.Drawing.Size(100, 25);
            this.btnDetener.TabIndex = 2;
            this.btnDetener.Text = "Detener";
            this.btnDetener.UseVisualStyleBackColor = false;
            this.btnDetener.Click += new System.EventHandler(this.btnDetener_Click);

            // ========================================================================
            // lblEstadisticas (contador de paquetes)
            // ========================================================================
            this.lblEstadisticas.AutoSize = true;
            this.lblEstadisticas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadisticas.ForeColor = System.Drawing.Color.FromArgb(0, 51, 102);
            this.lblEstadisticas.Location = new System.Drawing.Point(20, 60);
            this.lblEstadisticas.Name = "lblEstadisticas";
            this.lblEstadisticas.Size = new System.Drawing.Size(142, 15);
            this.lblEstadisticas.TabIndex = 3;
            this.lblEstadisticas.Text = "Paquetes capturados: 0";
            this.lblEstadisticas.Click += new System.EventHandler(this.lblEstadisticas_Click);

            // ========================================================================
            // cmbProtocolo (filtro por protocolo)
            // ========================================================================
            this.cmbProtocolo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProtocolo.FormattingEnabled = true;
            this.cmbProtocolo.Location = new System.Drawing.Point(20, 100);
            this.cmbProtocolo.Name = "cmbProtocolo";
            this.cmbProtocolo.Size = new System.Drawing.Size(120, 21);
            this.cmbProtocolo.TabIndex = 4;
            this.cmbProtocolo.SelectedIndexChanged += new System.EventHandler(this.cmbProtocolo_SelectedIndexChanged);

            // ========================================================================
            // txtIPOrigen
            // ========================================================================
            this.txtIPOrigen.Location = new System.Drawing.Point(160, 101);
            this.txtIPOrigen.Name = "txtIPOrigen";
            this.txtIPOrigen.Size = new System.Drawing.Size(120, 20);
            this.txtIPOrigen.TabIndex = 5;
            this.txtIPOrigen.TextChanged += new System.EventHandler(this.txtIPOrigen_TextChanged);

            // ========================================================================
            // txtIPDestino
            // ========================================================================
            this.txtIPDestino.Location = new System.Drawing.Point(300, 101);
            this.txtIPDestino.Name = "txtIPDestino";
            this.txtIPDestino.Size = new System.Drawing.Size(120, 20);
            this.txtIPDestino.TabIndex = 6;

            // ========================================================================
            // btnAplicarFiltro
            // ========================================================================
            this.btnAplicarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarFiltro.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnAplicarFiltro.ForeColor = System.Drawing.Color.White;
            this.btnAplicarFiltro.Location = new System.Drawing.Point(440, 99);
            this.btnAplicarFiltro.Name = "btnAplicarFiltro";
            this.btnAplicarFiltro.Size = new System.Drawing.Size(90, 23);
            this.btnAplicarFiltro.TabIndex = 7;
            this.btnAplicarFiltro.Text = "Aplicar Filtro";
            this.btnAplicarFiltro.UseVisualStyleBackColor = false;
            this.btnAplicarFiltro.Click += new System.EventHandler(this.btnAplicarFiltro_Click);

            // ========================================================================
            // btnLimpiarFiltros
            // ========================================================================
            this.btnLimpiarFiltros.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLimpiarFiltros.BackColor = System.Drawing.Color.LightGray;
            this.btnLimpiarFiltros.Location = new System.Drawing.Point(540, 99);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new System.Drawing.Size(90, 23);
            this.btnLimpiarFiltros.TabIndex = 8;
            this.btnLimpiarFiltros.Text = "Limpiar Filtros";
            this.btnLimpiarFiltros.UseVisualStyleBackColor = false;
            this.btnLimpiarFiltros.Click += new System.EventHandler(this.btnLimpiarFiltros_Click);

            // ========================================================================
            // lblEstadisticasFiltro
            // ========================================================================
            this.lblEstadisticasFiltro.AutoSize = true;
            this.lblEstadisticasFiltro.Location = new System.Drawing.Point(20, 140);
            this.lblEstadisticasFiltro.Name = "lblEstadisticasFiltro";
            this.lblEstadisticasFiltro.Size = new System.Drawing.Size(80, 13);
            this.lblEstadisticasFiltro.TabIndex = 11;
            this.lblEstadisticasFiltro.Text = "Estadísticas: ...";

            // ========================================================================
            // labelOrigen
            // ========================================================================
            this.labelOrigen.AutoSize = true;
            this.labelOrigen.Location = new System.Drawing.Point(160, 85);
            this.labelOrigen.Name = "labelOrigen";
            this.labelOrigen.Size = new System.Drawing.Size(51, 13);
            this.labelOrigen.TabIndex = 9;
            this.labelOrigen.Text = "IP Origen";

            // ========================================================================
            // labelDestino
            // ========================================================================
            this.labelDestino.AutoSize = true;
            this.labelDestino.Location = new System.Drawing.Point(300, 85);
            this.labelDestino.Name = "labelDestino";
            this.labelDestino.Size = new System.Drawing.Size(56, 13);
            this.labelDestino.TabIndex = 10;
            this.labelDestino.Text = "IP Destino";

            // ========================================================================
            // dgvPaquetes (tabla de paquetes capturados)
            // ========================================================================
            this.dgvPaquetes.BackgroundColor = System.Drawing.Color.White;
            this.dgvPaquetes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvPaquetes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.dgvPaquetes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvPaquetes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvPaquetes.EnableHeadersVisualStyles = false;
            this.dgvPaquetes.Location = new System.Drawing.Point(20, 170);
            this.dgvPaquetes.Name = "dgvPaquetes";
            this.dgvPaquetes.Size = new System.Drawing.Size(610, 200);
            this.dgvPaquetes.TabIndex = 12;
            this.dgvPaquetes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPaquetes_CellContentClick);
            this.dgvPaquetes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvPaquetes_CellFormatting);

            // ========================================================================
            // chartTrafico (gráfico en tiempo real)
            // ========================================================================
            this.chartTrafico.Location = new System.Drawing.Point(680, 170);
            this.chartTrafico.Name = "chartTrafico";
            this.chartTrafico.Size = new System.Drawing.Size(450, 200);
            this.chartTrafico.TabIndex = 13;
            this.chartTrafico.Text = "Gráfico de tráfico";

            // ========================================================================
            // lblEstadisticasTiempoReal
            // ========================================================================
            this.lblEstadisticasTiempoReal.AutoSize = true;
            this.lblEstadisticasTiempoReal.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEstadisticasTiempoReal.Location = new System.Drawing.Point(680, 390);
            this.lblEstadisticasTiempoReal.Name = "lblEstadisticasTiempoReal";
            this.lblEstadisticasTiempoReal.Size = new System.Drawing.Size(189, 15);
            this.lblEstadisticasTiempoReal.TabIndex = 14;
            this.lblEstadisticasTiempoReal.Text = "TCP: 0/s | UDP: 0/s | Total: 0/s";

            // ========================================================================
            // CONFIGURACIÓN DE ALERTAS
            // ========================================================================
            // chkAlertasActivas
            this.chkAlertasActivas.AutoSize = true;
            this.chkAlertasActivas.Checked = true;
            this.chkAlertasActivas.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlertasActivas.Location = new System.Drawing.Point(680, 420);
            this.chkAlertasActivas.Name = "chkAlertasActivas";
            this.chkAlertasActivas.Size = new System.Drawing.Size(95, 17);
            this.chkAlertasActivas.TabIndex = 15;
            this.chkAlertasActivas.Text = "Alertas activas";
            this.chkAlertasActivas.UseVisualStyleBackColor = true;

            // labelUmbralEscaneo
            this.labelUmbralEscaneo.AutoSize = true;
            this.labelUmbralEscaneo.Location = new System.Drawing.Point(20, 390);
            this.labelUmbralEscaneo.Name = "labelUmbralEscaneo";
            this.labelUmbralEscaneo.Size = new System.Drawing.Size(131, 13);
            this.labelUmbralEscaneo.TabIndex = 16;
            this.labelUmbralEscaneo.Text = "Umbral escaneo (puertos):";

            // txtUmbralEscaneo
            this.txtUmbralEscaneo.Location = new System.Drawing.Point(163, 387);
            this.txtUmbralEscaneo.Name = "txtUmbralEscaneo";
            this.txtUmbralEscaneo.Size = new System.Drawing.Size(86, 20);
            this.txtUmbralEscaneo.TabIndex = 17;
            this.txtUmbralEscaneo.Text = "10";

            // labelUmbralTrafico
            this.labelUmbralTrafico.AutoSize = true;
            this.labelUmbralTrafico.Location = new System.Drawing.Point(20, 420);
            this.labelUmbralTrafico.Name = "labelUmbralTrafico";
            this.labelUmbralTrafico.Size = new System.Drawing.Size(112, 13);
            this.labelUmbralTrafico.TabIndex = 18;
            this.labelUmbralTrafico.Text = "Umbral tráfico (paq/s):";

            // txtUmbralTrafico
            this.txtUmbralTrafico.Location = new System.Drawing.Point(163, 417);
            this.txtUmbralTrafico.Name = "txtUmbralTrafico";
            this.txtUmbralTrafico.Size = new System.Drawing.Size(86, 20);
            this.txtUmbralTrafico.TabIndex = 19;
            this.txtUmbralTrafico.Text = "500";
            this.txtUmbralTrafico.TextChanged += new System.EventHandler(this.txtUmbralTrafico_TextChanged);

            // ========================================================================
            // CONFIGURACIÓN FUERZA BRUTA
            // ========================================================================
            this.lblUmbralFuerzaBruta.AutoSize = true;
            this.lblUmbralFuerzaBruta.Location = new System.Drawing.Point(20, 450);
            this.lblUmbralFuerzaBruta.Name = "lblUmbralFuerzaBruta";
            this.lblUmbralFuerzaBruta.Size = new System.Drawing.Size(112, 13);
            this.lblUmbralFuerzaBruta.TabIndex = 20;
            this.lblUmbralFuerzaBruta.Text = "fuerza bruta (intentos):";

            this.txtUmbralFuerzaBruta.Location = new System.Drawing.Point(163, 447);
            this.txtUmbralFuerzaBruta.Name = "txtUmbralFuerzaBruta";
            this.txtUmbralFuerzaBruta.Size = new System.Drawing.Size(86, 20);
            this.txtUmbralFuerzaBruta.TabIndex = 21;
            this.txtUmbralFuerzaBruta.Text = "10";

            this.lblVentanaFuerzaBruta.AutoSize = true;
            this.lblVentanaFuerzaBruta.Location = new System.Drawing.Point(20, 480);
            this.lblVentanaFuerzaBruta.Name = "lblVentanaFuerzaBruta";
            this.lblVentanaFuerzaBruta.Size = new System.Drawing.Size(105, 13);
            this.lblVentanaFuerzaBruta.TabIndex = 22;
            this.lblVentanaFuerzaBruta.Text = "Ventana (segundos):";

            this.txtVentanaFuerzaBruta.Location = new System.Drawing.Point(163, 477);
            this.txtVentanaFuerzaBruta.Name = "txtVentanaFuerzaBruta";
            this.txtVentanaFuerzaBruta.Size = new System.Drawing.Size(86, 20);
            this.txtVentanaFuerzaBruta.TabIndex = 23;
            this.txtVentanaFuerzaBruta.Text = "60";

            // ========================================================================
            // CONFIGURACIÓN ICMP FLOOD
            // ========================================================================
            this.lblUmbralICMPFlood.AutoSize = true;
            this.lblUmbralICMPFlood.Location = new System.Drawing.Point(280, 390);
            this.lblUmbralICMPFlood.Name = "lblUmbralICMPFlood";
            this.lblUmbralICMPFlood.Size = new System.Drawing.Size(99, 13);
            this.lblUmbralICMPFlood.TabIndex = 24;
            this.lblUmbralICMPFlood.Text = "ICMP flood (paq/s):";

            this.txtUmbralICMPFlood.Location = new System.Drawing.Point(400, 387);
            this.txtUmbralICMPFlood.Name = "txtUmbralICMPFlood";
            this.txtUmbralICMPFlood.Size = new System.Drawing.Size(86, 20);
            this.txtUmbralICMPFlood.TabIndex = 25;
            this.txtUmbralICMPFlood.Text = "100";

            this.chkICMPFloodActivo.AutoSize = true;
            this.chkICMPFloodActivo.Checked = true;
            this.chkICMPFloodActivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkICMPFloodActivo.Location = new System.Drawing.Point(500, 389);
            this.chkICMPFloodActivo.Name = "chkICMPFloodActivo";
            this.chkICMPFloodActivo.Size = new System.Drawing.Size(56, 17);
            this.chkICMPFloodActivo.TabIndex = 26;
            this.chkICMPFloodActivo.Text = "Activo";

            // ========================================================================
            // CONFIGURACIÓN ESCANEO VERTICAL
            // ========================================================================
            this.lblUmbralEscaneoVertical.AutoSize = true;
            this.lblUmbralEscaneoVertical.Location = new System.Drawing.Point(280, 420);
            this.lblUmbralEscaneoVertical.Name = "lblUmbralEscaneoVertical";
            this.lblUmbralEscaneoVertical.Size = new System.Drawing.Size(113, 13);
            this.lblUmbralEscaneoVertical.TabIndex = 27;
            this.lblUmbralEscaneoVertical.Text = "Escaneo vertical (IPs):";

            this.txtUmbralEscaneoVertical.Location = new System.Drawing.Point(400, 417);
            this.txtUmbralEscaneoVertical.Name = "txtUmbralEscaneoVertical";
            this.txtUmbralEscaneoVertical.Size = new System.Drawing.Size(86, 20);
            this.txtUmbralEscaneoVertical.TabIndex = 28;
            this.txtUmbralEscaneoVertical.Text = "10";

            this.lblVentanaEscaneoVertical.AutoSize = true;
            this.lblVentanaEscaneoVertical.Location = new System.Drawing.Point(280, 450);
            this.lblVentanaEscaneoVertical.Name = "lblVentanaEscaneoVertical";
            this.lblVentanaEscaneoVertical.Size = new System.Drawing.Size(105, 13);
            this.lblVentanaEscaneoVertical.TabIndex = 29;
            this.lblVentanaEscaneoVertical.Text = "Ventana (segundos):";

            this.txtVentanaEscaneoVertical.Location = new System.Drawing.Point(400, 447);
            this.txtVentanaEscaneoVertical.Name = "txtVentanaEscaneoVertical";
            this.txtVentanaEscaneoVertical.Size = new System.Drawing.Size(86, 20);
            this.txtVentanaEscaneoVertical.TabIndex = 30;
            this.txtVentanaEscaneoVertical.Text = "60";

            // ========================================================================
            // BOTONES GENERALES
            // ========================================================================
            // btnGuardarConfig
            this.btnGuardarConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardarConfig.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnGuardarConfig.ForeColor = System.Drawing.Color.White;
            this.btnGuardarConfig.Location = new System.Drawing.Point(20, 520);
            this.btnGuardarConfig.Name = "btnGuardarConfig";
            this.btnGuardarConfig.Size = new System.Drawing.Size(140, 30);
            this.btnGuardarConfig.TabIndex = 31;
            this.btnGuardarConfig.Text = "Guardar Configuración";
            this.btnGuardarConfig.UseVisualStyleBackColor = false;
            this.btnGuardarConfig.Click += new System.EventHandler(this.btnGuardarConfig_Click);

            // btnVerAlertas
            this.btnVerAlertas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerAlertas.BackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.btnVerAlertas.ForeColor = System.Drawing.Color.White;
            this.btnVerAlertas.Location = new System.Drawing.Point(180, 520);
            this.btnVerAlertas.Name = "btnVerAlertas";
            this.btnVerAlertas.Size = new System.Drawing.Size(160, 30);
            this.btnVerAlertas.TabIndex = 32;
            this.btnVerAlertas.Text = "Ver Historial de Alertas";
            this.btnVerAlertas.UseVisualStyleBackColor = false;
            this.btnVerAlertas.Click += new System.EventHandler(this.btnVerAlertas_Click);

            // lblUltimaAlerta
            this.lblUltimaAlerta.AutoSize = true;
            this.lblUltimaAlerta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUltimaAlerta.ForeColor = System.Drawing.Color.Green;
            this.lblUltimaAlerta.Location = new System.Drawing.Point(680, 460);
            this.lblUltimaAlerta.Name = "lblUltimaAlerta";
            this.lblUltimaAlerta.Size = new System.Drawing.Size(84, 15);
            this.lblUltimaAlerta.TabIndex = 33;
            this.lblUltimaAlerta.Text = "✅ Sin alertas";

            // ========================================================================
            // FORMULARIO PRINCIPAL
            // ========================================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 248, 255); // AliceBlue
            this.ClientSize = new System.Drawing.Size(1200, 580);
            this.Controls.Add(this.btnVerAlertas);
            this.Controls.Add(this.lblUltimaAlerta);
            this.Controls.Add(this.btnGuardarConfig);
            this.Controls.Add(this.txtVentanaEscaneoVertical);
            this.Controls.Add(this.lblVentanaEscaneoVertical);
            this.Controls.Add(this.txtUmbralEscaneoVertical);
            this.Controls.Add(this.lblUmbralEscaneoVertical);
            this.Controls.Add(this.chkICMPFloodActivo);
            this.Controls.Add(this.txtUmbralICMPFlood);
            this.Controls.Add(this.lblUmbralICMPFlood);
            this.Controls.Add(this.txtVentanaFuerzaBruta);
            this.Controls.Add(this.lblVentanaFuerzaBruta);
            this.Controls.Add(this.txtUmbralFuerzaBruta);
            this.Controls.Add(this.lblUmbralFuerzaBruta);
            this.Controls.Add(this.txtUmbralTrafico);
            this.Controls.Add(this.labelUmbralTrafico);
            this.Controls.Add(this.txtUmbralEscaneo);
            this.Controls.Add(this.labelUmbralEscaneo);
            this.Controls.Add(this.chkAlertasActivas);
            this.Controls.Add(this.lblEstadisticasTiempoReal);
            this.Controls.Add(this.chartTrafico);
            this.Controls.Add(this.dgvPaquetes);
            this.Controls.Add(this.lblEstadisticasFiltro);
            this.Controls.Add(this.labelDestino);
            this.Controls.Add(this.labelOrigen);
            this.Controls.Add(this.btnLimpiarFiltros);
            this.Controls.Add(this.btnAplicarFiltro);
            this.Controls.Add(this.txtIPDestino);
            this.Controls.Add(this.txtIPOrigen);
            this.Controls.Add(this.cmbProtocolo);
            this.Controls.Add(this.lblEstadisticas);
            this.Controls.Add(this.btnDetener);
            this.Controls.Add(this.btnIniciar);
            this.Controls.Add(this.cmbInterfaces);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Monitorización de Red";
            this.Load += new System.EventHandler(this.Form1_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvPaquetes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ========================================================================
        // DECLARACIÓN DE CONTROLES (accesibles desde el código)
        // ========================================================================

        // Captura
        private System.Windows.Forms.ComboBox cmbInterfaces;
        private System.Windows.Forms.Button btnIniciar;
        private System.Windows.Forms.Button btnDetener;
        private System.Windows.Forms.Label lblEstadisticas;

        // Filtros
        private System.Windows.Forms.ComboBox cmbProtocolo;
        private System.Windows.Forms.TextBox txtIPOrigen;
        private System.Windows.Forms.TextBox txtIPDestino;
        private System.Windows.Forms.Button btnAplicarFiltro;
        private System.Windows.Forms.Button btnLimpiarFiltros;
        private System.Windows.Forms.Label lblEstadisticasFiltro;
        private System.Windows.Forms.Label labelOrigen;
        private System.Windows.Forms.Label labelDestino;

        // Visualización
        private System.Windows.Forms.DataGridView dgvPaquetes;
        private LiveCharts.WinForms.CartesianChart chartTrafico;
        private System.Windows.Forms.Label lblEstadisticasTiempoReal;

        // Alertas generales
        private System.Windows.Forms.CheckBox chkAlertasActivas;
        private System.Windows.Forms.TextBox txtUmbralEscaneo;
        private System.Windows.Forms.TextBox txtUmbralTrafico;
        private System.Windows.Forms.Button btnGuardarConfig;
        private System.Windows.Forms.Label lblUltimaAlerta;
        private System.Windows.Forms.Button btnVerAlertas;
        private System.Windows.Forms.Label labelUmbralEscaneo;
        private System.Windows.Forms.Label labelUmbralTrafico;

        // Fuerza bruta
        private System.Windows.Forms.TextBox txtUmbralFuerzaBruta;
        private System.Windows.Forms.TextBox txtVentanaFuerzaBruta;
        private System.Windows.Forms.Label lblUmbralFuerzaBruta;
        private System.Windows.Forms.Label lblVentanaFuerzaBruta;

        // ICMP flood
        private System.Windows.Forms.Label lblUmbralICMPFlood;
        private System.Windows.Forms.TextBox txtUmbralICMPFlood;
        private System.Windows.Forms.CheckBox chkICMPFloodActivo;

        // Escaneo vertical
        private System.Windows.Forms.Label lblUmbralEscaneoVertical;
        private System.Windows.Forms.TextBox txtUmbralEscaneoVertical;
        private System.Windows.Forms.Label lblVentanaEscaneoVertical;
        private System.Windows.Forms.TextBox txtVentanaEscaneoVertical;
    }
}