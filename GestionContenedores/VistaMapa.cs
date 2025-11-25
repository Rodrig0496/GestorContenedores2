using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionContenedores
{
    public partial class VistaMapa : UserControl
    {
        public event EventHandler<int> EditarSolicitado;
        public event EventHandler<int> EliminarSolicitado;
        // El 'PointLatLng' serán las coordenadas del clic
        public event EventHandler<PointLatLng> NuevoContenedorSolicitado;
        // Evento simple para pedir login
        public event EventHandler LoginSolicitado;

        private GMapOverlay marcadoresOverlay = new GMapOverlay("marcadores");
        private ContextMenuStrip menuContextual;
        private int _nivelPermisoUsuario;
        private int _idContenedorSeleccionadoTemporal;
        public VistaMapa()
        {
            InitializeComponent();
            ConfigurarMapa();
            ConfigurarMenuContextual();
            // Suscribir el evento del mouse interno
            this.gMapControl1.MouseClick += gMapControl1_MouseClick;
        }
        private GMapOverlay rutasOverlay = new GMapOverlay("rutas");
        public void EstablecerPermisos(int nivelPermiso)
        {
            _nivelPermisoUsuario = nivelPermiso;
        }

        private void ConfigurarMapa()
        {
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.ShowCenter = false;
            gMapControl1.Position = new PointLatLng(-18.0146, -70.2536); // Tacna
            gMapControl1.MinZoom = 5;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 13;
            gMapControl1.Overlays.Add(marcadoresOverlay);
            gMapControl1.Overlays.Add(rutasOverlay);
        }
        public void DibujarRuta(List<PointLatLng> puntosRuta)
        {
            rutasOverlay.Routes.Clear(); // Limpiar rutas anteriores

            if (puntosRuta.Count < 2) return; // Necesitamos al menos 2 puntos para una línea

            GMapRoute ruta = new GMapRoute(puntosRuta, "RutaRecoleccion");
            ruta.Stroke = new Pen(Color.Blue, 3); // Línea azul de grosor 3

            rutasOverlay.Routes.Add(ruta);
            gMapControl1.ZoomAndCenterRoute(ruta); // Enfocar la cámara en la ruta
        }
        public void LimpiarRuta()
        {
            rutasOverlay.Routes.Clear();
        }
        private void ConfigurarMenuContextual()
        {
            menuContextual = new ContextMenuStrip();
            
            var itemEliminar = menuContextual.Items.Add("Eliminar Contenedor");
            itemEliminar.ForeColor = Color.Red;

            // Al hacer clic en el menú, DISPARAMOS eventos hacia Form1
            
            itemEliminar.Click += (s, e) => EliminarSolicitado?.Invoke(this, _idContenedorSeleccionadoTemporal);
        }

        // Método público principal que Form1 usará para darle datos al mapa
        public void CargarMarcadores(List<Contenedores> listaContenedores)
        {
            marcadoresOverlay.Markers.Clear();

            if (listaContenedores == null) return;

            foreach (var item in listaContenedores)
            {
                if (item.Latitud != 0 && item.Longitud != 0)
                {
                    PointLatLng punto = new PointLatLng((double)item.Latitud, (double)item.Longitud);

                    // --- LÓGICA DE COLORES ACTUALIZADA ---
                    GMarkerGoogleType tipoPin;
                    string estadoNormalizado = item.Estado?.Trim().ToUpper(); // Convertir a mayúsculas para comparar

                    switch (estadoNormalizado)
                    {
                        case "LLENO":
                            tipoPin = GMarkerGoogleType.red; // Rojo
                            break;
                        case "MITAD":
                            tipoPin = GMarkerGoogleType.yellow; // Amarillo
                            break;
                        case "UTIL":   // Casos para vacio
                        case "VACIO":
                            tipoPin = GMarkerGoogleType.green; // Verde
                            break;
                        default:
                            tipoPin = GMarkerGoogleType.gray_small; // Gris para Error o desconocido
                            break;
                    }
                    // -------------------------------------

                    GMarkerGoogle marcador = new GMarkerGoogle(punto, tipoPin);
                    marcador.ToolTipText = $"{item.Nombre}\nEstado: {item.Estado}\n{item.Direccion}";
                    marcador.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    marcador.Tag = item.Id;

                    marcadoresOverlay.Markers.Add(marcador);
                }
            }
            gMapControl1.Refresh();

            // Opcional: Centrar si hay datos
            if (listaContenedores.Count > 0)
            {
                // gMapControl1.Position = new PointLatLng(listaContenedores[0].Latitud, listaContenedores[0].Longitud);
            }
        }
        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            // 1. BUSCAR SI CLICKEO UN PIN
            GMapMarker markerClickeado = null;
            foreach (var m in marcadoresOverlay.Markers)
            {
                if (m.IsMouseOver)
                {
                    markerClickeado = m;
                    break;
                }
            }
            if (markerClickeado != null)
            {
                // ... (Tu lógica de Editar: menú contextual)
                if (markerClickeado.Tag != null && markerClickeado.Tag is int id)
                {
                    _idContenedorSeleccionadoTemporal = id;
                    menuContextual.Show(gMapControl1, e.Location);
                }
            }
            else
            {
                // ... (Tu lógica de Nuevo Contenedor)
                DialogResult respuesta = MessageBox.Show("¿Quiere ingresar un nuevo contenedor aquí?",
                    "Nuevo Contenedor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    PointLatLng puntoClic = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    // AQUÍ DISPARAMOS EL EVENTO HACIA EL DASHBOARD
                    NuevoContenedorSolicitado?.Invoke(this, puntoClic);
                }
            }

        }
        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
