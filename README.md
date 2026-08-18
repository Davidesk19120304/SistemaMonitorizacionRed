 Sistema de análisis de tráfico de rojo y detección de anomalías

Una aplicación de escritura para monitoreo de tráfico de rojo en tiempo real y detección de anomalías, enfermedad para pequeñas empresas, instituciones educativas y cibercafés con recursos limitados.

## 📌 Características

### Captura de paquetes en tiempo real
- Captura paquetes de rojo usando SharpPcap y Npcap
- Decodificación de los protocolos TCP, UDP, ICMP e IGMP
- Muestra paquetes en una tabla detallada con marcas de tiempo, IP, puertos, indicadores y tamaños de paquetes

### Detección de anomalías
- **Detección adaptativa de picos de tráfico**: Utiliza análisis estadístico (media + sigma × desviación estándar) para detectar patrones de tráfico inusuales
- **Detección de inundaciones del ICMP**: Identificables posibles ataques DoS a través de tráfico ICMP excesivo
- **Detención de escándalo de puertos (horizontal)**: Detecta cuando una sola IP entra en contacto con varios puertos diferentes en poco tiempo
- **Detención de escándalo de puertos (vertical)**: Detecta cuando varias IP apuntan al mismo puerto simultáneamente
- **Detección de fuerza bruta**: Monitorea puertos sensibles (SSH, FTP, RDP, MySQL) para intenciones de conexión repetitivos

### Visualización en tiempo real
- Gráfico de gráfico en vivo con 4 líneas de colores (TCP, UDP, ICMP, IGMP)
- Medidores circulares para:
  - Velocidad del enlace (Mbps/Gbps)
  - Latencia de rojo (ping a 8.8.8.8)
  - Detección de banda Wi-Fi (2,4 GHz, 5 GHz, 6 GHz)

### Gestión de alertas
- Notificaciones emergentes instantáneas cuando se detectan anomalías
- Historial de alertas alcanzadas en PostgreSQL
- Clasificación de tumba (Baja, Media, Alta, Crítica)
- Visualización de alertas codificadas por colores

### Capacidades de exportación
- Exportar paquetes y alertas a **PDF** (formato profesional con colores corporativos)
- Exportar a **Excel** (compatible con WPS Office)
- Exportar a **CSV** (para análisis externo)
- Selección de rango de fiestas para exportaciones filtradas

### Características de seguridad
- Autenticación de dos factores (nombre de usuario/contraseña + PIN de 4 dígitos)
- Hashing de contraseñas SHA256
- Gestión de usuarios desde dentro de la aplicación
- Funcionalidad de cambio de PIN

### Interfaz de usuario
- Modo claro y oscuro
- Filtrado automático de IP con autocompletado
- Diseño responsivo que se adapta al tamaño de la ventana
- Esquema de colores profesional

## 🛠️ Tecnologías utilizadas

| Tecnología | Propósito |
|------------|---------|
| C# (.Marco NET 4.7.2) | Lenguaje de programación principal |
| Tapa afilada | Biblioteca de captura de paquetes |
| PaqueteDotNet | Decodificación de paquetes |
| Npgsql | Conector PostgreSQL |
| Gráficos en vivo | Gráficos en tiempo real |
| Pdf nítido | Generación de informes en formato PDF |
| Barra de progreso circular | Medidores visuales |

## 📋 Requisitos

### Hardware
- Intel Core i3 o equivalente
- 4 GB de RAM mínimo
- 500 MB de espacio libre en disco
- Network adapter compatible with Npcap

### Software
- Windows 10/11 (64-bit)
- Npcap installed (WinPcap compatible mode)
- PostgreSQL 12+ (or XAMPP with MySQL for development)

## 🚀 Installation

### 1. Install Npcap
Download and install from [npcap.com](https://npcap.com) with "WinPcap API-compatible Mode" enabled.

### 2. Install PostgreSQL
Download from [postgresql.org](https://www.postgresql.org/download/windows/) and create a database named `monitorizacion_red`.

 (compatible con WPS Office) 3. Configure the connection
Open `App.config` and set your connection string:

```xml
<connectionStrings>
  <agregar nombre="Conexión predeterminada" 
       connectionString="Host=localhost;Base de datos=monitorización_red;Nombre de usuario=postgres;Contraseña=TU_CONTRASEÑA" />
</connectionStrings>
```

### 4. Ejecute la aplicación
Ejecutar `SistemaMonitorizaciónRed.exe`. En el primer lanzamiento, el sistema creará automáticamente las tablas necesarias.

## 👤 Credenciales predeterminadas

| Nombre de usuario | Contraseña |
|----------|----------|
| `admin` | `admin` |

**Nota:** Al iniciar sesión por primera vez, se le solicitará que cree un PIN de 4 dígitos para la autenticación de dos factores.

## 📊 Esquema de base de datos

### Tablas
- **paquetes**: Almacena paquetes de capturados rojos
- **alertas**: Almacena anomalías detectadas
- **latencias**: Almacena mediciones de latencia de la red
- **usuarios**: Almacena cuentas de usuario y datos de autenticación

## 🧪 Pruebas

El sistema incluye un script de prueba de carga (`Prueba de carga.ps1`) que simula el tráfico de rojo:

```powershell
.\LoadTest.ps1 -targetIP 127.0.0.1 -duraciónSegundos 30 -paquetasPorSegundo 500
```

## 📁 Estructura del proyecto

```
SistemaMonitorizaciónRojo/
├── FrmMain.cs # Interfaz de monitoreo principal
├── FrmLogin.cs # Iniciar sesión con autenticación PIN
├── HistorialAlertas.cs # Visor de histórico de alertas
├── FrmConfiguraciónAlertas.cs # Configuración de alerta
xml
```
├── App.config # Configuración de la base de datos
└── Recursos/ # Imágenes y recursos visuales
```

## 🤝 Contribuyendo

Este proyecto fue desarrollado como tesis para la licencia en Ingeniería de Sistemas del Instituto Universitario Politécnico “Santiago Mariño” (IUPSM).

## 📄 Licencia

Este proyecto tiene fines educativos. Todos los derechos reservados.

## 👤 Autor

**Guillermo David Adrianza Díaz**
- Graduado en Ingeniería de Sistemas
- Cabimas, Venezuela
