using GestionContenedores.Services;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GestionContenedores
{
    public partial class VistaDashboard : UserControl
    {
        private LinqService _service;
        private List<Contenedores> _listaContenedores;
        private int _nivelPermisoUsuario;
        private string _nombreUsuario;
        private Func<bool> _verificadorPermisosFn;
        private Timer timerRefresco;
        public VistaDashboard(int nivelPermiso, string nombreUsuario, Func<bool> verificadorPermisosFn)
        {
            InitializeComponent();
            _service = new LinqService();
            _nivelPermisoUsuario = nivelPermiso;
            _nombreUsuario = nombreUsuario;
            _verificadorPermisosFn = verificadorPermisosFn;
            ConfigurarControlesInicial();
            CargarDatosDashboard();
            timerRefresco = new Timer();
            timerRefresco.Interval = 2000;
            timerRefresco.Tick += TimerRefresco_Tick;
            timerRefresco.Start();
        }
        private void TimerRefresco_Tick(object sender, EventArgs e)
        {
            // Llamamos a tu método existente que ya carga el Mapa, Gráfico y Tabla
            CargarDatosDashboard();
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (timerRefresco != null)
            {
                timerRefresco.Stop();
                timerRefresco.Dispose();
            }
            base.OnHandleDestroyed(e);
        }
        private void ConfigurarControlesInicial()
        {
            // 1. Configurar Mapa
            // Pasamos los permisos al control de mapa incrustado
            miVistaMapaDashboard.EstablecerPermisos(_nivelPermisoUsuario);
            // Suscribimos los eventos del mapa para que sigan funcionando (editar/eliminar/nuevo)
            miVistaMapaDashboard.EliminarSolicitado += MiVistaMapaDashboard_EliminarSolicitado;
            miVistaMapaDashboard.NuevoContenedorSolicitado += MiVistaMapaDashboard_NuevoContenedorSolicitado;

            // 2. Configurar Gráfico
            ConfigurarGrafico();

            // 3. Configurar Tabla
            dgvDashboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDashboard.ReadOnly = true;
            dgvDashboard.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        public void CargarDatosDashboard()
        {
            // Esta es la función central que actualiza TODO
            _listaContenedores = _service.ObtenerContenedores();

            // A. Actualizar Mapa
            miVistaMapaDashboard.CargarMarcadores(_listaContenedores);

            // B. Actualizar Tabla
            dgvDashboard.DataSource = null;
            dgvDashboard.DataSource = _listaContenedores;
            OcultarColumnasInnecesarias();

            // C. Actualizar Gráfico
            ActualizarDatosGrafico();
        }

        #region Lógica del Gráfico
        private void ConfigurarGrafico()
        {
            chartEstados.Series.Clear();
            chartEstados.Titles.Clear();

            chartEstados.Titles.Add("CONTENEDORES POR ESTADO");
            chartEstados.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);

            // Creamos la serie principal
            Series serie = new Series("Estados");
            serie.ChartType = SeriesChartType.Column; // Gráfico de barras verticales
            serie.IsValueShownAsLabel = true; // Mostrar el número sobre la barra
            serie.IsVisibleInLegend = false;
            chartEstados.Series.Add(serie);
            chartEstados.ChartAreas[0].AxisX.Interval = 1; // Asegurar que se vean todas las etiquetas X
        }

        private void ActualizarDatosGrafico()
        {
            if (_listaContenedores == null) return;

            // Usamos LINQ para agrupar y contar por estado
            var datosGrafico = _listaContenedores
                .GroupBy(c => c.Estado)
                .Select(grupo => new { Estado = grupo.Key, Cantidad = grupo.Count() })
                .ToList();

            chartEstados.Series["Estados"].Points.Clear();

            foreach (var dato in datosGrafico)
            {
                // Validar que el estado no sea nulo o vacío para el gráfico
                string estadoLabel = string.IsNullOrEmpty(dato.Estado) ? "Sin Estado" : dato.Estado;

                int indicePunto = chartEstados.Series["Estados"].Points.AddXY(estadoLabel, dato.Cantidad);
                DataPoint punto = chartEstados.Series["Estados"].Points[indicePunto];

                // Colorear según el estado (opcional, igual que en el mapa)
                if (estadoLabel.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                {
                    punto.Color = Color.FromArgb(220, 53, 69); // Rojo
                }
                else if (estadoLabel.Trim().Equals("Util", StringComparison.OrdinalIgnoreCase) ||
                         estadoLabel.Trim().Equals("Vacio", StringComparison.OrdinalIgnoreCase))
                {
                    punto.Color = Color.FromArgb(40, 167, 69); // Verde
                }
                // AGREGAMOS ESTO PARA EL AMARILLO
                else if (estadoLabel.Trim().Equals("Mitad", StringComparison.OrdinalIgnoreCase))
                {
                    punto.Color = Color.FromArgb(255, 193, 7); // Amarillo (Tono "Warning" estándar)
                }
                else
                {
                    punto.Color = Color.SteelBlue; // Por defecto
                }
            }
            chartEstados.Update();
        }
        #endregion

        #region Métodos Auxiliares y Eventos del Mapa

        private void OcultarColumnasInnecesarias()
        {
            if (dgvDashboard.Columns["Latitud"] != null) dgvDashboard.Columns["Latitud"].Visible = false;
            if (dgvDashboard.Columns["Longitud"] != null) dgvDashboard.Columns["Longitud"].Visible = false;
            // Oculta otras que no quieras ver en el dashboard
        }

        // --- Manejadores de eventos que vienen del mapa ---
        // Estos métodos abren los formularios de edición, igual que hacía Form1 antes.

        private void MiVistaMapaDashboard_NuevoContenedorSolicitado(object sender, PointLatLng punto)
        {
            // 1. VERIFICACIÓN DE SEGURIDAD
            // Llamamos a la función del MainForm. Si devuelve false, salimos.
            // Esto abrirá el login si es necesario y esperará el resultado.
            if (!_verificadorPermisosFn()) return;

            string direccionEncontrada = "";

            try
            {
                // Usamos una lista de Placemarks para recibir el resultado
                List<GMap.NET.Placemark> placemarks = null;

                // Intentamos obtener la dirección usando OpenStreetMap (Es gratis y no pide API Key)
                // Nota: Aunque uses Google en el mapa visual, puedes usar OSM para obtener la dirección
                var estado = GMap.NET.MapProviders.GMapProviders.OpenStreetMap.GetPlacemarks(punto, out placemarks);

                if (estado == GMap.NET.GeoCoderStatusCode.G_GEO_SUCCESS && placemarks != null && placemarks.Count > 0)
                {
                    direccionEncontrada = placemarks[0].Address;
                }
            }
            catch (Exception)
            {
                // Si falla (por internet o lo que sea), simplemente dejamos la dirección vacía
                direccionEncontrada = "";
            }

            // 2. Si pasa la verificación, procedemos con la acción
            var formNuevo = new CambioEstado();
            formNuevo.PrellenarDatos(punto.Lat, punto.Lng, direccionEncontrada);
            formNuevo.ContenedorAgregado += (s, args) => CargarDatosDashboard();
            formNuevo.ShowDialog();
        }

        private void MiVistaMapaDashboard_EliminarSolicitado(object sender, int idContenedor)
        {
            // 1. VERIFICACIÓN DE SEGURIDAD
            if (!_verificadorPermisosFn()) return;

            // 2. Proceder
            DialogResult confirmacion = MessageBox.Show(
               $"¿Eliminar contenedor ID: {idContenedor}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    _service.EliminarContenedor(idContenedor);
                    CargarDatosDashboard();
                    MessageBox.Show("Eliminado correctamente.");
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            }
        }
        #endregion
    }
}
