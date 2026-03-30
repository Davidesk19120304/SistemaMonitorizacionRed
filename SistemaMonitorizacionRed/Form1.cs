using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using MySql.Data.MySqlClient;

namespace SistemaMonitorizacionRed
{
    public partial class Form1 : Form
    {
        private LibPcapLiveDevice dispositivo;
        private int contadorPaquetes = 0;

        //Lista para Almacenar todos los paquetes capturados
        private List<PaqueteInfo> todosLosPaquetes = new List<PaqueteInfo>();

        //Para el grafico en tiempo real
        private ChartValues<int> valoresTrafico = new ChartValues<int>();
        private List<string> etiquetasTiempo = new List<string>();
        private int contadorSegundos = 0;
        private Timer timerGrafico;

        //contadores para estadisticas en tiempo real
        private int paquetesPorSegundo = 0;
        private int tcpPorSegundo = 0;
        private int udpPorSegundo = 0;
        private int icmpPorSegundo = 0;

        //Sistema de alertas
        private AlertaHelper alertaHelper = new AlertaHelper();
        private int umbralEscaneo = 10;
        private int umbralTrafico = 500;
        private bool alertasActivas = true;

        // ========== NUEVAS ESTRUCTURAS PARA DETECCIÓN DE ANOMALÍAS ==========
        // Escaneo horizontal mejorado (con ventana de tiempo)
        private Dictionary<string, Dictionary<int, DateTime>> intentosEscaneo = new Dictionary<string, Dictionary<int, DateTime>>();
        private int ventanaEscaneoSegundos = 60;

        // Fuerza bruta
        private Dictionary<string, Dictionary<int, List<DateTime>>> intentosFuerzaBruta = new Dictionary<string, Dictionary<int, List<DateTime>>>();
        private int umbralFuerzaBruta = 10;
        private int ventanaFuerzaBrutaSegundos = 60;

        // Escaneo vertical
        private Dictionary<int, Dictionary<string, List<DateTime>>> intentosEscaneoVertical = new Dictionary<int, Dictionary<string, List<DateTime>>>();
        private int umbralEscaneoVertical = 10;
        private int ventanaEscaneoVerticalSegundos = 60;

        // ICMP flood
        private int umbralICMPFlood = 100;
        private bool alertasICMPFlood = true;

        // Timer para limpieza general de estructuras
        private Timer timerLimpiezaGeneral;
        // ================================================================

        //Clave para Almacenar informacion de paquetes
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

        //Cadena de conexion a MySQL
        private string connectionString = "Server=localhost;Database=monitorizacion_red;Uid=root;Pwd=;";
        public Form1()
        {
            InitializeComponent();

            ConfigurarDataGridView();
            ConfigurarControlesFiltro();
            ConfigurarGrafico();

            //Deshabilita la fila de nuevo registro en el Dgv
            dgvPaquetes.AllowUserToAddRows = false;
            dgvPaquetes.CellFormatting += DgvPaquetes_CellFormatting;

            // Inicializar timer de limpieza general (cada minuto)
            timerLimpiezaGeneral = new Timer();
            timerLimpiezaGeneral.Interval = 60000; // 60 segundos
            timerLimpiezaGeneral.Tick += (s, e) => LimpiarEstructurasAntiguas();
            timerLimpiezaGeneral.Start();
        }

        private void ConfigurarGrafico()
        {
            //configurar el grafico
            chartTrafico.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Paquetes/segundo",
                    Values = valoresTrafico,
                    PointGeometry = null,
                    StrokeThickness = 2,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = System.Windows.Media.Brushes.DodgerBlue
                }
            };
            //configurar ejers
            chartTrafico.AxisX.Add(new Axis
            {
                Title = "Tiempo (segundos)",
                Labels = etiquetasTiempo,
                DisableAnimations = true
            });

            chartTrafico.AxisY.Add(new Axis
            {
                Title = "Paquetes",
                MinValue = 0,
                DisableAnimations = true
            });

            chartTrafico.LegendLocation = LegendLocation.Top;

            //timer para actualizar el grafico cada segundo
            timerGrafico = new Timer();
            timerGrafico.Interval = 1000; // 1 segundo
            timerGrafico.Tick += TimerGrafico_Tick;
        }

        private void TimerGrafico_Tick(object sender, EventArgs e)
        {
            //Agregar valor actual al grafico
            valoresTrafico.Add(paquetesPorSegundo);
            etiquetasTiempo.Add(contadorSegundos.ToString());

            //mantener solo los ultimos 30 seg en el trafico
            if (valoresTrafico.Count > 30)
            {
                valoresTrafico.RemoveAt(0);
                etiquetasTiempo.RemoveAt(0);
            }

            //Actualizar etiquetas del eje x
            if (chartTrafico.AxisX.Count > 0)
            {
                chartTrafico.AxisX[0].Labels = etiquetasTiempo;
            }

            //Actualizar estadisticas detalladas
            ActualizarEstadisticasTiempoReal();

            //Detectar pico de trafico
            DetectarPicoTrafico();
            // Detectar ICMP flood
            DetectarICMPFlood();

            //Resetear contadores por segundo
            paquetesPorSegundo = 0;
            tcpPorSegundo = 0;
            udpPorSegundo = 0;
            icmpPorSegundo = 0;
            contadorSegundos++;
        }
        private void ActualizarEstadisticasTiempoReal()
        {
            if (lblEstadisticasTiempoReal.InvokeRequired)
            {
                lblEstadisticasTiempoReal.Invoke(new Action(ActualizarEstadisticasTiempoReal));
                return;
            }
            lblEstadisticasTiempoReal.Text = $"TCP: {tcpPorSegundo}/s | UDP: {udpPorSegundo}/s | ICMP: {icmpPorSegundo}/s | " +
                $"Total: {paquetesPorSegundo}/s";
        }
        private void ConfigurarDataGridView()
        {
            //Limpiar columnas existentes
            dgvPaquetes.Columns.Clear();

            //Configurar Columnas
            dgvPaquetes.Columns.Add("Hora", "Hora");
            dgvPaquetes.Columns.Add("Origen", "IPOrigen");
            dgvPaquetes.Columns.Add("Destino", "IPDestino");
            dgvPaquetes.Columns.Add("Protocolo", "Protocolo");
            dgvPaquetes.Columns.Add("PuertoOrigen", "Puerto Origen");
            dgvPaquetes.Columns.Add("PuertoDestino", "Puerto Destino ");
            dgvPaquetes.Columns.Add("Tamaño", "Tamaño (bytes)");
            dgvPaquetes.Columns.Add("Info", "Informacion");

            //Ajustar anchos
            dgvPaquetes.Columns["Hora"].Width = 80;
            dgvPaquetes.Columns["Origen"].Width = 120;
            dgvPaquetes.Columns["Destino"].Width = 120;
            dgvPaquetes.Columns["Protocolo"].Width = 70;
            dgvPaquetes.Columns["PuertoOrigen"].Width = 70;
            dgvPaquetes.Columns["PuertoDestino"].Width = 70;
            dgvPaquetes.Columns["Tamaño"].Width = 70;
            dgvPaquetes.Columns["Info"].Width = 200;

            //Configurar color de fondo alternado
            dgvPaquetes.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvPaquetes.RowHeadersVisible = false;
        }

        private void ConfigurarControlesFiltro()
        {

            //Configurar ComboBox de protocolos
            cmbProtocolo.Items.Clear();
            cmbProtocolo.Items.Add("Todos");
            cmbProtocolo.Items.Add("TCP");
            cmbProtocolo.Items.Add("UDP");
            cmbProtocolo.Items.Add("ICMP");
            cmbProtocolo.SelectedIndex = 0; // "TODOS" por defecto
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarInterfaces();
            btnDetener.Enabled = false;

            //Cargar configuracion de alertas
            CargarConfiguracion();
            lblUltimaAlerta.Text = "✅ Sin alertas";
            lblUltimaAlerta.ForeColor = Color.Green;
        }

        private void CargarInterfaces()
        {
            cmbInterfaces.Items.Clear();
            var dispositivos = LibPcapLiveDeviceList.Instance;

            if (dispositivos.Count == 0)
            {
                MessageBox.Show("No se encontraron interfaces de red. Asegúrate de tener Npcap instalado y ejecutar la aplicación como administrador.",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIniciar.Enabled = false;
                return;
            }

            foreach (var dev in dispositivos)
            {
                cmbInterfaces.Items.Add(dev.Name + " - " + dev.Description);
            }

            if (cmbInterfaces.Items.Count > 0)
                cmbInterfaces.SelectedIndex = 0;
            btnIniciar.Enabled = true;
        }

        private void lblEstadisticas_Click(object sender, EventArgs e)
        {

        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones robustas para evitar errores de índice
                if (cmbInterfaces.Items.Count == 0)
                {
                    MessageBox.Show("No hay interfaces de red disponibles. Asegúrate de tener Npcap instalado y ejecutar la app como administrador.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (cmbInterfaces.SelectedIndex == -1)
                {
                    MessageBox.Show("Selecciona una interfaz de red de la lista.",
                                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Obtener el dispositivo seleccionado
                var dispositivos = LibPcapLiveDeviceList.Instance;
                if (cmbInterfaces.SelectedIndex >= dispositivos.Count)
                {
                    MessageBox.Show("La interfaz seleccionada ya no está disponible. Recargando lista...",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CargarInterfaces();
                    return;
                }
                dispositivo = dispositivos[cmbInterfaces.SelectedIndex];

                //Abrir Dispositivo
                dispositivo.Open(DeviceModes.Promiscuous, 1000);

                //Configurar Filtro (opcional: solo capturar IPv4)
                dispositivo.Filter = "ip";

                //Asociar evento para cuando llegue un paquete
                dispositivo.OnPacketArrival += Dispositivo_OnPacketArrival;

                //Iniciar Captura
                dispositivo.StartCapture();

                //Actualizar UI
                btnIniciar.Enabled = false;
                btnDetener.Enabled = true;
                cmbInterfaces.Enabled = false;

                todosLosPaquetes.Clear();
                contadorPaquetes = 0;
                valoresTrafico.Clear();
                etiquetasTiempo.Clear();
                contadorSegundos = 0;

                timerGrafico.Start();

                lblEstadisticas.Text = "Capturando... Paquetes: 0";
                ActualizarEstadisticasFiltro();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al Iniciar captura: " + ex.Message);
            }
        }

        private void btnDetener_Click(object sender, EventArgs e)
        {
            DetenerCaptura();
        }

        private void DetenerCaptura()
        {
            timerGrafico.Stop();

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

        private void Dispositivo_OnPacketArrival(Object sender, PacketCapture e)
        {

            try
            {
                var paquete = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);
                contadorPaquetes++;

                //Incrementar contadores por segundo
                paquetesPorSegundo++;

                var paqueteInfo = new PaqueteInfo
                {
                    Hora = DateTime.Now.ToString("HH:mm:ss.fff"),
                    Tamaño = e.GetPacket().Data.Length
                };

                //Analizar paquete IP
                var ipPacket = paquete.Extract<IPPacket>();
                if (ipPacket != null)
                {
                    paqueteInfo.IPOrigen = ipPacket.SourceAddress.ToString();
                    paqueteInfo.IPDestino = ipPacket.DestinationAddress.ToString();
                    paqueteInfo.Protocolo = ipPacket.Protocol.ToString();

                    //Contar por Protocolo
                    if (ipPacket.Protocol == ProtocolType.Tcp)
                    {
                        tcpPorSegundo++;

                        //Analizar protocolos de transporte
                        var tcpPacket = paquete.Extract<TcpPacket>();
                        if (tcpPacket != null)
                        {
                            paqueteInfo.PuertoOrigen = tcpPacket.SourcePort;
                            paqueteInfo.PuertoDestino = tcpPacket.DestinationPort;
                            paqueteInfo.InformacionAdicional = $"TCP: Flags={tcpPacket.Flags}";

                            // Detección básica de escaneo (mejorada con ventana de tiempo)
                            DetectarEscaneoPuertosMejorado(paqueteInfo.IPOrigen, tcpPacket.DestinationPort);
                            // Detección de fuerza bruta
                            DetectarFuerzaBruta(paqueteInfo.IPOrigen, tcpPacket.DestinationPort);
                            // Detección de escaneo vertical
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

                        // Para IPv4
                        var icmpV4 = paquete.Extract<IcmpV4Packet>();
                        if (icmpV4 != null)
                        {
                            paqueteInfo.InformacionAdicional = $"ICMPv4 Type={icmpV4.TypeCode}";
                        }
                        else
                        {
                            // Para IPv6
                            var icmpV6 = paquete.Extract<IcmpV6Packet>();
                            if (icmpV6 != null)
                            {
                                paqueteInfo.InformacionAdicional = $"ICMPv6 Type={icmpV6.Type}";
                            }
                            else
                            {
                                paqueteInfo.InformacionAdicional = "ICMP (otros)";
                            }
                        }
                    }
                }

                GuardarPaqueteEnBD(paqueteInfo);

                //Guardar en la lista
                lock (todosLosPaquetes)
                {
                    todosLosPaquetes.Insert(0, paqueteInfo);

                    //Mantener solo los ultimos 1000 paquetes en memoria
                    if (todosLosPaquetes.Count > 1000)
                    {
                        todosLosPaquetes.RemoveAt(todosLosPaquetes.Count - 1);
                    }
                }

                //Actualizar UI de manera segura
                ActualizarUISeguro();
            }
            catch (Exception ex)
            {
                //Silenciar errore para no interrumpir la captura
                System.Diagnostics.Debug.WriteLine("Error procesando paquete: " + ex.Message);
            }
        }

        //Metodo para guardar paquete en MySQL
        private void GuardarPaqueteEnBD(PaqueteInfo p)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO paquetes 
                                    (hora, ip_origen, ip_destino, protocolo, puerto_origen, puerto_destino, tamaño, informacion_adicional) 
                                    VALUES 
                                    (@hora, @ip_origen, @ip_destino, @protocolo, @puerto_origen, @puerto_destino, @tamaño, @info)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
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
            catch (Exception ex)
            {
                //No interrumpir la captura por error de BD, solo loguear
                System.Diagnostics.Debug.WriteLine("Error guardando en BD: " + ex.Message);
            }
        }

        // ========== MÉTODOS DE DETECCIÓN DE ANOMALÍAS MEJORADOS ==========

        // Escaneo de puertos mejorado (con ventana de tiempo)
        private void DetectarEscaneoPuertosMejorado(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;
            DateTime ahora = DateTime.Now;

            if (!intentosEscaneo.ContainsKey(ipOrigen))
                intentosEscaneo[ipOrigen] = new Dictionary<int, DateTime>();

            // Agregar o actualizar timestamp del puerto
            intentosEscaneo[ipOrigen][puertoDestino] = ahora;

            // Limpiar puertos fuera de ventana
            var puertosViejos = intentosEscaneo[ipOrigen].Where(kvp => (ahora - kvp.Value).TotalSeconds > ventanaEscaneoSegundos).Select(kvp => kvp.Key).ToList();
            foreach (var p in puertosViejos)
                intentosEscaneo[ipOrigen].Remove(p);

            if (intentosEscaneo[ipOrigen].Count >= umbralEscaneo)
            {
                string descripcion = $"Posible escaneo de puertos desde {ipOrigen}. Puertos distintos en los últimos {ventanaEscaneoSegundos} segundos: {intentosEscaneo[ipOrigen].Count}.";
                //Guardar alerta en BD
                alertaHelper.GuardarAlerta(
                    tipo: "ESCANEO_PUERTOS",
                    descripcion: descripcion,
                    severidad: "Alta",
                    ipInvolucrada: ipOrigen
                );
                //Mostrar en el label de ultima alerta
                lblUltimaAlerta.Text = $"⚠️ Alerta: {descripcion}";
                lblUltimaAlerta.ForeColor = Color.Red;
                intentosEscaneo[ipOrigen].Clear();
            }
        }

        // Detección de fuerza bruta
        private void DetectarFuerzaBruta(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;

            // Puertos comunes para ataques de fuerza bruta
            int[] puertosSospechosos = { 21, 22, 23, 3389, 5900, 1433, 3306, 8080 };
            if (!puertosSospechosos.Contains(puertoDestino)) return;

            // Inicializar diccionarios si no existen
            if (!intentosFuerzaBruta.ContainsKey(ipOrigen))
                intentosFuerzaBruta[ipOrigen] = new Dictionary<int, List<DateTime>>();

            if (!intentosFuerzaBruta[ipOrigen].ContainsKey(puertoDestino))
                intentosFuerzaBruta[ipOrigen][puertoDestino] = new List<DateTime>();

            // Agregar timestamp actual
            intentosFuerzaBruta[ipOrigen][puertoDestino].Add(DateTime.Now);

            // Limpiar timestamps fuera de la ventana de tiempo
            intentosFuerzaBruta[ipOrigen][puertoDestino].RemoveAll(t => (DateTime.Now - t).TotalSeconds > ventanaFuerzaBrutaSegundos);

            // Verificar umbral
            if (intentosFuerzaBruta[ipOrigen][puertoDestino].Count >= umbralFuerzaBruta)
            {
                string descripcion = $"Posible ataque de fuerza bruta desde {ipOrigen} al puerto {puertoDestino}. " +
                                     $"{intentosFuerzaBruta[ipOrigen][puertoDestino].Count} intentos en {ventanaFuerzaBrutaSegundos} segundos.";

                // Guardar alerta en BD
                alertaHelper.GuardarAlerta(
                    tipo: "FUERZA_BRUTA",
                    descripcion: descripcion,
                    severidad: "Alta",
                    ipInvolucrada: ipOrigen
                );

                // Mostrar en el label de última alerta
                lblUltimaAlerta.Text = $"⚠️ Alerta: {descripcion}";
                lblUltimaAlerta.ForeColor = Color.Red;

                // Limpiar contadores para esa IP/puerto para no repetir alerta inmediatamente
                intentosFuerzaBruta[ipOrigen][puertoDestino].Clear();
            }
        }

        // Detección de escaneo vertical
        private void DetectarEscaneoVertical(string ipOrigen, int puertoDestino)
        {
            if (!alertasActivas) return;

            // Inicializar diccionarios
            if (!intentosEscaneoVertical.ContainsKey(puertoDestino))
                intentosEscaneoVertical[puertoDestino] = new Dictionary<string, List<DateTime>>();

            if (!intentosEscaneoVertical[puertoDestino].ContainsKey(ipOrigen))
                intentosEscaneoVertical[puertoDestino][ipOrigen] = new List<DateTime>();

            // Agregar timestamp actual
            intentosEscaneoVertical[puertoDestino][ipOrigen].Add(DateTime.Now);

            // Limpiar timestamps fuera de la ventana
            intentosEscaneoVertical[puertoDestino][ipOrigen].RemoveAll(t => (DateTime.Now - t).TotalSeconds > ventanaEscaneoVerticalSegundos);

            // Si la lista se queda vacía, eliminar la IP
            if (intentosEscaneoVertical[puertoDestino][ipOrigen].Count == 0)
                intentosEscaneoVertical[puertoDestino].Remove(ipOrigen);

            // Contar IPs distintas que han atacado este puerto en la ventana
            int ipDistintas = intentosEscaneoVertical[puertoDestino].Keys.Count;

            if (ipDistintas >= umbralEscaneoVertical)
            {
                string descripcion = $"Posible escaneo vertical al puerto {puertoDestino}: {ipDistintas} IPs diferentes en {ventanaEscaneoVerticalSegundos}s.";

                alertaHelper.GuardarAlerta(
                    tipo: "ESCANEO_VERTICAL",
                    descripcion: descripcion,
                    severidad: "Alta",
                    ipInvolucrada: null
                );

                lblUltimaAlerta.Text = $"⚠️ Alerta: {descripcion}";
                lblUltimaAlerta.ForeColor = Color.Red;

                // Limpiar el puerto para no repetir inmediatamente
                intentosEscaneoVertical[puertoDestino].Clear();
            }
        }

        // Detección de ICMP flood
        private void DetectarICMPFlood()
        {
            if (!alertasActivas || !alertasICMPFlood) return;

            if (icmpPorSegundo >= umbralICMPFlood)
            {
                string descripcion = $"Posible ICMP flood: {icmpPorSegundo} paquetes ICMP/s (umbral: {umbralICMPFlood})";

                alertaHelper.GuardarAlerta(
                    tipo: "ICMP_FLOOD",
                    descripcion: descripcion,
                    severidad: "Media",
                    ipInvolucrada: null
                );

                lblUltimaAlerta.Text = $"⚠️ Alerta: {descripcion}";
                lblUltimaAlerta.ForeColor = Color.Red;
            }
        }

        // Limpieza periódica de estructuras para evitar consumo de memoria
        private void LimpiarEstructurasAntiguas()
        {
            DateTime limite = DateTime.Now.AddSeconds(-Math.Max(ventanaEscaneoSegundos, Math.Max(ventanaFuerzaBrutaSegundos, ventanaEscaneoVerticalSegundos)) - 10);

            // Limpiar intentosEscaneo
            foreach (var ip in intentosEscaneo.Keys.ToList())
            {
                var puertosViejos = intentosEscaneo[ip].Where(kvp => kvp.Value < limite).Select(kvp => kvp.Key).ToList();
                foreach (var p in puertosViejos)
                    intentosEscaneo[ip].Remove(p);
                if (intentosEscaneo[ip].Count == 0)
                    intentosEscaneo.Remove(ip);
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
                if (intentosFuerzaBruta[ip].Count == 0)
                    intentosFuerzaBruta.Remove(ip);
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
                if (intentosEscaneoVertical[puerto].Count == 0)
                    intentosEscaneoVertical.Remove(puerto);
            }
        }

        // ================================================================

        private void DetectarPicoTrafico()
        {
            if (!alertasActivas) return;

            if (paquetesPorSegundo >= umbralTrafico)
            {
                string descripcion = $"Pico de trafico: {paquetesPorSegundo} pa q/s (umbral: {umbralTrafico})";
                alertaHelper.GuardarAlerta(
                    tipo: "PICO_TRAFICO",
                    descripcion: descripcion,
                    severidad: "Media",
                    ipInvolucrada: null
                    );

                lblUltimaAlerta.Text = $"⚠️ Alerta: {descripcion}";
                lblUltimaAlerta.ForeColor = Color.Red;
            }
        }

        private void CargarConfiguracion()
        {
            umbralEscaneo = 10;
            umbralTrafico = 500;
            alertasActivas = true;
            // Nuevos umbrales
            umbralFuerzaBruta = 10;
            ventanaFuerzaBrutaSegundos = 60;
            umbralICMPFlood = 100;
            alertasICMPFlood = true;
            ventanaEscaneoSegundos = 60;
            umbralEscaneoVertical = 10;
            ventanaEscaneoVerticalSegundos = 60;

            txtUmbralEscaneo.Text = umbralEscaneo.ToString();
            txtUmbralTrafico.Text = umbralTrafico.ToString();
            chkAlertasActivas.Checked = alertasActivas;
            // Asegurar que los nuevos controles existan en el diseñador
            if (txtUmbralFuerzaBruta != null) txtUmbralFuerzaBruta.Text = umbralFuerzaBruta.ToString();
            if (txtVentanaFuerzaBruta != null) txtVentanaFuerzaBruta.Text = ventanaFuerzaBrutaSegundos.ToString();
            if (txtUmbralICMPFlood != null) txtUmbralICMPFlood.Text = umbralICMPFlood.ToString();
            if (chkICMPFloodActivo != null) chkICMPFloodActivo.Checked = alertasICMPFlood;
            if (txtUmbralEscaneoVertical != null) txtUmbralEscaneoVertical.Text = umbralEscaneoVertical.ToString();
            if (txtVentanaEscaneoVertical != null) txtVentanaEscaneoVertical.Text = ventanaEscaneoVerticalSegundos.ToString();
        }

        private void ActualizarUISeguro()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ActualizarUISeguro));
                return;
            }

            // Actualizar contador
            lblEstadisticas.Text = $"Capturando... Paquetes: {contadorPaquetes}";

            // Aplicar filtros y actualizar vista
            AplicarFiltros(); // Este método ya maneja la limpieza del grid correctamente
        }

        private void btnAplicarFiltro_Click(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            cmbProtocolo.SelectedIndex = 0;
            txtIPOrigen.Text = "";
            txtIPDestino.Text = "";
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            //Obtener valores de filtro
            string filtroProtocolo = cmbProtocolo.SelectedItem?.ToString() ?? "Todos";
            string filtroIPOrigen = txtIPOrigen.Text.Trim().ToLower();
            string filtroIPDestino = txtIPDestino.Text.Trim().ToLower();

            //Filtrar Paquetes
            var paquetesFiltrados = todosLosPaquetes.AsEnumerable();

            if (filtroProtocolo != "Todos")
            {
                paquetesFiltrados = paquetesFiltrados.Where(p => p.Protocolo != null && p.Protocolo.Equals(filtroProtocolo, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filtroIPOrigen))
            {
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPOrigen != null && p.IPOrigen.ToLower().Contains(filtroIPOrigen));
            }
            if (!string.IsNullOrEmpty(filtroIPDestino))
            {
                paquetesFiltrados = paquetesFiltrados.Where(p => p.IPDestino != null && p.IPDestino.ToLower().Contains(filtroIPDestino));
            }

            //Mostrar resultados
            MostrarPaquetesEnGrid(paquetesFiltrados.ToList());
            ActualizarEstadisticasFiltro();
        }
        private void MostrarPaquetesEnGrid(List<PaqueteInfo> paquetes)
        {
            dgvPaquetes.Rows.Clear();

            foreach (var p in paquetes.Take(100))
            {
                dgvPaquetes.Rows.Add(
                    p.Hora, p.IPOrigen ?? "N/A",
                    p.IPDestino ?? "N/A",
                    p.Protocolo ?? "N/A",
                    p.PuertoOrigen > 0 ? p.PuertoOrigen.ToString() : "-",
                    p.PuertoDestino > 0 ? p.PuertoDestino.ToString() : "-",
                    p.Tamaño,
                    p.InformacionAdicional ?? "");
            }
        }
        private void ActualizarEstadisticasFiltro()
        {
            //Contar protocolos
            int tcp = todosLosPaquetes.Count(p => p.Protocolo == "TCP");
            int udp = todosLosPaquetes.Count(p => p.Protocolo == "UDP");
            int icmp = todosLosPaquetes.Count(p => p.Protocolo == "ICMP");

            lblEstadisticasFiltro.Text = $"TCP: {tcp} | UDP: {udp} | ICMP: {icmp} | Total: {todosLosPaquetes.Count}";
            //Cambiar Color si hay mucho Trafico
            if (tcp > 500)
            {
                lblEstadisticasFiltro.ForeColor = Color.Red;
            }
            else
            {
                lblEstadisticasFiltro.ForeColor = Color.Black;
            }
        }
        private void DgvPaquetes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //Resaltar filas segun el protocolo o puertos conocidos
            if (e.RowIndex >= 0 && dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value != null)
            {
                string protocolo = dgvPaquetes.Rows[e.RowIndex].Cells["Protocolo"].Value.ToString();
                string puertoDestino = dgvPaquetes.Rows[e.RowIndex].Cells["PuertoDestino"].Value?.ToString() ?? "0";

                //colorear segun el tipo de trafico
                if (protocolo == "TCP" && puertoDestino == "80")
                {
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightBlue; // HTTP
                }
                else if (protocolo == "TCP" && puertoDestino == "443") // CORREGIDO: era 433
                {
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen; // HTTPS
                }
                else if (protocolo == "TCP" && (puertoDestino == "22" || puertoDestino == "23"))
                {
                    dgvPaquetes.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink; // SSH/Telnet (sospechoso)
                }
            }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        public class AlertaHelper
        {
            private string connectionString = "Server=LocalHost;Database=monitorizacion_red;Uid=root;Pwd=;";
            public void GuardarAlerta(string tipo, string descripcion, string severidad, string ipInvolucrada = null)
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"INSERT INTO alertas (tipo, descripcion, severidad, ip_involucrada, timestamp) Values(@tipo, @descripcion, @severidad, @ip, @timestamp)";
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

        private void btnGuardarConfig_Click(object sender, EventArgs e)
        {
            try
            {
                umbralEscaneo = int.Parse(txtUmbralEscaneo.Text);
                umbralTrafico = int.Parse(txtUmbralTrafico.Text);
                alertasActivas = chkAlertasActivas.Checked;

                // Nuevos valores de configuración (si los controles existen)
                if (txtUmbralFuerzaBruta != null) umbralFuerzaBruta = int.Parse(txtUmbralFuerzaBruta.Text);
                if (txtVentanaFuerzaBruta != null) ventanaFuerzaBrutaSegundos = int.Parse(txtVentanaFuerzaBruta.Text);
                if (txtUmbralICMPFlood != null) umbralICMPFlood = int.Parse(txtUmbralICMPFlood.Text);
                if (chkICMPFloodActivo != null) alertasICMPFlood = chkICMPFloodActivo.Checked;
                if (txtUmbralEscaneoVertical != null) umbralEscaneoVertical = int.Parse(txtUmbralEscaneoVertical.Text);
                if (txtVentanaEscaneoVertical != null) ventanaEscaneoVerticalSegundos = int.Parse(txtVentanaEscaneoVertical.Text);

                MessageBox.Show("Configuracion guardada correctamente", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ingresa valores numericos Validos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerAlertas_Click(object sender, EventArgs e)
        {
            HistorialAlertas frm = new HistorialAlertas();
            frm.ShowDialog(); // ShowDialog para que sea modal
        }

        private void dgvPaquetes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtIPOrigen_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelUmbralTrafico_Click(object sender, EventArgs e)
        {

        }

        private void txtUmbralTrafico_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmbProtocolo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}