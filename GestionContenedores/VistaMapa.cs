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
        }

        private void ConfigurarMenuContextual()
        {
            menuContextual = new ContextMenuStrip();
            var itemEditar = menuContextual.Items.Add("Editar Estado");
            var itemEliminar = menuContextual.Items.Add("Eliminar Contenedor");
            itemEliminar.ForeColor = Color.Red;

            // Al hacer clic en el menú, DISPARAMOS eventos hacia Form1
            itemEditar.Click += (s, e) => EditarSolicitado?.Invoke(this, _idContenedorSeleccionadoTemporal);
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

                    GMarkerGoogleType tipoPin = GMarkerGoogleType.green;
                    if (item.Estado != null && item.Estado.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                    {
                        tipoPin = GMarkerGoogleType.red;
                    }

                    GMarkerGoogle marcador = new GMarkerGoogle(punto, tipoPin);
                    marcador.ToolTipText = $"{item.Nombre}\nEstado: {item.Estado}\n{item.Direccion}";
                    marcador.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                    // GUARDAMOS EL ID EN EL TAG (Crucial)
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

            // 2. VERIFICAR PERMISOS (Lógica de UI solamente)
            // Si es invitado (1), preguntamos si quiere loguearse.
            if (_nivelPermisoUsuario == 1)
            {
                string accion = markerClickeado != null ? "gestionar contenedores" : "agregar contenedores";
                DialogResult r = MessageBox.Show($"Para {accion} necesita ser Admin.\n¿Iniciar sesión?",
                    "Permisos", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (r == DialogResult.Yes)
                {
                    // Avisamos al padre que intente iniciar sesión
                    LoginSolicitado?.Invoke(this, EventArgs.Empty);
                    // El padre se encargará de mostrar el login y actualizar los permisos si tiene éxito.
                    // Por ahora, salimos hasta que el usuario vuelva a intentar la acción ya logueado.
                    return;
                }
                else
                {
                    return; // No quiso loguearse, no hacemos nada.
                }
            }

            // Si llegamos aquí, es Admin (0) o ya se manejó el login.

            // 3. ACCIONES SEGÚN DONDE CLICKEO
            if (markerClickeado != null)
            {
                // --- CASO A: CLICK EN UN PIN (MOSTRAR MENU) ---
                if (markerClickeado.Tag != null && markerClickeado.Tag is int id)
                {
                    _idContenedorSeleccionadoTemporal = id;
                    menuContextual.Show(gMapControl1, e.Location);
                }
            }
            else
            {
                // --- CASO B: CLICK EN EL SUELO (CREAR NUEVO) ---
                DialogResult respuesta = MessageBox.Show("¿Quiere ingresar un nuevo contenedor aquí?",
                    "Nuevo Contenedor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    PointLatLng puntoClic = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                    // Disparamos el evento con las coordenadas
                    NuevoContenedorSolicitado?.Invoke(this, puntoClic);
                }
            }
        }
        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
