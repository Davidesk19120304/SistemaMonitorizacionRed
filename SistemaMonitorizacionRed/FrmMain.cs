using LiveCharts;               // Librería para gráficos en tiempo real
using LiveCharts.Wpf;           // Componentes WPF para LiveCharts
using MySql.Data.MySqlClient;   // Conector MySQL
using PacketDotNet;             // Decodificación de paquetes de red
using SharpPcap;                // Captura de paquetes
using SharpPcap.LibPcap;        // Implementación para Windows (Npcap)
using System;                   // Tipos básicos
using System.Collections.Generic; // Listas, diccionarios
using System.Drawing;           // Gráficos (colores, fuentes, etc.)
using System.Linq;              // Operaciones LINQ
using System.Windows.Forms;     // Controles de interfaz gráfica

namespace SistemaMonitorizacionRed
{
    public partial class FrmMain : Form
    {
        #region Variables

        // Dispositivo de captura (interfaz de red seleccionada)
        private LibPcapLiveDevice dispositivo;
        // Contador total de paquetes capturados
        private int contadorPaquetes = 0;
        // Lista en memoria con todos los paquetes capturados (máximo 1000)
        private List<PaqueteInfo> todosLosPaquetes = new List<PaqueteInfo>();

        // Series de datos para el gráfico (cada una guarda los valores de los últimos 30 segundos)
        private ChartValues<int> valoresTCP = new ChartValues<int>();
        private ChartValues<int> valoresUDP = new ChartValues<int>();
        private ChartValues<int> valoresICMP = new ChartValues<int>();
        private ChartValues<int> valoresIGMP = new ChartValues<int>();
        // Etiquetas del eje X del gráfico (segundos)
        private List<string> etiquetasTiempo = new List<string>();
        private int contadorSegundos = 0;           // Contador de segundos para el gráfico
        private Timer timerGrafico;                 // Timer que actualiza el gráfico cada segundo

        // Contadores de tráfico por segundo
        private int paquetesPorSegundo = 0;
        private int tcpPorSegundo = 0;
        private int udpPorSegundo = 0;
        private int icmpPorSegundo = 0;
        private int igmpPorSegundo = 0;

        // Timer para actualizar el DataGridView periódicamente (evita saturar la UI)
        private Timer refreshTimerGrid;
        private int paquetesPendientes = 0;         // Paquetes recibidos desde la última actualización

        // Sistema de alertas
        private AlertaHelper alertaHelper = new AlertaHelper();
        private bool alertasActivas = true;         // Habilita/deshabilita todas las alertas

        // Colas para la detección adaptativa (línea base dinámica)
        private Queue<int> historialPaquetes = new Queue<int>();   // Historial de paquetes/segundo
        private Queue<int> historialICMP = new Queue<int>();       // Historial de ICMP/segundo
        private double factorSigma = 3.0;                           // Sensibilidad (desviaciones estándar)
        private DateTime ultimaAlertaTrafico = DateTime.MinValue;   // Última alerta de pico
        private DateTime ultimaAlertaICMP = DateTime.MinValue;      // Última alerta de ICMP flood
        private TimeSpan cooldown = TimeSpan.FromSeconds(10);       // Tiempo mínimo entre alertas del mismo tipo

        // Estructuras para detección con umbrales fijos (no configurables)
        // Escaneo de puertos horizontal
        private Dictionary<string, Dictionary<int, DateTime>> intentosEscaneo = new Dictionary<string, Dictionary<int, DateTime>>();
        private int ventanaEscaneoSegundos = 60;
        private int umbralEscaneo = 10;

        // Fuerza bruta (múltiples intentos de conexión a puertos sensibles)
        private Dictionary<string, Dictionary<int, List<DateTime>>> intentosFuerzaBruta = new Dictionary<string, Dictionary<int, List<DateTime>>>();
        private int umbralFuerzaBruta = 10;
        private int ventanaFuerzaBrutaSegundos = 60;

        // Escaneo vertical (múltiples IPs atacando el mismo puerto)
        private Dictionary<int, Dictionary<string, List<DateTime>>> intentosEscaneoVertical = new Dictionary<int, Dictionary<string, List<DateTime>>>();
        private int umbralEscaneoVertical = 10;
        private int ventanaEscaneoVerticalSegundos = 60;

        // Timer para limpiar estructuras antiguas (evita consumo excesivo de memoria)
        private Timer timerLimpiezaGeneral;

        // Filtros de visualización y tema
        private string filtroProtocolo = "Todos";
        private string filtroIPOrigen = "";
        private string filtroIPDestino = "";
        private bool esModoOscuro = false;      // Estado del tema actual

        // Cadena de conexión a MySQL (ajústala según tu configuración)
        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";

        // Clase interna para almacenar la información de cada paquete capturado
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

        /// <summary>
        /// Constructor del formulario principal. Recibe el usuario y el rol autenticados.
        /// </summary>
        /// <param name="usuario">Nombre del usuario que inició sesión</param>
        /// <param name="rol">Rol del usuario (Administrador o Usuario)</param>
        public FrmMain(string usuario, string rol)
        {
            InitializeComponent();                          // Inicializar los controles del formulario
            this.Text = $"Monitorización - Usuario: {usuario} ({rol})"; // Mostrar usuario en la barra de título

            // Agregar un espacio para un logo en el panel de cabecera
            AgregarLogo();

            // Configurar el DataGridView (columnas, anchos, estilos)
            ConfigurarDataGridView();
            // Configurar el gráfico (series, ejes, timer)
            ConfigurarGrafico();

            // Deshabilitar la adición automática de filas en el DataGridView
            dgvPaquetes.AllowUserToAddRows = false;
            // Asociar el evento de formateo de celdas para colorear según protocolo
            dgvPaquetes.CellFormatting += DgvPaquetes_CellFormatting;

            // Timer que limpia las estructuras de detección cada minuto para liberar memoria
            timerLimpiezaGeneral = new Timer();
            timerLimpiezaGeneral.Interval = 60000;
            timerLimpiezaGeneral.Tick += (s, e) => LimpiarEstructurasAntiguas();
            timerLimpiezaGeneral.Start();

            // Timer que actualiza el DataGridView periódicamente (cada 500 ms)
            // Esto evita que la UI se sature al recibir muchos paquetes por segundo
            refreshTimerGrid = new Timer();
            refreshTimerGrid.Interval = 500;
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

            // Valores por defecto de las alertas
            alertasActivas = true;
            factorSigma = 3.0;

            // Permitir solo números y puntos en los campos de IP
            txtFiltroIPOrigen.KeyPress += TxtIP_KeyPress;
            txtFiltroIPDestino.KeyPress += TxtIP_KeyPress;
            // Opcional: validar formato de IP al salir del campo
            txtFiltroIPOrigen.Leave += TxtIP_Leave;
            txtFiltroIPDestino.Leave += TxtIP_Leave;
        }

        /// <summary>
        /// Agrega un PictureBox para el logo en la esquina superior izquierda del headerPanel.
        /// Si no se proporciona una imagen, solo reserva el espacio.
        /// </summary>
        private void AgregarLogo()
        {
            if (headerPanel == null) return;  // Si no existe el panel, salir

            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            logo.Size = new Size(60, 60);      // Tamaño del logo
            logo.Location = new Point(10, 5);   // Posición dentro del panel
            logo.BackColor = Color.Transparent;
            // logo.Image = Image.FromFile("logo.png"); // Descomenta si tienes una imagen
            logo.BorderStyle = BorderStyle.None;
            headerPanel.Controls.Add(logo);

            // Desplazar el título hacia la derecha para que no se superponga con el logo
            titleLabel.Left = 80;
        }

        /// <summary>
        /// Configura el gráfico de LiveCharts: define las series (TCP, UDP, ICMP, IGMP),
        /// los ejes, la leyenda y el timer que actualizará los datos cada segundo.
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

            // Configurar el eje X (tiempo en segundos)
            chartTrafico.AxisX.Add(new Axis
            {
                Title = "Tiempo (segundos)",
                Labels = etiquetasTiempo,
                DisableAnimations = true
            });
            // Configurar el eje Y (cantidad de paquetes por segundo)
            chartTrafico.AxisY.Add(new Axis
            {
                Title = "Paquetes por segundo",
                MinValue = 0,
                DisableAnimations = true
            });
            // Ubicación de la leyenda (arriba)
            chartTrafico.LegendLocation = LegendLocation.Top;

            // Timer que se ejecutará cada segundo para actualizar el gráfico y las estadísticas
            timerGrafico = new Timer();
            timerGrafico.Interval = 1000;
            timerGrafico.Tick += TimerGrafico_Tick;
        }

        /// <summary>
        /// Configura las columnas del DataGridView: nombres, anchos y estilos.
        /// </summary>
        private void ConfigurarDataGridView()
        {
            dgvPaquetes.Columns.Clear();   // Limpiar columnas existentes
            // Agregar columnas con sus encabezados
            dgvPaquetes.Columns.Add("Hora", "Hora");
            dgvPaquetes.Columns.Add("Origen", "IPOrigen");
            dgvPaquetes.Columns.Add("Destino", "IPDestino");
            dgvPaquetes.Columns.Add("Protocolo", "Protocolo");
            dgvPaquetes.Columns.Add("PuertoOrigen", "Puerto Origen");
            dgvPaquetes.Columns.Add("PuertoDestino", "Puerto Destino");
            dgvPaquetes.Columns.Add("Tamaño", "Tamaño (bytes)");
            dgvPaquetes.Columns.Add("Info", "Informacion");

            // Ajustar anchos de columna para mejor visualización
            dgvPaquetes.Columns["Hora"].Width = 100;
            dgvPaquetes.Columns["Origen"].Width = 150;
            dgvPaquetes.Columns["Destino"].Width = 150;
            dgvPaquetes.Columns["Protocolo"].Width = 120;
            dgvPaquetes.Columns["PuertoOrigen"].Width = 120;
            dgvPaquetes.Columns["PuertoDestino"].Width = 120;
            dgvPaquetes.Columns["Tamaño"].Width = 120;
            dgvPaquetes.Columns["Info"].Width = 300;

            // Alternar colores de filas para facilitar la lectura
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPaquetes.RowHeadersVisible = false;
        }

        #endregion

        #region Interfaces y Captura

        /// <summary>
        /// Evento Load del formulario. Carga las interfaces de red disponibles.
        /// </summary>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            CargarInterfaces();      // Obtener la lista de interfaces
            btnDetener.Enabled = false; // Inicialmente el botón Detener está deshabilitado
        }

        /// <summary>
        /// Carga en el ComboBox todas las interfaces de red detectadas por SharpPcap.
        /// </summary>
        private void CargarInterfaces()
        {
            cmbInterfaces.Items.Clear();
            var dispositivos = LibPcapLiveDeviceList.Instance; // Lista de dispositivos

            if (dispositivos.Count == 0)
            {
                MessageBox.Show("No se encontraron interfaces de red. Asegúrate de tener Npcap instalado y ejecutar la aplicación como administrador.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIniciar.Enabled = false;
                return;
            }

            foreach (var dev in dispositivos)
                cmbInterfaces.Items.Add(dev.Name + " - " + dev.Description); // Agregar al ComboBox

            if (cmbInterfaces.Items.Count > 0)
                cmbInterfaces.SelectedIndex = 0; // Seleccionar la primera interfaz
            btnIniciar.Enabled = true;            // Habilitar el botón Iniciar
        }

        /// <summary>
        /// Inicia la captura de paquetes en la interfaz seleccionada.
        /// </summary>
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que el ComboBox no esté vacío y que haya una selección válida
                if (cmbInterfaces.Items.Count == 0)
                {
                    MessageBox.Show("No hay interfaces de red disponibles.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (cmbInterfaces.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecciona una interfaz de red.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var dispositivos = LibPcapLiveDeviceList.Instance;
                if (cmbInterfaces.SelectedIndex >= dispositivos.Count)
                {
                    MessageBox.Show("Interfaz no disponible. Recargando...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CargarInterfaces();
                    return;
                }
                dispositivo = dispositivos[cmbInterfaces.SelectedIndex];

                // Abrir el dispositivo en modo promiscuo y con timeout de 1000 ms
                dispositivo.Open(DeviceModes.Promiscuous, 1000);
                dispositivo.Filter = "ip";   // Capturar solo tráfico IPv4
                dispositivo.OnPacketArrival += Dispositivo_OnPacketArrival; // Asignar evento
                dispositivo.StartCapture();

                // Actualizar la interfaz de usuario
                btnIniciar.Enabled = false;
                btnDetener.Enabled = true;
                cmbInterfaces.Enabled = false;

                // Limpiar estructuras de datos previas
                todosLosPaquetes.Clear();
                contadorPaquetes = 0;
                valoresTCP.Clear();
                valoresUDP.Clear();
                valoresICMP.Clear();
                valoresIGMP.Clear();
                etiquetasTiempo.Clear();
                contadorSegundos = 0;

                timerGrafico.Start();        // Iniciar el timer del gráfico
                refreshTimerGrid.Start();
                lblEstadisticas.Text = "Capturando... Paquetes: 0";
                ActualizarEstadisticasFiltro(); // Inicializar estadísticas
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar captura: " + ex.Message);
            }
        }

        /// <summary>
        /// Detiene la captura de paquetes y los timers asociados.
        /// </summary>
        private void btnDetener_Click(object sender, EventArgs e) => DetenerCaptura();

        private void DetenerCaptura()
        {
            timerGrafico.Stop();
            refreshTimerGrid.Stop();   // Detener el refresco periódico del DataGridView
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
        /// Manejador del evento que se dispara al llegar un nuevo paquete.
        /// Analiza el paquete, extrae la información, guarda en BD y actualiza la UI.
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

                    // Análisis según el protocolo
                    if (ipPacket.Protocol == ProtocolType.Tcp)
                    {
                        tcpPorSegundo++;
                        var tcpPacket = paquete.Extract<TcpPacket>();
                        if (tcpPacket != null)
                        {
                            paqueteInfo.PuertoOrigen = tcpPacket.SourcePort;
                            paqueteInfo.PuertoDestino = tcpPacket.DestinationPort;
                            paqueteInfo.InformacionAdicional = $"TCP: Flags={tcpPacket.Flags}";

                            // Llamar a los métodos de detección de anomalías
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

                // Guardar el paquete en la base de datos (en segundo plano)
                GuardarPaqueteEnBD(paqueteInfo);

                // Almacenar el paquete en la lista en memoria (máximo 1000)
                lock (todosLosPaquetes)
                {
                    todosLosPaquetes.Insert(0, paqueteInfo);
                    if (todosLosPaquetes.Count > 1000)
                        todosLosPaquetes.RemoveAt(todosLosPaquetes.Count - 1);
                }

                // Actualizar la interfaz de usuario (asincrónico)
                ActualizarUISeguro();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error procesando paquete: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserta un registro en la tabla `paquetes` de MySQL.
        /// </summary>
        private void GuardarPaqueteEnBD(PaqueteInfo p)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO paquetes (hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional) 
                                    VALUES (@hora, @ip_origen, @ip_destino, @protocolo, @puerto_origen, @puerto_destino, @tamaño, @info)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
        }

        /// <summary>
        /// Actualiza la interfaz de usuario de forma segura desde hilos secundarios.
        /// Solo actualiza el contador de paquetes y marca que hay paquetes pendientes.
        /// El refresco real del DataGridView se hace periódicamente (timer refreshTimerGrid).
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
        /// Evento del timer que se ejecuta cada segundo. Actualiza el gráfico,
        /// las estadísticas en tiempo real, alimenta las colas para detección adaptativa y ejecuta las detecciones.
        /// </summary>
        private void TimerGrafico_Tick(object sender, EventArgs e)
        {
            // Agregar los valores actuales a cada serie del gráfico
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

            // Asignar los nuevos valores a las series del gráfico
            if (chartTrafico.Series.Count >= 4)
            {
                chartTrafico.Series[0].Values = valoresTCP;
                chartTrafico.Series[1].Values = valoresUDP;
                chartTrafico.Series[2].Values = valoresICMP;
                chartTrafico.Series[3].Values = valoresIGMP;
            }
            if (chartTrafico.AxisX.Count > 0)
                chartTrafico.AxisX[0].Labels = etiquetasTiempo;

            // Actualizar el label de estadísticas en tiempo real
            ActualizarEstadisticasTiempoReal();

            // Alimentar las colas para la detección adaptativa (máximo 60 muestras)
            historialPaquetes.Enqueue(paquetesPorSegundo);
            if (historialPaquetes.Count > 60) historialPaquetes.Dequeue();
            historialICMP.Enqueue(icmpPorSegundo);
            if (historialICMP.Count > 60) historialICMP.Dequeue();

            // Ejecutar las detecciones adaptativas
            DetectarPicoTraficoAdaptativo();
            DetectarICMPFloodAdaptativo();

            // Resetear los contadores por segundo para el siguiente intervalo
            paquetesPorSegundo = 0;
            tcpPorSegundo = 0;
            udpPorSegundo = 0;
            icmpPorSegundo = 0;
            igmpPorSegundo = 0;
            contadorSegundos++;
        }

        /// <summary>
        /// Filtra los caracteres permitidos en campos de IP: solo dígitos, punto y teclas de control.
        /// </summary>
       
        private void TxtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir dígitos, punto y teclas de control (backspace, suprimir, etc.)
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;   // Cancela la entrada
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Valida el formato completo de la IP cuando el usuario sale del campo.
        /// </summary>
        private void TxtIP_Leave(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (!string.IsNullOrEmpty(txt.Text) && !EsIPValida(txt.Text))
            {
                MessageBox.Show("Formato de IP inválido. Ejemplo: 192.168.1.1", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txt.Focus();
            }
        }

        /// <summary>
        /// Verifica si una cadena tiene formato de IPv4 válido (cuatro números entre 0 y 255 separados por puntos).
        /// </summary>
        private bool EsIPValida(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return true;   // El campo vacío es válido (sin filtro)
            string[] octetos = ip.Split('.');
            if (octetos.Length != 4) return false;
            foreach (string oct in octetos)
            {
                if (!int.TryParse(oct, out int num) || num < 0 || num > 255)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Actualiza el label de estadísticas en tiempo real con los valores actuales.
        /// </summary>
        private void ActualizarEstadisticasTiempoReal()
        {
            if (lblEstadisticasTiempoReal.InvokeRequired)
                lblEstadisticasTiempoReal.Invoke(new Action(ActualizarEstadisticasTiempoReal));
            else
                lblEstadisticasTiempoReal.Text = $"TCP: {tcpPorSegundo}/s | UDP: {udpPorSegundo}/s | ICMP: {icmpPorSegundo}/s | IGMP: {igmpPorSegundo}/s | Total: {paquetesPorSegundo}/s";
        }

        #endregion

        #region Detección de Anomalías

        // ----- DETECCIÓN ADAPTATIVA (basada en media y desviación estándar) -----

        /// <summary>
        /// Detecta picos anómalos de tráfico (paquetes totales por segundo) usando línea base dinámica.
        /// Necesita al menos 10 muestras en el historial.
        /// </summary>
        private void DetectarPicoTraficoAdaptativo()
        {
            if (!alertasActivas) return;
            if (historialPaquetes.Count < 10) return;

            double media = historialPaquetes.Average();
            double desviacion = Math.Sqrt(historialPaquetes.Select(v => Math.Pow(v - media, 2)).Average());
            double umbral = media + factorSigma * desviacion;

            if (paquetesPorSegundo > umbral && paquetesPorSegundo > 50 &&
                DateTime.Now - ultimaAlertaTrafico > cooldown)
            {
                string desc = $"Pico anómalo detectado: {paquetesPorSegundo} paq/s (media={media:F1}, sigma={desviacion:F1})";
                alertaHelper.GuardarAlerta("PICO_ADAPTATIVO", desc, "Media", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                ultimaAlertaTrafico = DateTime.Now;
            }
        }

        /// <summary>
        /// Detecta inundaciones anómalas de paquetes ICMP usando línea base dinámica.
        /// </summary>
        private void DetectarICMPFloodAdaptativo()
        {
            if (!alertasActivas) return;
            if (historialICMP.Count < 10) return;

            double media = historialICMP.Average();
            double desviacion = Math.Sqrt(historialICMP.Select(v => Math.Pow(v - media, 2)).Average());
            double umbral = media + factorSigma * desviacion;

            if (icmpPorSegundo > umbral && icmpPorSegundo > 10 &&
                DateTime.Now - ultimaAlertaICMP > cooldown)
            {
                string desc = $"ICMP flood anómalo: {icmpPorSegundo} icmp/s (media={media:F1}, sigma={desviacion:F1})";
                alertaHelper.GuardarAlerta("ICMP_FLOOD_ADAPT", desc, "Media", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                ultimaAlertaICMP = DateTime.Now;
            }
        }

        // ----- DETECCIÓN CON UMBRALES FIJOS -----

        /// <summary>
        /// Detecta escaneo de puertos horizontal (una IP consulta muchos puertos distintos en un periodo de tiempo).
        /// </summary>
        private void DetectarEscaneoPuertosMejorado(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;
            DateTime ahora = DateTime.Now;
            if (!intentosEscaneo.ContainsKey(ipOrigen))
                intentosEscaneo[ipOrigen] = new Dictionary<int, DateTime>();
            intentosEscaneo[ipOrigen][puertoDestino] = ahora;

            // Eliminar puertos que quedaron fuera de la ventana de tiempo
            var viejos = intentosEscaneo[ipOrigen].Where(kvp => (ahora - kvp.Value).TotalSeconds > ventanaEscaneoSegundos).Select(kvp => kvp.Key).ToList();
            foreach (var p in viejos) intentosEscaneo[ipOrigen].Remove(p);

            if (intentosEscaneo[ipOrigen].Count >= umbralEscaneo)
            {
                string desc = $"Posible escaneo de puertos desde {ipOrigen}. Puertos distintos en últimos {ventanaEscaneoSegundos}s: {intentosEscaneo[ipOrigen].Count}.";
                alertaHelper.GuardarAlerta("ESCANEO_PUERTOS", desc, "Alta", ipOrigen);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosEscaneo[ipOrigen].Clear();
            }
        }

        /// <summary>
        /// Detecta ataques de fuerza bruta (múltiples intentos de conexión a puertos sensibles en poco tiempo).
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
            // Eliminar intentos fuera de la ventana de tiempo
            intentosFuerzaBruta[ipOrigen][puertoDestino].RemoveAll(t => (DateTime.Now - t).TotalSeconds > ventanaFuerzaBrutaSegundos);

            if (intentosFuerzaBruta[ipOrigen][puertoDestino].Count >= umbralFuerzaBruta)
            {
                string desc = $"Posible ataque de fuerza bruta desde {ipOrigen} al puerto {puertoDestino}. " +
                              $"{intentosFuerzaBruta[ipOrigen][puertoDestino].Count} intentos en {ventanaFuerzaBrutaSegundos}s.";
                alertaHelper.GuardarAlerta("FUERZA_BRUTA", desc, "Alta", ipOrigen);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosFuerzaBruta[ipOrigen][puertoDestino].Clear();
            }
        }

        /// <summary>
        /// Detecta escaneo vertical (muchas IPs diferentes atacando el mismo puerto).
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
                alertaHelper.GuardarAlerta("ESCANEO_VERTICAL", desc, "Alta", null);
                MostrarNotificacionEmergente(desc, Color.FromArgb(200, 50, 50));
                intentosEscaneoVertical[puertoDestino].Clear();
            }
        }

        /// <summary>
        /// Limpia periódicamente las estructuras de detección para liberar memoria.
        /// Elimina entradas con timestamps anteriores al límite.
        /// </summary>
        private void LimpiarEstructurasAntiguas()
        {
            DateTime limite = DateTime.Now.AddSeconds(-Math.Max(ventanaEscaneoSegundos, Math.Max(ventanaFuerzaBrutaSegundos, ventanaEscaneoVerticalSegundos)) - 10);
            // Limpiar intentosEscaneo
            foreach (var ip in intentosEscaneo.Keys.ToList())
            {
                var viejos = intentosEscaneo[ip].Where(kvp => kvp.Value < limite).Select(kvp => kvp.Key).ToList();
                foreach (var p in viejos) intentosEscaneo[ip].Remove(p);
                if (intentosEscaneo[ip].Count == 0) intentosEscaneo.Remove(ip);
            }
            // Limpiar intentosFuerzaBruta
            foreach (var ip in intentosFuerzaBruta.Keys.ToList())
            {
                foreach (var puerto in intentosFuerzaBruta[ip].Keys.ToList())
                {
                    intentosFuerzaBruta[ip][puerto].RemoveAll(t => t < limite);
                    if (intentosFuerzaBruta[ip][puerto].Count == 0)
                        intentosFuerzaBruta[ip].Remove(puerto);
                }
                if (intentosFuerzaBruta[ip].Count == 0) intentosFuerzaBruta.Remove(ip);
            }
            // Limpiar intentosEscaneoVertical
            foreach (var puerto in intentosEscaneoVertical.Keys.ToList())
            {
                foreach (var ip in intentosEscaneoVertical[puerto].Keys.ToList())
                {
                    intentosEscaneoVertical[puerto][ip].RemoveAll(t => t < limite);
                    if (intentosEscaneoVertical[puerto][ip].Count == 0)
                        intentosEscaneoVertical[puerto].Remove(ip);
                }
                if (intentosEscaneoVertical[puerto].Count == 0) intentosEscaneoVertical.Remove(puerto);
            }
        }

        #endregion

        #region Filtros y UI

        /// <summary>
        /// Aplica los filtros actuales (protocolo, IP origen, IP destino) sobre la lista de paquetes.
        /// Utiliza una copia segura de la lista (bajo lock) para evitar errores de modificación concurrente.
        /// </summary>
        private void AplicarFiltros()
        {
            // Copia segura de la lista de paquetes
            List<PaqueteInfo> copia;
            lock (todosLosPaquetes)
            {
                copia = new List<PaqueteInfo>(todosLosPaquetes);
            }

            string filtroProto = cmbProtocolo.SelectedItem?.ToString() ?? "Todos";
            string filtroOrigen = txtFiltroIPOrigen.Text.Trim().ToLower();
            string filtroDestino = txtFiltroIPDestino.Text.Trim().ToLower();

            var paquetesFiltrados = copia.AsEnumerable();

            if (filtroProto != "Todos")
                paquetesFiltrados = paquetesFiltrados.Where(p => p.Protocolo != null && p.Protocolo.Equals(filtroProto, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(filtroOrigen))
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPOrigen != null && p.IPOrigen.ToLower().Contains(filtroOrigen));
            if (!string.IsNullOrEmpty(filtroDestino))
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPDestino != null && p.IPDestino.ToLower().Contains(filtroDestino));

            MostrarPaquetesEnGrid(paquetesFiltrados.ToList());
            ActualizarEstadisticasFiltro(copia);
        }

        /// <summary>
        /// Muestra hasta 100 paquetes en el DataGridView.
        /// </summary>
        private void MostrarPaquetesEnGrid(List<PaqueteInfo> paquetes)
        {
            dgvPaquetes.Rows.Clear();
            foreach (var p in paquetes.Take(100))
            {
                dgvPaquetes.Rows.Add(
                    p.Hora,
                    p.IPOrigen ?? "N/A",
                    p.IPDestino ?? "N/A",
                    p.Protocolo ?? "N/A",
                    p.PuertoOrigen > 0 ? p.PuertoOrigen.ToString() : "-",
                    p.PuertoDestino > 0 ? p.PuertoDestino.ToString() : "-",
                    p.Tamaño,
                    p.InformacionAdicional ?? "");
            }
        }

        /// <summary>
        /// Actualiza el label de estadísticas del filtro (conteo de protocolos y total).
        /// </summary>
        private void ActualizarEstadisticasFiltro(List<PaqueteInfo> copia)
        {
            int tcp = copia.Count(p => p.Protocolo == "TCP");
            int udp = copia.Count(p => p.Protocolo == "UDP");
            int icmp = copia.Count(p => p.Protocolo == "ICMP");
            int igmp = copia.Count(p => p.Protocolo == "IGMP");
            int total = copia.Count;

            if (lblEstadisticasFiltro.InvokeRequired)
                lblEstadisticasFiltro.Invoke(new Action(() => lblEstadisticasFiltro.Text = $"TCP: {tcp} | UDP: {udp} | ICMP: {icmp} | IGMP: {igmp} | Total: {total}"));
            else
                lblEstadisticasFiltro.Text = $"TCP: {tcp} | UDP: {udp} | ICMP: {icmp} | IGMP: {igmp} | Total: {total}";
        }

        // Evento necesario para evitar error del diseñador (Click del lblEstadisticas)
        private void lblEstadisticas_Click(object sender, EventArgs e) { }

        // Método sobrecargado para mantener compatibilidad con llamadas sin parámetros
        private void ActualizarEstadisticasFiltro()
        {
            lock (todosLosPaquetes)
            {
                ActualizarEstadisticasFiltro(new List<PaqueteInfo>(todosLosPaquetes));
            }
        }

        /// <summary>
        /// Colorea las filas del DataGridView según el protocolo y el puerto destino.
        /// </summary>
        private void DgvPaquetes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value != null)
            {
                string protocolo = dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value.ToString();
                string puerto = dgvPaquetes.Rows[e.RowIndex].Cells["PuertoDestino"].Value?.ToString() ?? "0";

                if (protocolo == "TCP" && puerto == "80")
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue;    // HTTP
                else if (protocolo == "TCP" && puerto == "443")
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;   // HTTPS
                else if (protocolo == "TCP" && (puerto == "22" || puerto == "23"))
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;    // SSH / Telnet
            }
        }

        /// <summary>
        /// Evento del botón "Aplicar" de filtros.
        /// </summary>
        private void btnAplicarFiltro_Click(object sender, EventArgs e) => AplicarFiltros();

        /// <summary>
        /// Evento del botón "Limpiar" de filtros (restablece los valores por defecto).
        /// </summary>
        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            cmbProtocolo.SelectedIndex = 0;
            txtFiltroIPOrigen.Text = "";
            txtFiltroIPDestino.Text = "";
            AplicarFiltros();
        }

        #endregion

        #region Configuración (Ventana de Configuración)

        /// <summary>
        /// Abre la ventana de configuración de alertas (factor sigma y activación).
        /// </summary>
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

        #endregion

        #region Exportación a CSV

        // Exportar paquetes a CSV
        private void ExportarPaquetesMenu_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Exportar Paquetes a CSV";
                sfd.FileName = $"paquetes_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                    ExportarPaquetesACSV(sfd.FileName);
            }
        }

        // Exportar alertas a CSV
        private void ExportarAlertasMenu_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.Title = "Exportar Alertas a CSV";
                sfd.FileName = $"alertas_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                    ExportarAlertasACSV(sfd.FileName);
            }
        }

        private void ExportarPaquetesACSV(string ruta)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional FROM paquetes ORDER BY hora DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ruta))
                    {
                        sw.WriteLine("Hora,IP Origen,IP Destino,Protocolo,Puerto Origen,Puerto Destino,Tamaño (bytes),Información Adicional");
                        while (reader.Read())
                        {
                            sw.WriteLine($"{reader["hora"]},{reader["ip_origen"]},{reader["ip_destino"]},{reader["protocolo"]},{reader["puerto_origen"]},{reader["puerto_destino"]},{reader["tamaño"]},\"{reader["informacion_adicional"]}\"");
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

        private void ExportarAlertasACSV(string ruta)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT tipo, descripcion, severidad, ip_involucrada, timestamp FROM alertas ORDER BY timestamp DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ruta))
                    {
                        sw.WriteLine("Tipo,Descripción,Severidad,IP Involucrada,Fecha y Hora");
                        while (reader.Read())
                        {
                            sw.WriteLine($"{reader["tipo"]},\"{reader["descripcion"]}\",{reader["severidad"]},{reader["ip_involucrada"]},{reader["timestamp"]}");
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

        #endregion

        #region Modo Oscuro / Claro

        // Alterna entre tema oscuro y claro
        private void ToggleModoOscuro(object sender, EventArgs e)
        {
            esModoOscuro = !esModoOscuro;
            if (esModoOscuro) AplicarTemaOscuro();
            else AplicarTemaClaro();
        }

        // Aplica el tema oscuro (tonos grises y azules oscuros)
        private void AplicarTemaOscuro()
        {
            this.BackColor = Color.FromArgb(30, 30, 35);
            this.ForeColor = Color.WhiteSmoke;
            headerPanel.BackColor = Color.FromArgb(45, 45, 48);

            foreach (var gb in new[] { gbCaptura, gbFiltros })
            {
                if (gb == null) continue;
                gb.BackColor = Color.FromArgb(40, 40, 45);
                gb.ForeColor = Color.WhiteSmoke;
                foreach (Control ctrl in gb.Controls) AplicarColorOscuroControl(ctrl);
            }

            dgvPaquetes.BackgroundColor = Color.FromArgb(50, 50, 55);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 70, 75);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPaquetes.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgvPaquetes.DefaultCellStyle.ForeColor = Color.WhiteSmoke;
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(75, 75, 85);
            dgvPaquetes.GridColor = Color.FromArgb(80, 80, 85);
            dgvPaquetes.EnableHeadersVisualStyles = false;

            chartTrafico.BackColor = Color.FromArgb(40, 40, 45);

            lblEstadisticas.ForeColor = Color.WhiteSmoke;
            lblEstadisticasTiempoReal.ForeColor = Color.WhiteSmoke;
            lblEstadisticasFiltro.ForeColor = Color.WhiteSmoke;

            BtnConfig.BackColor = Color.FromArgb(60, 60, 65);
            BtnConfig.ForeColor = Color.WhiteSmoke;
            btnVerAlertas.BackColor = Color.FromArgb(60, 60, 65);
            btnVerAlertas.ForeColor = Color.WhiteSmoke;
            btnAplicarFiltro.BackColor = Color.FromArgb(0, 102, 204);
            btnAplicarFiltro.ForeColor = Color.White;
            btnLimpiarFiltros.BackColor = Color.FromArgb(80, 80, 85);
            btnLimpiarFiltros.ForeColor = Color.WhiteSmoke;

            menuStrip.BackColor = Color.FromArgb(45, 45, 48);
            menuStrip.ForeColor = Color.WhiteSmoke;
        }

        // Aplica el tema claro (colores originales)
        private void AplicarTemaClaro()
        {
            this.BackColor = Color.FromArgb(240, 248, 255);
            this.ForeColor = Color.Black;
            headerPanel.BackColor = Color.FromArgb(0, 102, 204);

            foreach (var gb in new[] { gbCaptura, gbFiltros })
            {
                if (gb == null) continue;
                gb.BackColor = Color.White;
                gb.ForeColor = Color.FromArgb(0, 51, 102);
                foreach (Control ctrl in gb.Controls) AplicarColorClaroControl(ctrl);
            }

            dgvPaquetes.BackgroundColor = Color.White;
            dgvPaquetes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgvPaquetes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPaquetes.DefaultCellStyle.BackColor = Color.White;
            dgvPaquetes.DefaultCellStyle.ForeColor = Color.Black;
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPaquetes.GridColor = Color.LightGray;

            chartTrafico.BackColor = Color.Transparent;

            lblEstadisticas.ForeColor = Color.FromArgb(0, 51, 102);
            lblEstadisticasTiempoReal.ForeColor = Color.Black;
            lblEstadisticasFiltro.ForeColor = Color.Black;

            BtnConfig.BackColor = Color.FromArgb(0, 102, 204);
            BtnConfig.ForeColor = Color.White;
            btnVerAlertas.BackColor = Color.FromArgb(0, 102, 204);
            btnVerAlertas.ForeColor = Color.White;
            btnAplicarFiltro.BackColor = Color.FromArgb(0, 102, 204);
            btnAplicarFiltro.ForeColor = Color.White;
            btnLimpiarFiltros.BackColor = Color.LightGray;
            btnLimpiarFiltros.ForeColor = Color.Black;

            menuStrip.BackColor = SystemColors.Control;
            menuStrip.ForeColor = Color.Black;
        }

        // Aplica colores oscuros a un control específico (utilizado recursivamente)
        private void AplicarColorOscuroControl(Control ctrl)
        {
            if (ctrl is Label lbl) lbl.ForeColor = Color.LightGray;
            else if (ctrl is TextBox txt)
            {
                txt.BackColor = Color.FromArgb(70, 70, 75);
                txt.ForeColor = Color.LightGray;
            }
            else if (ctrl is CheckBox chk) chk.ForeColor = Color.LightGray;
            else if (ctrl is ComboBox cb)
            {
                cb.BackColor = Color.FromArgb(70, 70, 75);
                cb.ForeColor = Color.LightGray;
            }
        }

        // Restaura los colores claros a un control
        private void AplicarColorClaroControl(Control ctrl)
        {
            if (ctrl is Label lbl) lbl.ForeColor = Color.Black;
            else if (ctrl is TextBox txt)
            {
                txt.BackColor = Color.White;
                txt.ForeColor = Color.Black;
            }
            else if (ctrl is CheckBox chk) chk.ForeColor = Color.Black;
            else if (ctrl is ComboBox cb)
            {
                cb.BackColor = Color.White;
                cb.ForeColor = Color.Black;
            }
        }

        #endregion

        #region Notificaciones y Eventos Varios

        // Muestra una notificación emergente (toast) en la esquina superior derecha
        private void MostrarNotificacionEmergente(string mensaje, Color colorFondo, int duracionMs = 3000)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MostrarNotificacionEmergente(mensaje, colorFondo, duracionMs)));
                return;
            }
            FrmToast.Mostrar(mensaje, colorFondo, duracionMs);
        }

        // Abre el formulario de historial de alertas
        private void btnVerAlertas_Click(object sender, EventArgs e)
        {
            HistorialAlertas frm = new HistorialAlertas();
            frm.ShowDialog();
        }

        // Cierra la aplicación desde el menú
        private void SalirMenu_Click(object sender, EventArgs e) => this.Close();

        // Enfoca el ComboBox de interfaces (desde menú)
        private void SeleccionarInterfazMenu_Click(object sender, EventArgs e) => cmbInterfaces.Focus();

        // Evento que se ejecuta al cerrar el formulario: detiene timers
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimerGrid?.Stop();
            timerGrafico?.Stop();
            base.OnFormClosing(e);
        }

        #endregion

        #region Clase Auxiliar (AlertaHelper)

        /// <summary>
        /// Clase auxiliar para insertar alertas en la base de datos MySQL.
        /// </summary>
        public class AlertaHelper
        {
            private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";
            public void GuardarAlerta(string tipo, string descripcion, string severidad, string ipInvolucrada = null)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"INSERT INTO alertas (tipo, descripcion, severidad, ip_involucrada, timestamp) VALUES(@tipo, @descripcion, @severidad, @ip, @timestamp)";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
            }
        }

        #endregion

        // Evento vacío del título (no usado pero necesario si está enlazado en el diseñador)
        private void titleLabel_Click(object sender, EventArgs e) { }
    }
}