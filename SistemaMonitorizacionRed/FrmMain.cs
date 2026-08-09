using LiveCharts;                          // SeriesCollection, LineSeries, Axis, LegendLocation
using LiveCharts.Wpf;                      // CartesianChart (usa internamente System.Windows.Media.Brushes)
using PacketDotNet;                        // Parseo y extracción de paquetes (IP, TCP, UDP, ICMP, IGMP)
using PdfSharp.Drawing;                     // XGraphics, XFont, XBrushes, XPens, XStringFormats, XColor
using PdfSharp.Fonts;                       // IFontResolver, FontResolverInfo, GlobalFontSettings
using PdfSharp.Pdf;                         // PdfDocument, PdfPage, PageSize, PageOrientation
using SharpPcap;                           // Dispositivos de captura y modos de apertura
using SharpPcap.LibPcap;                   // Implementación concreta para Windows (Npcap)
using System;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;
using System.IO;                            // Path, File, StreamWriter
using System.Linq;
using System.Net.NetworkInformation;        // NetworkInterface, Ping, IPStatus
using System.Runtime.InteropServices;       // DllImport, Marshal, StructLayout
using System.Text;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;
using System.Threading.Tasks;

namespace SistemaMonitorizacionRed
{
    public partial class FrmMain : Form
    {
        #region Variables

        // --- Captura de red ---
        private LibPcapLiveDevice dispositivo;          // Interfaz de red seleccionada para captura
        private int contadorPaquetes = 0;               // Total de paquetes capturados en la sesión
        private List<PaqueteInfo> todosLosPaquetes = new List<PaqueteInfo>(); // Búfer en memoria (máx. 1000)

        // --- Series y gráfico ---
        private ChartValues<int> valoresTCP = new ChartValues<int>();
        private ChartValues<int> valoresUDP = new ChartValues<int>();
        private ChartValues<int> valoresICMP = new ChartValues<int>();
        private ChartValues<int> valoresIGMP = new ChartValues<int>();
        private List<string> etiquetasTiempo = new List<string>();
        private int contadorSegundos = 0;
        private Timer timerGrafico;                     // Dispara la actualización del gráfico cada 1s

        private PictureBox logoPictureBox; // logo que cambia con el tema
        // --- Contadores por segundo ---
        private int paquetesPorSegundo = 0;
        private int tcpPorSegundo = 0;
        private int udpPorSegundo = 0;
        private int icmpPorSegundo = 0;
        private int igmpPorSegundo = 0;

        // --- Refresco de la UI ---
        private Timer refreshTimerGrid;
        private int paquetesPendientes = 0;             // Paquetes acumulados entre refrescos

        // --- Alertas ---
        private AlertaHelper alertaHelper = new AlertaHelper();
        private bool alertasActivas = true;

        // --- Detección adaptativa (línea base dinámica) ---
        private Queue<int> historialPaquetes = new Queue<int>();   // Últimos 60 valores de paq/s
        private Queue<int> historialICMP = new Queue<int>();       // Últimos 60 valores de ICMP/s
        private double factorSigma = 3.0;                          // Número de desviaciones para el umbral
        private DateTime ultimaAlertaTrafico = DateTime.MinValue;
        private DateTime ultimaAlertaICMP = DateTime.MinValue;
        private TimeSpan cooldown = TimeSpan.FromSeconds(10);      // Evita alertas repetitivas

        // --- Estructuras para detección de ataques ---
        private Dictionary<string, Dictionary<int, DateTime>> intentosEscaneo = new Dictionary<string, Dictionary<int, DateTime>>();
        private int ventanaEscaneoSegundos = 60;
        private int umbralEscaneo = 10;

        private Dictionary<string, Dictionary<int, List<DateTime>>> intentosFuerzaBruta = new Dictionary<string, Dictionary<int, List<DateTime>>>();
        private int umbralFuerzaBruta = 12;
        private int ventanaFuerzaBrutaSegundos = 60;

        private Dictionary<int, Dictionary<string, List<DateTime>>> intentosEscaneoVertical = new Dictionary<int, Dictionary<string, List<DateTime>>>();
        private int umbralEscaneoVertical = 10;
        private int ventanaEscaneoVerticalSegundos = 60;

        private Timer timerLimpiezaGeneral;             // Limpia estructuras antiguas cada 60s

        // --- Filtros ---
        private string filtroProtocolo = "Todos";
        private string filtroIPOrigen = "";
        private string filtroIPDestino = "";
        private bool esModoOscuro = false;

        // --- Diagnóstico de red ---
        private Timer timerLatencia;
        private Timer timerActualizarDiagnostico;
        private string velocidadEnlaceActual = "No disponible";
        private string bandaWiFiActual = "No conectado";
        private long ultimaLatencia = -1;

        // Control de frecuencia de notificaciones emergentes
        private DateTime ultimaNotificacionEmergente = DateTime.MinValue;
        private TimeSpan intervaloMinimoNotificaciones = TimeSpan.FromSeconds(5);

        // Cola de notificaciones para mostrar en orden
        private Queue<(string mensaje, Color colorFondo, int duracionMs)> colaNotificaciones = new Queue<(string, Color, int)>();
        private Timer timerNotificaciones;
        private bool procesandoCola = false;

        // --- Conexión a BD
        private string connectionString = "Host=localhost;Database=monitorizacion_red;Username=postgres;Password=Theflashtemp*123";

        #endregion

        #region Clase Interna PaqueteInfo

        /// <summary>
        /// Representa un paquete de red capturado con sus campos más relevantes.
        /// </summary>
        private class PaqueteInfo
        {
            public string Hora { get; set; }
            public string IPOrigen { get; set; }
            public string IPDestino { get; set; }
            public string Protocolo { get; set; }
            public int PuertoOrigen { get; set; }
            public int PuertoDestino { get; set; }
            public int Tamaño { get; set; }
            public string InformacionAdicional { get; set; }
        }

        #endregion

        #region Constructor y Configuración Inicial

        public FrmMain(string usuario, string rol)
        {
            // Configurar el resolver de fuentes para PDFsharp (lee de C:\Windows\Fonts)
            GlobalFontSettings.FontResolver = new SystemFontResolver();
            InitializeComponent();
            this.Text = $"Monitorización - Usuario: {usuario} ({rol})";

            AgregarLogo();
            ConfigurarDataGridView();
            ConfigurarGrafico();
            cmbFiltroIPOrigen.KeyPress += TxtIP_KeyPress;
            cmbFiltroIPDestino.KeyPress += TxtIP_KeyPress;

            dgvPaquetes.AllowUserToAddRows = false;
            dgvPaquetes.CellFormatting += DgvPaquetes_CellFormatting;

            // Timer de limpieza de estructuras de detección (cada 60 segundos)
            timerLimpiezaGeneral = new Timer { Interval = 60000 };
            timerLimpiezaGeneral.Tick += (s, e) => LimpiarEstructurasAntiguas();
            timerLimpiezaGeneral.Start();

            // Timer para guardar la latencia en BD cada 60 segundos
            timerLatencia = new Timer { Interval = 60000 };
            timerLatencia.Tick += (s, e) =>
            {
                if (ultimaLatencia != -1)
                    GuardarLatenciaEnBD("8.8.8.8", ultimaLatencia, perdido: false);
            };
            timerLatencia.Start();

            // Timer que actualiza los velocímetros cada 5 segundos
            timerActualizarDiagnostico = new Timer { Interval = 5000 };
            timerActualizarDiagnostico.Tick += (s, e) => ActualizarDiagnostico();
            timerActualizarDiagnostico.Start();

            // Timer para refrescar el DataGridView sin saturar la UI
            refreshTimerGrid = new Timer { Interval = 500 };
            refreshTimerGrid.Tick += (s, e) =>
            {
                if (paquetesPendientes > 0)
                {
                    if (this.InvokeRequired)
                        this.Invoke(new Action(AplicarFiltros));
                    else
                        AplicarFiltros();
                    paquetesPendientes = 0;
                }
            };
            refreshTimerGrid.Start();

            alertasActivas = true;
            factorSigma = 3.0;

            // Timer para procesar la cola de notificaciones cada 5 segundos
            timerNotificaciones = new Timer { Interval = 5000 };
            timerNotificaciones.Tick += (s, e) => ProcesarColaNotificaciones();
        }
        private void TxtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Agrega el logo en el panel superior (headerPanel).
        /// </summary>
        private void AgregarLogo()
        {
            if (headerPanel == null) return;
            logoPictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(130, 130),
                Location = new Point(15, (headerPanel.Height - 130) / 2),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.None
            };
            headerPanel.Controls.Add(logoPictureBox);
            titleLabel.Left = logoPictureBox.Right + 20;

            // Cargar el logo según el tema actual
            ActualizarLogo();
        }
        /// <summary>
        /// Cambia la imagen del logo según el modo oscuro o claro.
        /// </summary>
        private void ActualizarLogo()
        {
            if (logoPictureBox == null) return;

            try
            {
                if (esModoOscuro)
                    logoPictureBox.Image = Properties.Resources.LOGO_OSCURO;   // Imagen para tema oscuro
                else
                    logoPictureBox.Image = Properties.Resources.LOGO_PEQUENO;   // Imagen para tema claro
            }
            catch
            {
                // Si no encuentra las imágenes, intenta cargarlas desde archivo
                string nombreArchivo = esModoOscuro ? "LOGO_OSCURO.png" : "LOGO_PEQUENO.png";
                string ruta = System.IO.Path.Combine(Application.StartupPath, "Resources", nombreArchivo);
                if (System.IO.File.Exists(ruta))
                    logoPictureBox.Image = Image.FromFile(ruta);
            }
        }

        /// <summary>
        /// Configura las series del gráfico de tráfico en tiempo real (TCP, UDP, ICMP, IGMP).
        /// </summary>
        private void ConfigurarGrafico()
        {
            chartTrafico.Series = new SeriesCollection
            {
                new LineSeries { Title = "TCP/s", Values = valoresTCP, PointGeometry = null, StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.DodgerBlue },
                new LineSeries { Title = "UDP/s", Values = valoresUDP, PointGeometry = null, StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.Red },
                new LineSeries { Title = "ICMP/s", Values = valoresICMP, PointGeometry = null, StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.Orange },
                new LineSeries { Title = "IGMP/s", Values = valoresIGMP, PointGeometry = null, StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent, Stroke = System.Windows.Media.Brushes.Purple }
            };

            chartTrafico.AxisX.Add(new Axis { Title = "Tiempo (segundos)", Labels = etiquetasTiempo, DisableAnimations = true });
            chartTrafico.AxisY.Add(new Axis { Title = "Paquetes por segundo", MinValue = 0, DisableAnimations = true });
            chartTrafico.LegendLocation = LegendLocation.Top;

            timerGrafico = new Timer { Interval = 1000 };
            timerGrafico.Tick += TimerGrafico_Tick;
        }

        /// <summary>
        /// Define las columnas del DataGridView que muestra los paquetes capturados.
        /// </summary>
        private void ConfigurarDataGridView()
        {
            dgvPaquetes.Columns.Clear();
            dgvPaquetes.Columns.Add("Hora", "Hora");
            dgvPaquetes.Columns.Add("Origen", "IP Origen");
            dgvPaquetes.Columns.Add("Destino", "IP Destino");
            dgvPaquetes.Columns.Add("Protocolo", "Protocolo");
            dgvPaquetes.Columns.Add("PuertoOrigen", "Puerto Origen");
            dgvPaquetes.Columns.Add("PuertoDestino", "Puerto Destino");
            dgvPaquetes.Columns.Add("Tamaño", "Tamaño (bytes)");
            dgvPaquetes.Columns.Add("Info", "Información");

            dgvPaquetes.Columns["Hora"].Width = 100;
            dgvPaquetes.Columns["Origen"].Width = 150;
            dgvPaquetes.Columns["Destino"].Width = 150;
            dgvPaquetes.Columns["Protocolo"].Width = 120;
            dgvPaquetes.Columns["PuertoOrigen"].Width = 120;
            dgvPaquetes.Columns["PuertoDestino"].Width = 120;
            dgvPaquetes.Columns["Tamaño"].Width = 120;
            dgvPaquetes.Columns["Info"].Width = 300;

            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPaquetes.RowHeadersVisible = false;
        }

        /// <summary>
        /// Implementa IFontResolver para PDFsharp leyendo fuentes desde C:\Windows\Fonts.
        /// </summary>
        public class SystemFontResolver : IFontResolver
        {
            // Mapeo de nombres de fuente a nombres de archivo (sin extensión)
            private static readonly Dictionary<string, string> FamilyToFileName = new Dictionary<string, string>
            {
                { "Arial", "arial" },
                { "Arial Bold", "arialbd" },
                { "Arial Italic", "ariali" },
                { "Arial Bold Italic", "arialbi" },
                { "Segoe UI", "segoeui" },
                { "Segoe UI Bold", "segoeuib" },
                { "Segoe UI Italic", "segoeuii" },
                { "Segoe UI Bold Italic", "seguili" },
                { "Microsoft Sans Serif", "micross" },
                { "Courier New", "cour" },
                { "Times New Roman", "times" }
            };

            public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
            {
                string key = familyName;
                if (isBold && isItalic) key += " Bold Italic";
                else if (isBold) key += " Bold";
                else if (isItalic) key += " Italic";

                if (FamilyToFileName.ContainsKey(key))
                    return new FontResolverInfo(key);
                if (FamilyToFileName.ContainsKey(familyName))
                    return new FontResolverInfo(familyName);
                return new FontResolverInfo("Arial"); // Fallback
            }

            public byte[] GetFont(string faceName)
            {
                if (!FamilyToFileName.TryGetValue(faceName, out string fileName))
                    fileName = faceName.Replace(" ", "").ToLowerInvariant();

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName + ".ttf");
                if (File.Exists(fontPath))
                    return File.ReadAllBytes(fontPath);

                fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), fileName + ".ttc");
                if (File.Exists(fontPath))
                    return File.ReadAllBytes(fontPath);

                throw new FileNotFoundException($"No se encontró el archivo de fuente: {fileName}.ttf/ttc");
            }
        }

        #endregion

        #region Interfaces y Captura

        private void FrmMain_Load(object sender, EventArgs e)
        {
            CargarInterfaces();
            btnDetener.Enabled = false;

            // Cargar las IPs únicas de forma asíncrona para no bloquear el inicio
            cmbFiltroIPOrigen.Text = "Cargando IPs...";
            cmbFiltroIPDestino.Text = "Cargando IPs...";
            cmbFiltroIPOrigen.Enabled = false;
            cmbFiltroIPDestino.Enabled = false;

            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                List<string> ipsOrigen = new List<string>();
                List<string> ipsDestino = new List<string>();

                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString + ";Connect Timeout=5"))
                    {
                        conn.Open();

                        string queryOrigen = "SELECT DISTINCT ip_origen FROM paquetes WHERE ip_origen IS NOT NULL AND ip_origen != '' ORDER BY ip_origen";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(queryOrigen, conn))
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                ipsOrigen.Add(reader["ip_origen"].ToString());
                        }

                        string queryDestino = "SELECT DISTINCT ip_destino FROM paquetes WHERE ip_destino IS NOT NULL AND ip_destino != '' ORDER BY ip_destino";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(queryDestino, conn))
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                ipsDestino.Add(reader["ip_destino"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error cargando IPs: " + ex.Message);
                }
                // Actualizar los ComboBox en el hilo principal
                this.Invoke(new Action(() =>
                {
                    // Limpiar Items
                    cmbFiltroIPOrigen.Items.Clear();
                    cmbFiltroIPDestino.Items.Clear();

                    // Agregar todas las IPs
                    foreach (string ip in ipsOrigen)
                        cmbFiltroIPOrigen.Items.Add(ip);
                    foreach (string ip in ipsDestino)
                        cmbFiltroIPDestino.Items.Add(ip);

                    // Configurar autocompletado usando la propia lista (más fiable)
                    cmbFiltroIPOrigen.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cmbFiltroIPOrigen.AutoCompleteSource = AutoCompleteSource.ListItems;

                    cmbFiltroIPDestino.AutoCompleteMode = AutoCompleteMode.Suggest;
                    cmbFiltroIPDestino.AutoCompleteSource = AutoCompleteSource.ListItems;

                    // Habilitar los ComboBox
                    cmbFiltroIPOrigen.Enabled = true;
                    cmbFiltroIPDestino.Enabled = true;

                    // Limpiar el texto "Cargando IPs..."
                    cmbFiltroIPOrigen.Text = "";
                    cmbFiltroIPDestino.Text = "";
                }));
            });
        }

        /// <summary>
        /// Carga las interfaces de red disponibles mostrando solo la descripción amigable.
        /// </summary>
        private void CargarInterfaces()
        {
            cmbInterfaces.Items.Clear();
            var dispositivos = LibPcapLiveDeviceList.Instance;
            if (dispositivos.Count == 0)
            {
                MessageBox.Show("No se encontraron interfaces de red. Asegúrate de tener Npcap instalado.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIniciar.Enabled = false;
                return;
            }
            foreach (var dev in dispositivos)
                cmbInterfaces.Items.Add(dev.Description);
            if (cmbInterfaces.Items.Count > 0)
                cmbInterfaces.SelectedIndex = 0;
            btnIniciar.Enabled = true;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbInterfaces.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecciona una interfaz de red.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dispositivos = LibPcapLiveDeviceList.Instance;
                string desc = cmbInterfaces.SelectedItem.ToString();

                // Buscar el dispositivo por su descripción (evita usar índice que puede estar desincronizado)
                dispositivo = dispositivos.FirstOrDefault(d => d.Description == desc);
                if (dispositivo == null)
                {
                    MessageBox.Show("No se encontró la interfaz seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dispositivo.Open(DeviceModes.Promiscuous, 1000);
                dispositivo.Filter = "ip";                             // Solo tráfico IP
                dispositivo.OnPacketArrival += Dispositivo_OnPacketArrival;
                dispositivo.StartCapture();

                btnIniciar.Enabled = false;
                btnDetener.Enabled = true;
                cmbInterfaces.Enabled = false;

                todosLosPaquetes.Clear();
                contadorPaquetes = 0;
                valoresTCP.Clear();
                valoresUDP.Clear();
                valoresICMP.Clear();
                valoresIGMP.Clear();
                etiquetasTiempo.Clear();
                contadorSegundos = 0;

                timerGrafico.Start();
                refreshTimerGrid.Start();

                lblEstadisticas.Text = "Capturando... Paquetes: 0";
                ActualizarEstadisticasFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar captura: " + ex.Message);
            }
        }

        private void btnDetener_Click(object sender, EventArgs e) => DetenerCaptura();

        /// <summary>
        /// Detiene la captura de paquetes y restaura los controles.
        /// </summary>
        private void DetenerCaptura()
        {
            timerGrafico.Stop();
            refreshTimerGrid.Stop();
            if (dispositivo != null && dispositivo.Started)
            {
                dispositivo.StopCapture();
                dispositivo.Close();
            }
            btnIniciar.Enabled = true;
            btnDetener.Enabled = false;
            cmbInterfaces.Enabled = true;
            lblEstadisticas.Text = $"Detenido. Total paquetes: {contadorPaquetes}";
        }

        #endregion

        #region Procesamiento de Paquetes

        /// <summary>
        /// Callback invocado por SharpPcap cada vez que llega un paquete.
        /// Decodifica el paquete, lo almacena en BD y en memoria, y ejecuta los detectores de anomalías.
        /// </summary>
        private void Dispositivo_OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var paquete = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                contadorPaquetes++;
                paquetesPorSegundo++;

                var paqueteInfo = new PaqueteInfo
                {
                    Hora = DateTime.Now.ToString("HH:mm:ss.fff"),
                    Tamaño = e.GetPacket().Data.Length
                };

                var ipPacket = paquete.Extract<IPPacket>();
                if (ipPacket != null)
                {
                    paqueteInfo.IPOrigen = ipPacket.SourceAddress.ToString();
                    paqueteInfo.IPDestino = ipPacket.DestinationAddress.ToString();
                    paqueteInfo.Protocolo = ipPacket.Protocol.ToString();

                    if (ipPacket.Protocol == ProtocolType.Tcp)
                    {
                        tcpPorSegundo++;
                        var tcpPacket = paquete.Extract<TcpPacket>();
                        if (tcpPacket != null)
                        {
                            paqueteInfo.PuertoOrigen = tcpPacket.SourcePort;
                            paqueteInfo.PuertoDestino = tcpPacket.DestinationPort;
                            paqueteInfo.InformacionAdicional = $"TCP: Flags={tcpPacket.Flags}";

                            // Ejecutar detectores de ataques basados en TCP
                            DetectarEscaneoPuertosMejorado(paqueteInfo.IPOrigen, tcpPacket.DestinationPort);
                            DetectarFuerzaBruta(paqueteInfo.IPOrigen, tcpPacket.DestinationPort);
                            DetectarEscaneoVertical(paqueteInfo.IPOrigen, tcpPacket.DestinationPort);
                        }
                    }
                    else if (ipPacket.Protocol == ProtocolType.Udp)
                    {
                        udpPorSegundo++;
                        var udpPacket = paquete.Extract<UdpPacket>();
                        if (udpPacket != null)
                        {
                            paqueteInfo.PuertoOrigen = udpPacket.SourcePort;
                            paqueteInfo.PuertoDestino = udpPacket.DestinationPort;
                            paqueteInfo.InformacionAdicional = "UDP";
                        }
                    }
                    else if (ipPacket.Protocol == ProtocolType.Icmp)
                    {
                        icmpPorSegundo++;
                        var icmpV4 = paquete.Extract<IcmpV4Packet>();
                        if (icmpV4 != null)
                            paqueteInfo.InformacionAdicional = $"ICMPv4 Type={icmpV4.TypeCode}";
                        else
                        {
                            var icmpV6 = paquete.Extract<IcmpV6Packet>();
                            if (icmpV6 != null)
                                paqueteInfo.InformacionAdicional = $"ICMPv6 Type={icmpV6.Type}";
                            else
                                paqueteInfo.InformacionAdicional = "ICMP (otros)";
                        }
                    }
                    else if (ipPacket.Protocol == ProtocolType.Igmp)
                    {
                        igmpPorSegundo++;
                        paqueteInfo.InformacionAdicional = "IGMP";
                    }
                }

                _ = GuardarPaqueteEnBDAsync(paqueteInfo);

                // Insertar al inicio de la lista en memoria (más reciente primero)
                lock (todosLosPaquetes)
                {
                    todosLosPaquetes.Insert(0, paqueteInfo);
                    if (todosLosPaquetes.Count > 1000)
                        todosLosPaquetes.RemoveAt(todosLosPaquetes.Count - 1);
                }

                ActualizarUISeguro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error procesando paquete: " + ex.Message);
            }
        }

        /// <summary>
        /// Guarda un paquete en la base de datos PostgreSQL de forma asíncrona.
        /// </summary>
        private async Task GuardarPaqueteEnBDAsync(PaqueteInfo p)
        {
            await Task.Run(() =>
            {
                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"INSERT INTO paquetes (hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional) 
                                 VALUES (@hora, @ip_origen, @ip_destino, @protocolo, @puerto_origen, @puerto_destino, @tamaño, @info)";
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@hora", DateTime.Parse(p.Hora));
                            cmd.Parameters.AddWithValue("@ip_origen", p.IPOrigen ?? "");
                            cmd.Parameters.AddWithValue("@ip_destino", p.IPDestino ?? "");
                            cmd.Parameters.AddWithValue("@protocolo", p.Protocolo ?? "");
                            cmd.Parameters.AddWithValue("@puerto_origen", p.PuertoOrigen > 0 ? p.PuertoOrigen : 0);
                            cmd.Parameters.AddWithValue("@puerto_destino", p.PuertoDestino > 0 ? p.PuertoDestino : 0);
                            cmd.Parameters.AddWithValue("@tamaño", p.Tamaño);
                            cmd.Parameters.AddWithValue("@info", p.InformacionAdicional ?? "");
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error guardando en BD: " + ex.Message);
                }
            });
        }

        /// <summary>
        /// Actualiza la UI de forma segura desde hilos secundarios.
        /// </summary>
        private void ActualizarUISeguro()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ActualizarUISeguro));
                return;
            }
            lblEstadisticas.Text = $"Capturando... Paquetes: {contadorPaquetes}";
            paquetesPendientes++;
        }

        #endregion

        #region Timer y Estadísticas en Tiempo Real

        /// <summary>
        /// Se ejecuta cada segundo: actualiza las series del gráfico y calcula detección adaptativa.
        /// </summary>
        private void TimerGrafico_Tick(object sender, EventArgs e)
        {
            valoresTCP.Add(tcpPorSegundo);
            valoresUDP.Add(udpPorSegundo);
            valoresICMP.Add(icmpPorSegundo);
            valoresIGMP.Add(igmpPorSegundo);
            etiquetasTiempo.Add(contadorSegundos.ToString());

            // Mantener solo los últimos 30 valores
            if (valoresTCP.Count > 30) valoresTCP.RemoveAt(0);
            if (valoresUDP.Count > 30) valoresUDP.RemoveAt(0);
            if (valoresICMP.Count > 30) valoresICMP.RemoveAt(0);
            if (valoresIGMP.Count > 30) valoresIGMP.RemoveAt(0);
            if (etiquetasTiempo.Count > 30) etiquetasTiempo.RemoveAt(0);

            if (chartTrafico.Series.Count >= 4)
            {
                chartTrafico.Series[0].Values = valoresTCP;
                chartTrafico.Series[1].Values = valoresUDP;
                chartTrafico.Series[2].Values = valoresICMP;
                chartTrafico.Series[3].Values = valoresIGMP;
            }
            if (chartTrafico.AxisX.Count > 0)
                chartTrafico.AxisX[0].Labels = etiquetasTiempo;

            ActualizarEstadisticasTiempoReal();

            // Alimentar colas de historial para detección adaptativa
            historialPaquetes.Enqueue(paquetesPorSegundo);
            if (historialPaquetes.Count > 60) historialPaquetes.Dequeue();
            historialICMP.Enqueue(icmpPorSegundo);
            if (historialICMP.Count > 60) historialICMP.Dequeue();

            DetectarPicoTraficoAdaptativo();
            DetectarICMPFloodAdaptativo();

            // Reiniciar contadores por segundo
            paquetesPorSegundo = 0;
            tcpPorSegundo = 0;
            udpPorSegundo = 0;
            icmpPorSegundo = 0;
            igmpPorSegundo = 0;
            contadorSegundos++;
        }

        private void ActualizarEstadisticasTiempoReal()
        {
            if (lblEstadisticasTiempoReal.InvokeRequired)
                lblEstadisticasTiempoReal.Invoke(new Action(ActualizarEstadisticasTiempoReal));
            else
                lblEstadisticasTiempoReal.Text = $"TCP: {tcpPorSegundo}/s | UDP: {udpPorSegundo}/s | ICMP: {icmpPorSegundo}/s | IGMP: {igmpPorSegundo}/s | Total: {paquetesPorSegundo}/s";
        }

        #endregion

        #region Detección de Anomalías

        /// <summary>
        /// Detecta picos de tráfico comparando el valor actual con la media + factorSigma * desviación.
        /// </summary>
        private void DetectarPicoTraficoAdaptativo()
        {
            if (!alertasActivas || historialPaquetes.Count < 10) return;

            double media = historialPaquetes.Average();
            double desviacion = Math.Sqrt(historialPaquetes.Select(v => Math.Pow(v - media, 2)).Average());
            double umbral = media + factorSigma * desviacion;

            if (paquetesPorSegundo > umbral && paquetesPorSegundo > 50 &&
                DateTime.Now - ultimaAlertaTrafico > cooldown)
            {
                string desc = $"Pico anómalo detectado: {paquetesPorSegundo} paq/s (media={media:F1}, sigma={desviacion:F1})";
                _ = alertaHelper.GuardarAlertaAsync("PICO_ADAPTATIVO", desc, "Media", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                ultimaAlertaTrafico = DateTime.Now;
            }
        }

        /// <summary>
        /// Detecta inundaciones ICMP usando el mismo principio adaptativo.
        /// </summary>
        private void DetectarICMPFloodAdaptativo()
        {
            if (!alertasActivas || historialICMP.Count < 10) return;

            double media = historialICMP.Average();
            double desviacion = Math.Sqrt(historialICMP.Select(v => Math.Pow(v - media, 2)).Average());
            double umbral = media + factorSigma * desviacion;

            if (icmpPorSegundo > umbral && icmpPorSegundo > 10 &&
                DateTime.Now - ultimaAlertaICMP > cooldown)
            {
                string desc = $"ICMP flood anómalo: {icmpPorSegundo} icmp/s (media={media:F1}, sigma={desviacion:F1})";
                _ = alertaHelper.GuardarAlertaAsync("ICMP_FLOOD_ADAPT", desc, "Media", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                ultimaAlertaICMP = DateTime.Now;
            }
        }

        /// <summary>
        /// Detecta escaneo horizontal: una IP contacta muchos puertos distintos en poco tiempo.
        /// </summary>
        private void DetectarEscaneoPuertosMejorado(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;
            DateTime ahora = DateTime.Now;
            if (!intentosEscaneo.ContainsKey(ipOrigen))
                intentosEscaneo[ipOrigen] = new Dictionary<int, DateTime>();
            intentosEscaneo[ipOrigen][puertoDestino] = ahora;

            var viejos = intentosEscaneo[ipOrigen].Where(kvp => (ahora - kvp.Value).TotalSeconds > ventanaEscaneoSegundos).Select(kvp => kvp.Key).ToList();
            foreach (var p in viejos) intentosEscaneo[ipOrigen].Remove(p);

            if (intentosEscaneo[ipOrigen].Count >= umbralEscaneo)
            {
                string desc = $"Posible escaneo de puertos desde {ipOrigen}. Puertos distintos: {intentosEscaneo[ipOrigen].Count}.";
                _ = alertaHelper.GuardarAlertaAsync("ESCANEO_PUERTOS", desc, "Alta", ipOrigen);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosEscaneo[ipOrigen].Clear();
            }
        }

        /// <summary>
        /// Detecta fuerza bruta: muchos intentos a puertos sensibles (SSH, FTP, RDP...).
        /// </summary>
        private void DetectarFuerzaBruta(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;
            int[] puertosSospechosos = { 21, 22, 23, 3389, 5900, 1433, 3306, 8080 };
            if (!puertosSospechosos.Contains(puertoDestino)) return;

            if (!intentosFuerzaBruta.ContainsKey(ipOrigen))
                intentosFuerzaBruta[ipOrigen] = new Dictionary<int, List<DateTime>>();
            if (!intentosFuerzaBruta[ipOrigen].ContainsKey(puertoDestino))
                intentosFuerzaBruta[ipOrigen][puertoDestino] = new List<DateTime>();

            intentosFuerzaBruta[ipOrigen][puertoDestino].Add(DateTime.Now);
            intentosFuerzaBruta[ipOrigen][puertoDestino].RemoveAll(t => (DateTime.Now - t).TotalSeconds > ventanaFuerzaBrutaSegundos);

            if (intentosFuerzaBruta[ipOrigen][puertoDestino].Count >= umbralFuerzaBruta)
            {
                string desc = $"Posible ataque de fuerza bruta desde {ipOrigen} al puerto {puertoDestino}. " +
                              $"{intentosFuerzaBruta[ipOrigen][puertoDestino].Count} intentos en {ventanaFuerzaBrutaSegundos}s.";
                _ = alertaHelper.GuardarAlertaAsync("FUERZA_BRUTA", desc, "Alta", ipOrigen);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosFuerzaBruta[ipOrigen][puertoDestino].Clear();
            }
        }

        /// <summary>
        /// Detecta escaneo vertical: muchas IPs contactan un mismo puerto.
        /// </summary>
        private void DetectarEscaneoVertical(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;
            if (!intentosEscaneoVertical.ContainsKey(puertoDestino))
                intentosEscaneoVertical[puertoDestino] = new Dictionary<string, List<DateTime>>();
            if (!intentosEscaneoVertical[puertoDestino].ContainsKey(ipOrigen))
                intentosEscaneoVertical[puertoDestino][ipOrigen] = new List<DateTime>();

            intentosEscaneoVertical[puertoDestino][ipOrigen].Add(DateTime.Now);
            intentosEscaneoVertical[puertoDestino][ipOrigen].RemoveAll(t => (DateTime.Now - t).TotalSeconds > ventanaEscaneoVerticalSegundos);
            if (intentosEscaneoVertical[puertoDestino][ipOrigen].Count == 0)
                intentosEscaneoVertical[puertoDestino].Remove(ipOrigen);

            int ipDistintas = intentosEscaneoVertical[puertoDestino].Keys.Count;
            if (ipDistintas >= umbralEscaneoVertical)
            {
                string desc = $"Posible escaneo vertical al puerto {puertoDestino}: {ipDistintas} IPs diferentes en {ventanaEscaneoVerticalSegundos}s.";
                _= alertaHelper.GuardarAlertaAsync("ESCANEO_VERTICAL", desc, "Alta", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosEscaneoVertical[puertoDestino].Clear();
            }
        }

        /// <summary>
        /// Elimina entradas antiguas de las estructuras de detección para liberar memoria.
        /// </summary>
        private void LimpiarEstructurasAntiguas()
        {
            DateTime limite = DateTime.Now.AddSeconds(-Math.Max(ventanaEscaneoSegundos, Math.Max(ventanaFuerzaBrutaSegundos, ventanaEscaneoVerticalSegundos)) - 10);
            foreach (var ip in intentosEscaneo.Keys.ToList())
            {
                var viejos = intentosEscaneo[ip].Where(kvp => kvp.Value < limite).Select(kvp => kvp.Key).ToList();
                foreach (var p in viejos) intentosEscaneo[ip].Remove(p);
                if (intentosEscaneo[ip].Count == 0) intentosEscaneo.Remove(ip);
            }
            foreach (var ip in intentosFuerzaBruta.Keys.ToList())
            {
                foreach (var puerto in intentosFuerzaBruta[ip].Keys.ToList())
                {
                    intentosFuerzaBruta[ip][puerto].RemoveAll(t => t < limite);
                    if (intentosFuerzaBruta[ip][puerto].Count == 0) intentosFuerzaBruta[ip].Remove(puerto);
                }
                if (intentosFuerzaBruta[ip].Count == 0) intentosFuerzaBruta.Remove(ip);
            }
            foreach (var puerto in intentosEscaneoVertical.Keys.ToList())
            {
                foreach (var ip in intentosEscaneoVertical[puerto].Keys.ToList())
                {
                    intentosEscaneoVertical[puerto][ip].RemoveAll(t => t < limite);
                    if (intentosEscaneoVertical[puerto][ip].Count == 0) intentosEscaneoVertical[puerto].Remove(ip);
                }
                if (intentosEscaneoVertical[puerto].Count == 0) intentosEscaneoVertical.Remove(puerto);
            }
        }

        #endregion

        #region Filtros y UI

        /// <summary>
        /// Aplica los filtros de protocolo, IP origen e IP destino sobre la lista en memoria.
        /// </summary>
        private void AplicarFiltros()
        {
            List<PaqueteInfo> copia;
            lock (todosLosPaquetes)
            {
                copia = new List<PaqueteInfo>(todosLosPaquetes);
            }

            string filtroProto = cmbProtocolo.SelectedItem?.ToString() ?? "Todos";
            string filtroOrigen = cmbFiltroIPOrigen.Text.Trim();
            string filtroDestino = cmbFiltroIPDestino.Text.Trim();

            var paquetesFiltrados = copia.AsEnumerable();

            if (filtroProto != "Todos")
                paquetesFiltrados = paquetesFiltrados.Where(p => p.Protocolo != null && p.Protocolo.Equals(filtroProto, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filtroOrigen))
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPOrigen != null && p.IPOrigen.StartsWith(filtroOrigen, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filtroDestino))
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPDestino != null && p.IPDestino.StartsWith(filtroDestino, StringComparison.OrdinalIgnoreCase));

            MostrarPaquetesEnGrid(paquetesFiltrados.ToList());
            ActualizarEstadisticasFiltro(copia);
        }

        private void MostrarPaquetesEnGrid(List<PaqueteInfo> paquetes)
        {
            dgvPaquetes.Rows.Clear();
            foreach (var p in paquetes.Take(100))
            {
                dgvPaquetes.Rows.Add(
                    p.Hora, p.IPOrigen ?? "N/A", p.IPDestino ?? "N/A",
                    p.Protocolo ?? "N/A",
                    p.PuertoOrigen > 0 ? p.PuertoOrigen.ToString() : "-",
                    p.PuertoDestino > 0 ? p.PuertoDestino.ToString() : "-",
                    p.Tamaño, p.InformacionAdicional ?? "");
            }
        }

        private void ActualizarEstadisticasFiltro(List<PaqueteInfo> copia)
        {
            int tcp = copia.Count(p => p.Protocolo == "TCP");
            int udp = copia.Count(p => p.Protocolo == "UDP");
            int icmp = copia.Count(p => p.Protocolo == "ICMP");
            int igmp = copia.Count(p => p.Protocolo == "IGMP");
            int total = copia.Count;

            if (lblEstadisticasFiltro.InvokeRequired)
                lblEstadisticasFiltro.Invoke(new Action(() =>
                    lblEstadisticasFiltro.Text = $"TCP: {tcp} | UDP: {udp} | ICMP: {icmp} | IGMP: {igmp} | Total: {total}"));
        }

        private void ActualizarEstadisticasFiltro()
        {
            lock (todosLosPaquetes)
            {
                ActualizarEstadisticasFiltro(new List<PaqueteInfo>(todosLosPaquetes));
            }
        }

        /// <summary>
        /// Colorea las filas según el protocolo y puerto (HTTP, HTTPS, SSH/Telnet).
        /// </summary>
        private void DgvPaquetes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value != null)
            {
                string protocolo = dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value.ToString();
                string puerto = dgvPaquetes.Rows[e.RowIndex].Cells["PuertoDestino"].Value?.ToString() ?? "0";

                if (protocolo == "TCP" && puerto == "80")
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue;
                else if (protocolo == "TCP" && puerto == "443")
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (protocolo == "TCP" && (puerto == "22" || puerto == "23"))
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
            }
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            cmbProtocolo.SelectedIndex = 0;
            cmbFiltroIPOrigen.Text = "";
            cmbFiltroIPDestino.Text = "";
            AplicarFiltros();
        }

        #endregion

        #region Configuración

        private void BtnConfig_Click(object sender, EventArgs e)
        {
            using (FrmConfiguracionAlertas frm = new FrmConfiguracionAlertas())
            {
                frm.AlertasActivas = alertasActivas;
                frm.FactorSigma = factorSigma;
                frm.CargarValores();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    alertasActivas = frm.AlertasActivas;
                    factorSigma = frm.FactorSigma;
                    MostrarNotificacionEmergente("✅ Configuración guardada", Color.FromArgb(0, 120, 100), 2000);
                }
            }
        }
        private void cuentaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtener el usuario logueado del título del formulario
            string usuarioActual = this.Text.Replace("Monitorización - Usuario: ", "").Split('(')[0].Trim();

            // =============================================
            // VERIFICAR CONTRASEÑA ACTUAL
            // =============================================
            string claveActual = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese su contraseña actual:", "Verificación de seguridad", "", -1, -1);

            if (string.IsNullOrEmpty(claveActual))
                return; // Solo aquí se sale si el usuario cancela

            string hashActual = SHA256(claveActual);

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM usuarios WHERE usuario = @usr AND contraseña = @hash";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usr", usuarioActual);
                        cmd.Parameters.AddWithValue("@hash", hashActual);

                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        {
                            MessageBox.Show("Contraseña actual incorrecta.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar la contraseña: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // =============================================
            // CAMBIAR CONTRASEÑA (con reintentos)
            // =============================================
            string nuevaClave = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese la nueva contraseña:", "Cambiar Contraseña", "", -1, -1);

            if (!string.IsNullOrEmpty(nuevaClave))
            {
                bool confirmada = false;

                while (!confirmada)
                {
                    string confirmarClave = Microsoft.VisualBasic.Interaction.InputBox(
                        "Confirme la nueva contraseña:", "Cambiar Contraseña", "", -1, -1);

                    if (string.IsNullOrEmpty(confirmarClave))
                        break; // Canceló la confirmación, salir del bucle

                    if (nuevaClave == confirmarClave)
                    {
                        confirmada = true;

                        string nuevoHash = SHA256(nuevaClave);

                        try
                        {
                            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                            {
                                conn.Open();
                                string query = "UPDATE usuarios SET contraseña = @hash WHERE usuario = @usr";
                                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@hash", nuevoHash);
                                    cmd.Parameters.AddWithValue("@usr", usuarioActual);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            MessageBox.Show("Contraseña cambiada exitosamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al cambiar la contraseña: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Las contraseñas no coinciden. Intente nuevamente.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // =============================================
            // CAMBIAR NOMBRE DE USUARIO (SIEMPRE SE PREGUNTA)
            // =============================================
            string nuevoUsuario = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese el nuevo nombre de usuario (deje vacío para no cambiarlo):",
                "Cambiar Usuario", "", -1, -1);

            if (string.IsNullOrEmpty(nuevoUsuario))
                return;

            if (nuevoUsuario == usuarioActual)
            {
                MessageBox.Show("El nuevo nombre de usuario es igual al actual.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM usuarios WHERE usuario = @nuevo";
                    using (NpgsqlCommand cmdCheck = new NpgsqlCommand(checkQuery, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@nuevo", nuevoUsuario);
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("El nombre de usuario ya está en uso.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string updateQuery = "UPDATE usuarios SET usuario = @nuevo WHERE usuario = @actual";
                    using (NpgsqlCommand cmdUpdate = new NpgsqlCommand(updateQuery, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@nuevo", nuevoUsuario);
                        cmdUpdate.Parameters.AddWithValue("@actual", usuarioActual);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario cambiado exitosamente. Deberá iniciar sesión nuevamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar el usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar SHA256 (agrégalo si no lo tienes ya en FrmMain)
        private string SHA256(string texto)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(texto));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
        private void cambiarPINToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string usuarioActual = this.Text.Replace("Monitorización - Usuario: ", "").Split('(')[0].Trim();

            string pinActual = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese su PIN actual:", "Cambiar PIN", "", -1, -1);

            if (string.IsNullOrEmpty(pinActual) || pinActual.Length != 4 || !int.TryParse(pinActual, out _))
            {
                MessageBox.Show("PIN inválido. Debe ser exactamente 4 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string hashActual = SHA256(pinActual);

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT pin FROM usuarios WHERE usuario = @usr";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usr", usuarioActual);
                        string hashGuardado = cmd.ExecuteScalar()?.ToString();

                        if (hashActual != hashGuardado)
                        {
                            MessageBox.Show("PIN actual incorrecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar el PIN: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string nuevoPin = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el nuevo PIN (4 dígitos):", "Cambiar PIN", "", -1, -1);

            if (string.IsNullOrEmpty(nuevoPin) || nuevoPin.Length != 4 || !int.TryParse(nuevoPin, out _))
            {
                MessageBox.Show("El nuevo PIN debe ser exactamente 4 dígitos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string confirmarPin = Microsoft.VisualBasic.Interaction.InputBox("Confirme el nuevo PIN:", "Cambiar PIN", "", -1, -1);
            if (nuevoPin != confirmarPin)
            {
                MessageBox.Show("Los PIN no coinciden.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string nuevoHash = SHA256(nuevoPin);
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE usuarios SET pin = @hash WHERE usuario = @usr";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", nuevoHash);
                        cmd.Parameters.AddWithValue("@usr", usuarioActual);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("PIN cambiado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar el PIN: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Exportación (CSV y PDF con rango de fechas)

        /// <summary>
        /// Abre el diálogo de selección de fechas y luego el de guardar archivo para exportar paquetes.
        /// </summary>
        private void ExportarPaquetesMenu_Click(object sender, EventArgs e)
        {
            // 1. Seleccionar rango de fechas
            using (FrmExportarFechas frmFechas = new FrmExportarFechas("Paquetes"))
            {
                if (frmFechas.ShowDialog() != DialogResult.OK || !frmFechas.Aceptado)
                    return;

                // 2. Seleccionar ubicación y formato
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Files (*.pdf)|*.pdf|Excel (.xls)|*.xls|CSV Files (*.csv)|*.csv";
                    sfd.Title = "Exportar Paquetes";
                    sfd.FileName = $"paquetes_{frmFechas.FechaDesde:yyyyMMdd}_{frmFechas.FechaHasta:yyyyMMdd}";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLower();
                        if (extension == ".csv")
                            ExportarPaquetesACSV(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else if (extension == ".pdf")
                            ExportarPaquetesAPDF(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else if (extension == ".xls")
                            ExportarPaquetesAExcel(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else
                            MostrarNotificacionEmergente("❌ Formato no soportado.", Color.FromArgb(200, 50, 50), 3000);
                    }
                }
            }
        }

        /// <summary>
        /// Abre el diálogo de selección de fechas y luego el de guardar archivo para exportar alertas.
        /// </summary>
        private void ExportarAlertasMenu_Click(object sender, EventArgs e)
        {
            // 1. Seleccionar rango de fechas
            using (FrmExportarFechas frmFechas = new FrmExportarFechas("Alertas"))
            {
                if (frmFechas.ShowDialog() != DialogResult.OK || !frmFechas.Aceptado)
                    return;

                // 2. Seleccionar ubicación y formato
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF Files (*.pdf)|*.pdf|Excel (.xls)|*.xls|CSV Files (*.csv)|*.csv";
                    sfd.Title = "Exportar Alertas";
                    sfd.FileName = $"alertas_{frmFechas.FechaDesde:yyyyMMdd}_{frmFechas.FechaHasta:yyyyMMdd}";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string extension = Path.GetExtension(sfd.FileName).ToLower();
                        if (extension == ".csv")
                            ExportarAlertasACSV(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else if (extension == ".pdf")
                            ExportarAlertasAPDF(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else if (extension == ".xls")
                            ExportarAlertasAExcel(sfd.FileName, frmFechas.FechaDesde, frmFechas.FechaHasta);
                        else
                            MostrarNotificacionEmergente("❌ Formato no soportado.", Color.FromArgb(200, 50, 50), 3000);
                    }
                }
            }
        }
        /// <summary>
        /// Exporta los paquetes capturados a un archivo Excel (.xlsx) con formato visual profesional.
        /// Compatible con WPS Office y Microsoft Excel.
        /// </summary>
        private void ExportarPaquetesAExcel(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional 
                             FROM paquetes WHERE hora BETWEEN @desde AND @hasta ORDER BY hora DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            using (StreamWriter sw = new StreamWriter(ruta, false, Encoding.UTF8))
                            {
                                // Escribir en formato HTML que Excel y WPS pueden abrir como tabla
                                sw.WriteLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>");
                                sw.WriteLine("<head>");
                                sw.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>");
                                sw.WriteLine("<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>");
                                sw.WriteLine("<x:Name>Paquetes</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>");
                                sw.WriteLine("</x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->");
                                sw.WriteLine("<style>");
                                sw.WriteLine("td { border: 1px solid #C8C8C8; padding: 4px; font-family: 'Segoe UI', Arial; font-size: 10pt; }");
                                sw.WriteLine("th { background-color: #0066CC; color: white; font-weight: bold; padding: 6px; text-align: center; }");
                                sw.WriteLine(".title { font-size: 16pt; font-weight: bold; color: #0066CC; text-align: center; padding: 10px; }");
                                sw.WriteLine(".subtitle { font-size: 12pt; font-weight: bold; color: #0066CC; text-align: center; }");
                                sw.WriteLine(".info { text-align: center; font-size: 9pt; color: #404040; }");
                                sw.WriteLine(".even { background-color: #F0F8FF; }");
                                sw.WriteLine(".odd { background-color: #FFFFFF; }");
                                sw.WriteLine("</style>");
                                sw.WriteLine("</head><body>");

                                // Tabla principal
                                sw.WriteLine("<table border='0' cellspacing='0' cellpadding='0'>");

                                // Título
                                sw.WriteLine("<tr><td colspan='8' class='title'>SISTEMA DE MONITORIZACIÓN DE RED</td></tr>");
                                sw.WriteLine("<tr><td colspan='8' class='subtitle'>Reporte de Paquetes Capturados</td></tr>");
                                sw.WriteLine($"<tr><td colspan='8' class='info'>Período: {fechaDesde:dd/MM/yyyy HH:mm} — {fechaHasta:dd/MM/yyyy HH:mm}</td></tr>");
                                sw.WriteLine($"<tr><td colspan='8' class='info'>Exportado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</td></tr>");
                                sw.WriteLine("<tr><td colspan='8' style='height:10px;'></td></tr>");

                                // Encabezados
                                sw.WriteLine("<tr>");
                                sw.WriteLine("<th>Hora</th><th>IP Origen</th><th>IP Destino</th><th>Protocolo</th><th>Puerto Origen</th><th>Puerto Destino</th><th>Tamaño (bytes)</th><th>Información Adicional</th>");
                                sw.WriteLine("</tr>");

                                // Datos
                                int row = 0;
                                while (reader.Read())
                                {
                                    string rowClass = (row % 2 == 0) ? "even" : "odd";
                                    sw.WriteLine($"<tr class='{rowClass}'>");
                                    sw.WriteLine($"<td>{reader["hora"]}</td>");
                                    sw.WriteLine($"<td>{reader["ip_origen"]}</td>");
                                    sw.WriteLine($"<td>{reader["ip_destino"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["protocolo"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["puerto_origen"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["puerto_destino"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["tamaño"]}</td>");
                                    sw.WriteLine($"<td>{reader["informacion_adicional"]}</td>");
                                    sw.WriteLine("</tr>");
                                    row++;
                                }

                                sw.WriteLine("</table>");
                                sw.WriteLine("</body></html>");
                            }
                        }
                    }
                }
                // Renombrar a .xls para que WPS y Excel lo reconozcan como hoja de cálculo
                if (ruta.EndsWith(".xlsx"))
                {
                    string nuevaRuta = ruta.Replace(".xlsx", ".xls");
                    File.Move(ruta, nuevaRuta);
                    MostrarNotificacionEmergente("✅ Paquetes exportados a Excel (.xls)", Color.FromArgb(0, 120, 100), 2000);
                }
                else
                {
                    MostrarNotificacionEmergente("✅ Paquetes exportados a Excel", Color.FromArgb(0, 120, 100), 2000);
                }
            }
            catch (Exception ex)
            {
                MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000);
            }
        }

        /// <summary>
        /// Exporta el historial de alertas a un archivo Excel (.xlsx) con formato visual profesional.
        /// Compatible con WPS Office y Microsoft Excel.
        /// </summary>
        private void ExportarAlertasAExcel(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT tipo, descripcion, severidad, ip_involucrada, ""timestamp"" 
                             FROM alertas WHERE ""timestamp"" BETWEEN @desde AND @hasta ORDER BY timestamp DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            using (StreamWriter sw = new StreamWriter(ruta, false, Encoding.UTF8))
                            {
                                sw.WriteLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>");
                                sw.WriteLine("<head>");
                                sw.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>");
                                sw.WriteLine("<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>");
                                sw.WriteLine("<x:Name>Alertas</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>");
                                sw.WriteLine("</x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->");
                                sw.WriteLine("<style>");
                                sw.WriteLine("td { border: 1px solid #C8C8C8; padding: 4px; font-family: 'Segoe UI', Arial; font-size: 10pt; }");
                                sw.WriteLine("th { background-color: #0066CC; color: white; font-weight: bold; padding: 6px; text-align: center; }");
                                sw.WriteLine(".title { font-size: 16pt; font-weight: bold; color: #0066CC; text-align: center; padding: 10px; }");
                                sw.WriteLine(".subtitle { font-size: 12pt; font-weight: bold; color: #0066CC; text-align: center; }");
                                sw.WriteLine(".info { text-align: center; font-size: 9pt; color: #404040; }");
                                sw.WriteLine(".even { background-color: #F0F8FF; }");
                                sw.WriteLine(".odd { background-color: #FFFFFF; }");
                                sw.WriteLine("</style>");
                                sw.WriteLine("</head><body>");

                                sw.WriteLine("<table border='0' cellspacing='0' cellpadding='0'>");

                                sw.WriteLine("<tr><td colspan='5' class='title'>SISTEMA DE MONITORIZACIÓN DE RED</td></tr>");
                                sw.WriteLine("<tr><td colspan='5' class='subtitle'>Historial de Alertas de Seguridad</td></tr>");
                                sw.WriteLine($"<tr><td colspan='5' class='info'>Período: {fechaDesde:dd/MM/yyyy HH:mm} — {fechaHasta:dd/MM/yyyy HH:mm}</td></tr>");
                                sw.WriteLine($"<tr><td colspan='5' class='info'>Exportado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</td></tr>");
                                sw.WriteLine("<tr><td colspan='5' style='height:10px;'></td></tr>");

                                sw.WriteLine("<tr>");
                                sw.WriteLine("<th>Tipo de Alerta</th><th>Descripción</th><th>Severidad</th><th>IP Involucrada</th><th>Fecha y Hora</th>");
                                sw.WriteLine("</tr>");

                                int row = 0;
                                while (reader.Read())
                                {
                                    string rowClass = (row % 2 == 0) ? "even" : "odd";
                                    sw.WriteLine($"<tr class='{rowClass}'>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["tipo"]}</td>");
                                    sw.WriteLine($"<td>{reader["descripcion"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["severidad"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["ip_involucrada"]}</td>");
                                    sw.WriteLine($"<td style='text-align:center;'>{reader["timestamp"]}</td>");
                                    sw.WriteLine("</tr>");
                                    row++;
                                }

                                sw.WriteLine("</table>");
                                sw.WriteLine("</body></html>");
                            }
                        }
                    }
                }
                if (ruta.EndsWith(".xlsx"))
                {
                    string nuevaRuta = ruta.Replace(".xlsx", ".xls");
                    File.Move(ruta, nuevaRuta);
                    MostrarNotificacionEmergente("✅ Alertas exportadas a Excel (.xls)", Color.FromArgb(0, 120, 100), 2000);
                }
                else
                {
                    MostrarNotificacionEmergente("✅ Alertas exportadas a Excel", Color.FromArgb(0, 120, 100), 2000);
                }
            }
            catch (Exception ex)
            {
                MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000);
            }
        }

        private void ExportarPaquetesACSV(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional 
                             FROM paquetes WHERE hora BETWEEN @desde AND @hasta ORDER BY hora DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        using (StreamWriter sw = new StreamWriter(ruta, false, Encoding.UTF8))
                        {
                            // Anchos fijos para cada columna (ajústalos según tus necesidades)
                            int[] anchos = { 12, 18, 18, 10, 15, 15, 12, 50 };

                            // Cabecera decorativa
                            string separadorDoble = new string('═', anchos.Sum() + anchos.Length - 1 + 4);
                            string separadorSimple = new string('─', anchos.Sum() + anchos.Length - 1 + 4);

                            sw.WriteLine("╔" + separadorDoble + "╗");
                            sw.WriteLine("║" + CentrarTexto("SISTEMA DE MONITORIZACIÓN DE RED", separadorDoble.Length) + "║");
                            sw.WriteLine("║" + CentrarTexto("REPORTE DE PAQUETES CAPTURADOS", separadorDoble.Length) + "║");
                            sw.WriteLine("╠" + separadorDoble + "╣");
                            sw.WriteLine("║" + CentrarTexto($"Período: {fechaDesde:dd/MM/yyyy HH:mm} - {fechaHasta:dd/MM/yyyy HH:mm}", separadorDoble.Length) + "║");
                            sw.WriteLine("║" + CentrarTexto($"Exportado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", separadorDoble.Length) + "║");
                            sw.WriteLine("╚" + separadorDoble + "╝");
                            sw.WriteLine();

                            // Encabezados de columna
                            string[] headers = { "Hora", "IP Origen", "IP Destino", "Protocolo", "Puerto Origen", "Puerto Destino", "Tamaño (B)", "Información Adicional" };
                            string lineaEncabezado = "┌";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaEncabezado += new string('─', anchos[i]);
                                lineaEncabezado += (i < headers.Length - 1) ? "┬" : "┐";
                            }
                            sw.WriteLine(lineaEncabezado);

                            // Fila de títulos
                            sw.Write("│");
                            for (int i = 0; i < headers.Length; i++)
                            {
                                sw.Write(AlinearTexto(headers[i], anchos[i]));
                                sw.Write("│");
                            }
                            sw.WriteLine();

                            // Separador después de encabezados
                            string lineaSeparadora = "├";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaSeparadora += new string('─', anchos[i]);
                                lineaSeparadora += (i < headers.Length - 1) ? "┼" : "┤";
                            }
                            sw.WriteLine(lineaSeparadora);

                            // Datos
                            while (reader.Read())
                            {
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["hora"].ToString(), anchos[0]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["ip_origen"].ToString(), anchos[1]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["ip_destino"].ToString(), anchos[2]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["protocolo"].ToString(), anchos[3]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["puerto_origen"].ToString(), anchos[4]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["puerto_destino"].ToString(), anchos[5]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["tamaño"].ToString(), anchos[6]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["informacion_adicional"]?.ToString() ?? "", anchos[7]));
                                sw.WriteLine("│");
                            }

                            // Línea final de cierre
                            string lineaFinal = "└";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaFinal += new string('─', anchos[i]);
                                lineaFinal += (i < headers.Length - 1) ? "┴" : "┘";
                            }
                            sw.WriteLine(lineaFinal);

                            sw.WriteLine();
                            sw.WriteLine($"Total de registros: {contadorPaquetes}");
                        }
                    }
                }
                MostrarNotificacionEmergente("✅ Paquetes exportados correctamente", Color.FromArgb(0, 120, 100), 2000);
            }
            catch (Exception ex)
            {
                MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000);
            }
        }

        // Métodos auxiliares para formateo de texto
        private string AlinearTexto(string texto, int ancho)
        {
            if (texto.Length > ancho)
                return texto.Substring(0, ancho - 2) + "..";
            return texto.PadRight(ancho);
        }

        private string CentrarTexto(string texto, int ancho)
        {
            if (texto.Length >= ancho)
                return texto.Substring(0, ancho);
            int espacios = (ancho - texto.Length) / 2;
            return new string(' ', espacios) + texto + new string(' ', ancho - texto.Length - espacios);
        }

        private void ExportarAlertasACSV(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT tipo, descripcion, severidad, ip_involucrada, ""timestamp"" 
                             FROM alertas WHERE ""timestamp"" BETWEEN @desde AND @hasta ORDER BY timestamp DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        using (StreamWriter sw = new StreamWriter(ruta, false, Encoding.UTF8))
                        {
                            int[] anchos = { 20, 60, 12, 18, 22 };

                            string separadorDoble = new string('═', anchos.Sum() + anchos.Length - 1 + 4);

                            sw.WriteLine("╔" + separadorDoble + "╗");
                            sw.WriteLine("║" + CentrarTexto("SISTEMA DE MONITORIZACIÓN DE RED", separadorDoble.Length) + "║");
                            sw.WriteLine("║" + CentrarTexto("HISTORIAL DE ALERTAS", separadorDoble.Length) + "║");
                            sw.WriteLine("╠" + separadorDoble + "╣");
                            sw.WriteLine("║" + CentrarTexto($"Período: {fechaDesde:dd/MM/yyyy HH:mm} - {fechaHasta:dd/MM/yyyy HH:mm}", separadorDoble.Length) + "║");
                            sw.WriteLine("║" + CentrarTexto($"Exportado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", separadorDoble.Length) + "║");
                            sw.WriteLine("╚" + separadorDoble + "╝");
                            sw.WriteLine();

                            string[] headers = { "Tipo", "Descripción", "Severidad", "IP Involucrada", "Fecha y Hora" };
                            string lineaEncabezado = "┌";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaEncabezado += new string('─', anchos[i]);
                                lineaEncabezado += (i < headers.Length - 1) ? "┬" : "┐";
                            }
                            sw.WriteLine(lineaEncabezado);

                            sw.Write("│");
                            for (int i = 0; i < headers.Length; i++)
                            {
                                sw.Write(AlinearTexto(headers[i], anchos[i]));
                                sw.Write("│");
                            }
                            sw.WriteLine();

                            string lineaSeparadora = "├";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaSeparadora += new string('─', anchos[i]);
                                lineaSeparadora += (i < headers.Length - 1) ? "┼" : "┤";
                            }
                            sw.WriteLine(lineaSeparadora);

                            while (reader.Read())
                            {
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["tipo"].ToString(), anchos[0]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["descripcion"]?.ToString() ?? "", anchos[1]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["severidad"].ToString(), anchos[2]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["ip_involucrada"]?.ToString() ?? "N/A", anchos[3]));
                                sw.Write("│");
                                sw.Write(AlinearTexto(reader["timestamp"].ToString(), anchos[4]));
                                sw.WriteLine("│");
                            }

                            string lineaFinal = "└";
                            for (int i = 0; i < headers.Length; i++)
                            {
                                lineaFinal += new string('─', anchos[i]);
                                lineaFinal += (i < headers.Length - 1) ? "┴" : "┘";
                            }
                            sw.WriteLine(lineaFinal);
                        }
                    }
                }
                MostrarNotificacionEmergente("✅ Alertas exportadas correctamente", Color.FromArgb(0, 120, 100), 2000);
            }
            catch (Exception ex)
            {
                MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000);
            }
        }

        private void ExportarPaquetesAPDF(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional 
                                     FROM paquetes WHERE hora BETWEEN @desde AND @hasta ORDER BY hora DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            PdfDocument document = new PdfDocument { Info = { Title = "Paquetes Capturados" } };
                            XFont titleFont = new XFont("Segoe UI", 16, XFontStyleEx.Bold);
                            XFont headerFont = new XFont("Segoe UI", 8, XFontStyleEx.Bold);
                            XFont cellFont = new XFont("Segoe UI", 7, XFontStyleEx.Regular);
                            string[] headers = { "Hora", "IP Origen", "IP Destino", "Protocolo", "Puerto Origen", "Puerto Destino", "Tamaño (bytes)", "Información" };
                            double[] columnWidths = { 70, 120, 120, 60, 60, 60, 60, 212 };
                            string titulo = $"Paquetes ({fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy})";
                            DrawMultiPageTable(document, reader, titleFont, headerFont, cellFont, headers, columnWidths, titulo);
                            document.Save(ruta);
                        }
                    }
                }
                MostrarNotificacionEmergente("✅ Paquetes exportados a PDF", Color.FromArgb(0, 120, 100), 2000);
            }
            catch (Exception ex) { MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000); }
        }

        private void ExportarAlertasAPDF(string ruta, DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT tipo, descripcion, severidad, ip_involucrada, ""timestamp"" 
                                     FROM alertas WHERE ""timestamp"" BETWEEN @desde AND @hasta ORDER BY timestamp DESC";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", fechaDesde);
                        cmd.Parameters.AddWithValue("@hasta", fechaHasta);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            PdfDocument document = new PdfDocument { Info = { Title = "Alertas de Seguridad" } };
                            XFont titleFont = new XFont("Segoe UI", 16, XFontStyleEx.Bold);
                            XFont headerFont = new XFont("Segoe UI", 8, XFontStyleEx.Bold);
                            XFont cellFont = new XFont("Segoe UI", 7, XFontStyleEx.Regular);
                            string[] headers = { "Tipo", "Descripción", "Severidad", "IP Involucrada", "Fecha y Hora" };
                            double[] columnWidths = { 80, 280, 80, 100, 222 };
                            string titulo = $"Alertas ({fechaDesde:dd/MM/yyyy} - {fechaHasta:dd/MM/yyyy})";
                            DrawMultiPageTable(document, reader, titleFont, headerFont, cellFont, headers, columnWidths, titulo);
                            document.Save(ruta);
                        }
                    }
                }
                MostrarNotificacionEmergente("✅ Alertas exportadas a PDF", Color.FromArgb(0, 120, 100), 2000);
            }
            catch (Exception ex) { MostrarNotificacionEmergente($"❌ Error: {ex.Message}", Color.FromArgb(200, 50, 50), 3000); }
        }

        /// <summary>
        /// Dibuja una tabla profesional con colores del sistema, múltiples páginas y formato uniforme.
        /// </summary>
        private void DrawMultiPageTable(PdfDocument document, NpgsqlDataReader reader,
            XFont titleFont, XFont headerFont, XFont cellFont,
            string[] headers, double[] columnWidths, string reportTitle)
        {
            double marginLeft = 25, marginTop = 40, marginBottom = 30;
            double rowHeight = 18, headerHeight = 22, cellPadding = 3;

            XColor headerBgColor = XColor.FromArgb(0, 102, 204);
            XColor rowEvenColor = XColor.FromArgb(245, 245, 245);
            XColor rowOddColor = XColor.FromArgb(255, 255, 255);
            XColor borderColor = XColor.FromArgb(200, 200, 200);
            XFont infoFont = new XFont("Segoe UI", 6.5, XFontStyleEx.Regular);

            PdfPage currentPage = null;
            XGraphics gfx = null;
            double yPos = 0;
            bool isFirstPage = true;
            int rowIndex = 0;

            while (reader.Read())
            {
                if (currentPage == null || yPos + rowHeight > (currentPage.Height.Point - marginBottom))
                {
                    currentPage = document.AddPage();
                    currentPage.Size = PdfSharp.PageSize.A4;
                    currentPage.Orientation = PdfSharp.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(currentPage);
                    yPos = marginTop;

                    if (isFirstPage)
                    {
                        gfx.DrawString(reportTitle, titleFont, XBrushes.Black,
                            new XRect(0, yPos, currentPage.Width.Point, 24), XStringFormats.TopCenter);
                        yPos += 30;
                        gfx.DrawString($"Exportado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", cellFont,
                            new XSolidBrush(XColor.FromArgb(100, 100, 100)),
                            new XRect(0, yPos, currentPage.Width.Point, 12), XStringFormats.TopCenter);
                        yPos += 18;
                        isFirstPage = false;
                    }

                    double xPos = marginLeft;
                    for (int i = 0; i < headers.Length; i++)
                    {
                        gfx.DrawRectangle(new XSolidBrush(headerBgColor), xPos, yPos, columnWidths[i], headerHeight);
                        gfx.DrawString(headers[i], headerFont, XBrushes.White,
                            new XRect(xPos + cellPadding, yPos, columnWidths[i] - 2 * cellPadding, headerHeight),
                            XStringFormats.Center);
                        xPos += columnWidths[i];
                    }
                    yPos += headerHeight;
                }

                double xData = marginLeft;
                XColor rowBgColor = (rowIndex % 2 == 0) ? rowEvenColor : rowOddColor;
                gfx.DrawRectangle(new XSolidBrush(rowBgColor), xData, yPos, columnWidths.Sum(), rowHeight);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string text = reader[i]?.ToString() ?? "";
                    XFont fontToUse = (i == 7) ? infoFont : cellFont;
                    XStringFormat alignment = (i == 1 || i == 2 || i == 7) ? XStringFormats.CenterLeft : XStringFormats.Center;

                    gfx.DrawRectangle(new XPen(borderColor, 0.5), xData, yPos, columnWidths[i], rowHeight);
                    gfx.DrawString(text, fontToUse, XBrushes.Black,
                        new XRect(xData + cellPadding, yPos, columnWidths[i] - 2 * cellPadding, rowHeight), alignment);
                    xData += columnWidths[i];
                }
                yPos += rowHeight;
                rowIndex++;
            }
        }

        #endregion

        #region Modo Oscuro / Claro

        private void ToggleModoOscuro(object sender, EventArgs e)
        {
            esModoOscuro = !esModoOscuro;
            if (esModoOscuro) AplicarTemaOscuro(); else AplicarTemaClaro();
        }

        private void AplicarTemaOscuro()
        {
            // Fondo general del formulario y panel superior
            this.BackColor = Color.FromArgb(30, 30, 35);
            this.ForeColor = Color.WhiteSmoke;
            headerPanel.BackColor = Color.FromArgb(45, 45, 48);

            cmbFiltroIPOrigen.BackColor = Color.FromArgb(70, 70, 75);
            cmbFiltroIPOrigen.ForeColor = Color.White;
            cmbFiltroIPDestino.BackColor = Color.FromArgb(70, 70, 75);
            cmbFiltroIPDestino.ForeColor = Color.White;

            // Fondo de la pestaña (tabPage1)
            tabPage1.BackColor = Color.FromArgb(30, 30, 35);

            // Grupos de diagnóstico, captura y filtros
            foreach (var gb in new[] { gbCaptura, gbFiltros, gbDiagnostico })
            {
                if (gb == null) continue;
                gb.BackColor = Color.FromArgb(40, 40, 45);
                gb.ForeColor = Color.WhiteSmoke;
                foreach (Control ctrl in gb.Controls) AplicarColorOscuroControl(ctrl);
            }

            // Panel con scroll dentro del diagnóstico
            panelScrollDiagnostico.BackColor = Color.FromArgb(35, 35, 40);

            // DataGridView de paquetes
            dgvPaquetes.BackgroundColor = Color.FromArgb(50, 50, 55);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 70, 75);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPaquetes.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgvPaquetes.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(75, 75, 85);
            dgvPaquetes.GridColor = Color.FromArgb(80, 80, 85);
            dgvPaquetes.EnableHeadersVisualStyles = false;

            // Etiquetas de estadísticas
            lblEstadisticas.ForeColor = Color.WhiteSmoke;
            lblEstadisticasTiempoReal.ForeColor = Color.WhiteSmoke;
            lblEstadisticasFiltro.ForeColor = Color.WhiteSmoke;

            // Velocímetros y etiquetas de diagnóstico
            if (CpbVelocidadEnlace2 != null)
            {
                CpbVelocidadEnlace2.ForeColor = Color.White;
                CpbVelocidadEnlace2.OuterColor = Color.DarkGray;
                CpbVelocidadEnlace2.InnerColor = Color.FromArgb(50, 50, 55);
            }
            if (CpbLatencia2 != null)
            {
                CpbLatencia2.ForeColor = Color.White;
                CpbLatencia2.OuterColor = Color.DarkGray;
                CpbLatencia2.InnerColor = Color.FromArgb(50, 50, 55);
            }
            if (CpbBandaWifi2 != null)
            {
                CpbBandaWifi2.ForeColor = Color.White;
                CpbBandaWifi2.OuterColor = Color.DarkGray;
                CpbBandaWifi2.InnerColor = Color.FromArgb(50, 50, 55);
            }
            if (lblVelocidadValor2 != null) lblVelocidadValor2.ForeColor = Color.WhiteSmoke;
            if (lblLatenciaValor2 != null) lblLatenciaValor2.ForeColor = Color.WhiteSmoke;
            if (lblBandaValor2 != null) lblBandaValor2.ForeColor = Color.WhiteSmoke;

            // Botones de filtro
            btnLimpiarFiltros.BackColor = Color.FromArgb(80, 80, 85);
            btnLimpiarFiltros.ForeColor = Color.WhiteSmoke;

            // Menú
            menuStrip.BackColor = Color.FromArgb(45, 45, 48);
            menuStrip.ForeColor = Color.WhiteSmoke;

            // Gráfico de tráfico
            chartTrafico.BackColor = Color.FromArgb(30, 30, 35);
            chartTrafico.DefaultLegend.Foreground = System.Windows.Media.Brushes.White;
            if (chartTrafico.AxisX.Count > 0)
                chartTrafico.AxisX[0].Foreground = System.Windows.Media.Brushes.White;
            if (chartTrafico.AxisY.Count > 0)
                chartTrafico.AxisY[0].Foreground = System.Windows.Media.Brushes.White;

            // Actualizar logo
            ActualizarLogo();
        }

        private void AplicarColorOscuroControl(Control ctrl)
        {
            if (ctrl is Label lbl) lbl.ForeColor = Color.LightGray;
            else if (ctrl is TextBox txt) { txt.BackColor = Color.FromArgb(70, 70, 75); txt.ForeColor = Color.LightGray; }
            else if (ctrl is CheckBox chk) chk.ForeColor = Color.LightGray;
            else if (ctrl is ComboBox cb) { cb.BackColor = Color.FromArgb(70, 70, 75); cb.ForeColor = Color.LightGray; }
        }

        private void AplicarTemaClaro()
        {
            // Fondo general del formulario y panel superior
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.ForeColor = Color.Black;
            headerPanel.BackColor = Color.FromArgb(0, 102, 204);

            // Fondo de la pestaña (tabPage1)
            tabPage1.BackColor = Color.White;

            cmbFiltroIPOrigen.BackColor = Color.White;
            cmbFiltroIPOrigen.ForeColor = Color.Black;
            cmbFiltroIPDestino.BackColor = Color.White;
            cmbFiltroIPDestino.ForeColor = Color.Black;

            // Grupos de diagnóstico, captura y filtros
            foreach (var gb in new[] { gbCaptura, gbFiltros, gbDiagnostico })
            {
                if (gb == null) continue;
                gb.BackColor = Color.White;
                gb.ForeColor = Color.FromArgb(0, 51, 102);
                foreach (Control ctrl in gb.Controls) AplicarColorClaroControl(ctrl);
            }

            // Panel con scroll dentro del diagnóstico
            panelScrollDiagnostico.BackColor = Color.White;

            // Velocímetros y etiquetas de diagnóstico
            if (CpbVelocidadEnlace2 != null)
            {
                CpbVelocidadEnlace2.ForeColor = Color.Black;
                CpbVelocidadEnlace2.OuterColor = Color.LightGray;
                CpbVelocidadEnlace2.InnerColor = Color.White;
            }
            if (CpbLatencia2 != null)
            {
                CpbLatencia2.ForeColor = Color.Black;
                CpbLatencia2.OuterColor = Color.LightGray;
                CpbLatencia2.InnerColor = Color.White;
            }
            if (CpbBandaWifi2 != null)
            {
                CpbBandaWifi2.ForeColor = Color.Black;
                CpbBandaWifi2.OuterColor = Color.LightGray;
                CpbBandaWifi2.InnerColor = Color.White;
            }
            if (lblVelocidadValor2 != null) lblVelocidadValor2.ForeColor = Color.Black;
            if (lblLatenciaValor2 != null) lblLatenciaValor2.ForeColor = Color.Black;
            if (lblBandaValor2 != null) lblBandaValor2.ForeColor = Color.Black;

            // DataGridView de paquetes
            dgvPaquetes.BackgroundColor = Color.White;
            dgvPaquetes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPaquetes.DefaultCellStyle.BackColor = Color.White;
            dgvPaquetes.DefaultCellStyle.ForeColor = Color.Black;
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPaquetes.GridColor = Color.LightGray;

            // Etiquetas de estadísticas
            lblEstadisticas.ForeColor = Color.FromArgb(0, 51, 102);
            lblEstadisticasTiempoReal.ForeColor = Color.Black;
            lblEstadisticasFiltro.ForeColor = Color.Black;

            // Botones de filtro
            btnLimpiarFiltros.BackColor = Color.LightGray;
            btnLimpiarFiltros.ForeColor = Color.Black;

            // Menú
            menuStrip.BackColor = SystemColors.Control;
            menuStrip.ForeColor = Color.Black;

            chartTrafico.BackColor = Color.White;
            chartTrafico.DefaultLegend.Foreground = System.Windows.Media.Brushes.Black;
            if (chartTrafico.AxisX.Count > 0)
                chartTrafico.AxisX[0].Foreground = System.Windows.Media.Brushes.Black;
            if (chartTrafico.AxisY.Count > 0)
                chartTrafico.AxisY[0].Foreground = System.Windows.Media.Brushes.Black;

            // Actualizar logo
            ActualizarLogo();
        }

        private void AplicarColorClaroControl(Control ctrl)
        {
            if (ctrl is Label lbl) lbl.ForeColor = Color.Black;
            else if (ctrl is TextBox txt) { txt.BackColor = Color.White; txt.ForeColor = Color.Black; }
            else if (ctrl is CheckBox chk) chk.ForeColor = Color.Black;
            else if (ctrl is ComboBox cb) { cb.BackColor = Color.White; cb.ForeColor = Color.Black; }
        }

        #endregion

        #region Notificaciones y Eventos Varios

        private void MostrarNotificacionEmergente(string mensaje, Color colorFondo, int duracionMs = 5000)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MostrarNotificacionEmergente(mensaje, colorFondo, duracionMs)));
                return;
            }

            // Encolar la notificación
            colaNotificaciones.Enqueue((mensaje, colorFondo, duracionMs));

            // Si el timer no está activo, iniciar el procesamiento
            if (!timerNotificaciones.Enabled)
            {
                ProcesarColaNotificaciones();
                timerNotificaciones.Start();
            }
        }

        private void btnVerAlertas_Click(object sender, EventArgs e)
        {
            HistorialAlertas frm = new HistorialAlertas();
            frm.ModoOscuro = esModoOscuro;
            frm.ShowDialog();
        }

        private void SalirMenu_Click(object sender, EventArgs e) => this.Close();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimerGrid?.Stop();
            timerGrafico?.Stop();
            base.OnFormClosing(e);
        }

        #endregion

        #region Diagnóstico de Red (Velocidad, Latencia, Banda Wi-Fi)

        private void ActualizarVelocidadEnlace()
        {
            try
            {
                string selected = cmbInterfaces.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selected)) { velocidadEnlaceActual = "No seleccionada"; return; }

                string descripcion = selected.Contains("-") ? selected.Split('-')[1].Trim() : selected;
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.Description.Equals(descripcion, StringComparison.OrdinalIgnoreCase) || selected.Contains(ni.Name))
                    {
                        long speed = ni.Speed;
                        if (speed >= 1_000_000_000) velocidadEnlaceActual = $"{(speed / 1_000_000_000.0):F1} Gbps";
                        else if (speed >= 1_000_000) velocidadEnlaceActual = $"{(speed / 1_000_000):F0} Mbps";
                        else if (speed >= 1000) velocidadEnlaceActual = $"{speed / 1000} Kbps";
                        else velocidadEnlaceActual = $"{speed} bps";
                        return;
                    }
                }
                velocidadEnlaceActual = "No disponible";
            }
            catch { velocidadEnlaceActual = "Error"; }
        }

        private long ObtenerLatencia(string host)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(host, 1000);
                    return (reply.Status == IPStatus.Success) ? reply.RoundtripTime : -1;
                }
            }
            catch { return -1; }
        }

        private void ActualizarDiagnostico()
        {
            if (InvokeRequired) { Invoke(new Action(ActualizarDiagnostico)); return; }

            ActualizarVelocidadEnlace();

            if (CpbVelocidadEnlace2 != null && !velocidadEnlaceActual.Contains("No") && !velocidadEnlaceActual.Contains("Error"))
            {
                string valor = velocidadEnlaceActual.Replace(" Mbps", "").Replace(" Gbps", "").Replace(" Kbps", "").Replace(" bps", "");
                if (velocidadEnlaceActual.Contains("Gbps"))
                    CpbVelocidadEnlace2.Value = Math.Min((int)(double.Parse(valor, System.Globalization.CultureInfo.InvariantCulture) * 1000), 1000);
                else if (velocidadEnlaceActual.Contains("Mbps"))
                    CpbVelocidadEnlace2.Value = Math.Min(int.Parse(valor), 1000);
                else CpbVelocidadEnlace2.Value = 0;
                CpbVelocidadEnlace2.Text = velocidadEnlaceActual;
            }
            else if (CpbVelocidadEnlace2 != null) { CpbVelocidadEnlace2.Value = 0; CpbVelocidadEnlace2.Text = velocidadEnlaceActual; }

            long lat = ObtenerLatencia("8.8.8.8");
            ultimaLatencia = lat;
            if (CpbLatencia2 != null)
            {
                CpbLatencia2.Value = (lat == -1) ? 0 : (int)Math.Min(lat, CpbLatencia2.Maximum);
                CpbLatencia2.Text = (lat == -1) ? "Pérdida" : $"{lat} ms";
                CpbLatencia2.ProgressColor = (lat == -1) ? Color.Gray :
                    (lat < 50) ? Color.Green : (lat < 100) ? Color.Orange : Color.Red;
            }

            bandaWiFiActual = ObtenerBandaWifi();
            if (CpbBandaWifi2 != null)
            {
                if (bandaWiFiActual.Contains("2.4")) { CpbBandaWifi2.Value = 1; CpbBandaWifi2.ProgressColor = Color.Green; }
                else if (bandaWiFiActual.Contains("5")) { CpbBandaWifi2.Value = 2; CpbBandaWifi2.ProgressColor = Color.Orange; }
                else if (bandaWiFiActual.Contains("6")) { CpbBandaWifi2.Value = 3; CpbBandaWifi2.ProgressColor = Color.Red; }
                else { CpbBandaWifi2.Value = 0; CpbBandaWifi2.ProgressColor = Color.Gray; }
                CpbBandaWifi2.Text = bandaWiFiActual;
            }
        }

        private void GuardarLatenciaEnBD(string destino, long latenciaMs, bool perdido)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO latencias (""timestamp"", destino, latencia_ms, perdido) VALUES (@ts, @dest, @lat, @lost)";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ts", DateTime.Now);
                        cmd.Parameters.AddWithValue("@dest", destino);
                        cmd.Parameters.AddWithValue("@lat", latenciaMs > 0 ? latenciaMs : 0);
                        cmd.Parameters.AddWithValue("@lost", perdido);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error guardando latencia: " + ex.Message); }
        }

        #region WlanApi (obtención de banda Wi-Fi por canal)

        private const uint WLAN_AVAILABLE_NETWORK_INCLUDE_ALL = 0x00000001;
        private const uint WLAN_AVAILABLE_NETWORK_INCLUDE_ADHOC = 0x00000002;
        private const uint wlan_intf_opcode_current_connection = 0x00000007;
        private const uint wlan_intf_opcode_channel = 0x0000000C;

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_INTERFACE_INFO
        {
            public Guid InterfaceGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string strInterfaceDescription;
            public uint IsState;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_INTERFACE_INFO_LIST
        {
            public uint dwNumberOfItems;
            public uint dwIndex;
            public WLAN_INTERFACE_INFO InterfaceInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DOT11_SSID
        {
            public uint uSSIDLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] ucSSID;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WLAN_CONNECTION_ATTRIBUTES
        {
            public DOT11_SSID dot11Ssid;
            public uint dot11BssType;
            public uint dot11PhyType;
            public uint dot11SecurityType;
            public uint dot11CipherAlgorithm;
            public uint uFlags;
        }

        private delegate void WLAN_NOTIFICATION_CALLBACK(IntPtr data, IntPtr context);

        [DllImport("wlanapi.dll")]
        private static extern uint WlanOpenHandle(uint dwClientVersion, IntPtr pReserved, out uint pdwNegotiatedVersion, out IntPtr phClientHandle);
        [DllImport("wlanapi.dll")]
        private static extern uint WlanCloseHandle(IntPtr phClientHandle, IntPtr pReserved);
        [DllImport("wlanapi.dll")]
        private static extern uint WlanEnumInterfaces(IntPtr hClientHandle, IntPtr pReserved, out IntPtr ppInterfaceList);
        [DllImport("wlanapi.dll")]
        private static extern uint WlanFreeMemory(IntPtr pMemory);
        [DllImport("wlanapi.dll")]
        private static extern uint WlanQueryInterface(IntPtr hClientHandle, ref Guid pInterfaceGuid, uint OpCode, IntPtr pReserved, out uint pdwDataSize, out IntPtr ppData, out uint pWlanOpCodeValueType);

        /// <summary>
        /// Deduce la banda Wi‑Fi (2.4 GHz, 5 GHz o 6 GHz) a partir de la descripción
        /// y la velocidad del adaptador inalámbrico activo.
        /// </summary>
        /// <summary>
        /// Ejecuta "netsh wlan show interfaces" y extrae el canal para determinar la banda.
        /// </summary>
        private string ObtenerBandaWifi()
        {
            try
            {
                using (var proceso = new System.Diagnostics.Process())
                {
                    proceso.StartInfo.FileName = "netsh";
                    proceso.StartInfo.Arguments = "wlan show interfaces";
                    proceso.StartInfo.UseShellExecute = false;
                    proceso.StartInfo.RedirectStandardOutput = true;
                    proceso.StartInfo.CreateNoWindow = true;
                    proceso.Start();
                    string salida = proceso.StandardOutput.ReadToEnd();
                    proceso.WaitForExit(2000);

                    // Buscar la línea "Canal" o "Channel"
                    var lineas = salida.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var linea in lineas)
                    {
                        if (linea.Trim().StartsWith("Canal", StringComparison.OrdinalIgnoreCase) ||
                            linea.Trim().StartsWith("Channel", StringComparison.OrdinalIgnoreCase))
                        {
                            // Formato típico: "Canal                    : 6" o "Channel                 : 36"
                            var partes = linea.Split(':');
                            if (partes.Length >= 2)
                            {
                                string valor = partes[1].Trim();
                                if (int.TryParse(valor, out int canal))
                                {
                                    if (canal >= 1 && canal <= 14)
                                        return $"2.4 GHz (canal {canal})";
                                    else if (canal >= 36 && canal <= 165)
                                        return $"5 GHz (canal {canal})";
                                    else if (canal >= 1 && canal <= 233) // 6 GHz
                                        return $"6 GHz (canal {canal})";
                                    else
                                        return $"Canal desconocido: {canal}";
                                }
                            }
                        }
                    }

                    // Si no se encontró canal, verificar si la interfaz está conectada
                    foreach (var linea in lineas)
                    {
                        if (linea.Trim().StartsWith("Estado", StringComparison.OrdinalIgnoreCase) ||
                            linea.Trim().StartsWith("State", StringComparison.OrdinalIgnoreCase))
                        {
                            if (linea.IndexOf("conectado", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                linea.IndexOf("connected", StringComparison.OrdinalIgnoreCase) >= 0)
                                return "Banda no detectada";
                        }
                    }
                    return "Sin conexión Wi‑Fi activa";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en netsh: " + ex.Message);
                return "Error";
            }
        }

        #endregion

        #endregion

        #region Clase Auxiliar AlertaHelper

        /// <summary>
        /// Gestiona el guardado de alertas en la base de datos MySQL.
        /// </summary>
        public class AlertaHelper
        {
            private string connectionString = "Host=localhost;Database=monitorizacion_red;Username=postgres;Password=Theflashtemp*123";

            public async Task GuardarAlertaAsync(string tipo, string descripcion, string severidad, string ipInvolucrada = null)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                        {
                            conn.Open();
                            string query = @"INSERT INTO alertas (tipo, descripcion, severidad, ip_involucrada, ""timestamp"") 
                                     VALUES(@tipo, @descripcion, @severidad, @ip, @timestamp)";
                            using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@tipo", tipo);
                                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                                cmd.Parameters.AddWithValue("@severidad", severidad);
                                cmd.Parameters.AddWithValue("@ip", ipInvolucrada ?? "");
                                cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error guardando alerta: " + ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Procesa la cola de notificaciones mostrando una cada vez que se dispara el timer.
        /// </summary>
        private void ProcesarColaNotificaciones()
        {
            if (procesandoCola) return;

            if (colaNotificaciones.Count == 0)
            {
                timerNotificaciones.Stop();
                return;
            }

            procesandoCola = true;
            var (mensaje, colorFondo, duracionMs) = colaNotificaciones.Dequeue();
            FrmToast.Mostrar(mensaje, colorFondo, duracionMs);
            procesandoCola = false;
        }

        #endregion

        #region entradas vacias

        private void tabPage1_Click_1(object sender, EventArgs e) { }
        private void archivoToolStripMenuItem_Click_1(object sender, EventArgs e) { }
        private void headerPanel_Paint(object sender, PaintEventArgs e) { }

        #endregion
    }
}