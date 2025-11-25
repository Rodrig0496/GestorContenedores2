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
using System.Net.Http; // Para pedir la ruta a internet
using System.Globalization; // Para leer coordenadas con punto decimal

namespace GestionContenedores
{
    public partial class VistaTrabajador : UserControl
    {
        private LinqService _service;
        private List<Contenedores> _listaCompleta;
        private Timer timerRefresco;
        private bool _modoRutaActivo = false;
        private List<int> _idsLlenosAnteriores = new List<int>(); // Ahora usamos una lista de números, no un string
        private PointLatLng? _ultimaUbicacionCamion = null;
        public VistaTrabajador()
        {
            InitializeComponent();
            _service = new LinqService();
            CargarMapaInicial();

            timerRefresco = new Timer();
            timerRefresco.Interval = 2000; // Cada 2 segundos
            timerRefresco.Tick += TimerRefresco_Tick;
            timerRefresco.Start();
        }
        private void RecalcularRutaDinamica(PointLatLng? puntoInicioPersonalizado = null)
        {
            // 1. Filtrar los que siguen LLENOS
            var contenedoresLlenos = _listaCompleta
                .Where(c => c.Estado != null && c.Estado.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Si ya no hay nada lleno, limpiamos todo
            if (contenedoresLlenos.Count == 0)
            {
                mapaTrabajador.LimpiarRuta();
                _modoRutaActivo = false;
                _idsLlenosAnteriores.Clear();
                MessageBox.Show("¡Ruta completada! Todos los contenedores están vacíos.", "Fin del Trabajo");
                return;
            }

            // 2. Definir el punto de partida
            PointLatLng ubicacionActual;

            if (puntoInicioPersonalizado.HasValue)
            {
                // Si le pasamos un punto (el contenedor que acabamos de vaciar), empezamos ahí.
                ubicacionActual = puntoInicioPersonalizado.Value;
            }
            else
            {
                // Si es la primera vez, empezamos en el primer contenedor lleno
                ubicacionActual = new PointLatLng(contenedoresLlenos[0].Latitud, contenedoresLlenos[0].Longitud);
            }

            // 3. Algoritmo del Vecino Más Cercano
            List<Contenedores> rutaOrdenada = new List<Contenedores>();
            var pendientes = new List<Contenedores>(contenedoresLlenos);

            // OJO: No agregamos 'ubicacionActual' a la lista de pendientes porque ya estamos ahí (o acabamos de vaciarlo)
            // Solo buscamos cuál es el siguiente más cercano.

            while (pendientes.Count > 0)
            {
                var masCercano = pendientes
                    .OrderBy(c => ObtenerDistancia(ubicacionActual, c.Latitud, c.Longitud))
                    .First();

                rutaOrdenada.Add(masCercano);
                ubicacionActual = new PointLatLng(masCercano.Latitud, masCercano.Longitud);
                pendientes.Remove(masCercano);
            }

            // 4. Dibujar la ruta (Incluyendo el tramo desde donde estamos hasta el primero)
            List<PointLatLng> puntosDibujo = new List<PointLatLng>();

            // Tramo A: Desde mi ubicación actual (o contenedor vaciado) hacia el primero de la lista nueva
            if (puntoInicioPersonalizado.HasValue)
            {
                PointLatLng destino = new PointLatLng(rutaOrdenada[0].Latitud, rutaOrdenada[0].Longitud);
                var tramoInicial = ObtenerRutaPorCalles(puntoInicioPersonalizado.Value, destino);
                puntosDibujo.AddRange(tramoInicial);
            }

            // Tramo B: Unir el resto de los contenedores llenos
            for (int i = 0; i < rutaOrdenada.Count - 1; i++)
            {
                PointLatLng inicio = new PointLatLng(rutaOrdenada[i].Latitud, rutaOrdenada[i].Longitud);
                PointLatLng fin = new PointLatLng(rutaOrdenada[i + 1].Latitud, rutaOrdenada[i + 1].Longitud);

                List<PointLatLng> tramo = ObtenerRutaPorCalles(inicio, fin);
                puntosDibujo.AddRange(tramo);
            }

            mapaTrabajador.DibujarRuta(puntosDibujo);
        }

        private void TimerRefresco_Tick(object sender, EventArgs e)
        {
            // 1. Refrescar datos
            _listaCompleta = _service.ObtenerContenedores();
            mapaTrabajador.CargarMarcadores(_listaCompleta);

            // 2. Lógica inteligente de ruta
            if (_modoRutaActivo)
            {
                // Obtenemos la lista de IDs que están llenos AHORA
                var idsLlenosActuales = _listaCompleta
                    .Where(c => c.Estado != null && c.Estado.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Id)
                    .ToList();

                // Comparamos: ¿Había más contenedores antes que ahora? (Significa que uno se vació)
                // Usamos 'Except' para encontrar cuál ID estaba antes y ya no está.
                var idVaciado = _idsLlenosAnteriores.Except(idsLlenosActuales).FirstOrDefault();

                if (idVaciado != 0) // ¡Encontramos uno vaciado!
                {
                    // Buscamos las coordenadas de ese contenedor que acabamos de vaciar
                    // Nota: Buscamos en _listaCompleta aunque ya no sea "Lleno", sigue existiendo en la lista general
                    var contenedorVaciado = _listaCompleta.FirstOrDefault(c => c.Id == idVaciado);

                    if (contenedorVaciado != null)
                    {
                        _ultimaUbicacionCamion = new PointLatLng(contenedorVaciado.Latitud, contenedorVaciado.Longitud);

                        // Recalculamos la ruta EMPEZANDO desde el que acabamos de vaciar
                        RecalcularRutaDinamica(_ultimaUbicacionCamion);
                    }

                    // Actualizamos nuestra "memoria" para el siguiente ciclo
                    _idsLlenosAnteriores = new List<int>(idsLlenosActuales);
                }
                else if (_idsLlenosAnteriores.Count != idsLlenosActuales.Count)
                {
                    // Caso raro: Se llenó uno nuevo mientras conducíamos (ruta cambia pero sin vaciar nada)
                    // O es la primera vez que corre.
                    RecalcularRutaDinamica(_ultimaUbicacionCamion);
                    _idsLlenosAnteriores = new List<int>(idsLlenosActuales);
                }
            }
        }
        private List<PointLatLng> ObtenerRutaPorCalles(PointLatLng inicio, PointLatLng fin)
        {
            List<PointLatLng> puntosCalle = new List<PointLatLng>();

            try
            {
                // Construimos la URL para la API gratuita de OSRM
                // Formato: /driving/lon1,lat1;lon2,lat2
                string url = $"http://router.project-osrm.org/route/v1/driving/" +
                             $"{inicio.Lng.ToString(CultureInfo.InvariantCulture)},{inicio.Lat.ToString(CultureInfo.InvariantCulture)};" +
                             $"{fin.Lng.ToString(CultureInfo.InvariantCulture)},{fin.Lat.ToString(CultureInfo.InvariantCulture)}" +
                             $"?overview=full&geometries=geojson";

                using (HttpClient client = new HttpClient())
                {
                    // Hacemos la petición a internet (esperamos el resultado síncronamente para simplificar)
                    var respuesta = client.GetStringAsync(url).Result;

                    // --- PARSEO MANUAL DEL JSON (Para no obligarte a instalar librerías extra) ---
                    // Buscamos donde empiezan las coordenadas en la respuesta
                    int indexCoords = respuesta.IndexOf("\"coordinates\":[[");
                    if (indexCoords != -1)
                    {
                        indexCoords += 15; // Saltamos el texto "coordinates":[
                        int indexFin = respuesta.IndexOf("]]", indexCoords);
                        string data = respuesta.Substring(indexCoords, indexFin - indexCoords + 1);

                        // Limpiamos corchetes y separamos
                        data = data.Replace("[", "").Replace("]", "");
                        string[] valores = data.Split(',');

                        // OSRM devuelve: lon, lat, lon, lat...
                        for (int i = 0; i < valores.Length; i += 2)
                        {
                            if (i + 1 < valores.Length)
                            {
                                double lon = double.Parse(valores[i], CultureInfo.InvariantCulture);
                                double lat = double.Parse(valores[i + 1], CultureInfo.InvariantCulture);
                                puntosCalle.Add(new PointLatLng(lat, lon));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Si falla internet o el servidor, devolvemos al menos una línea recta para que no se rompa
                puntosCalle.Add(inicio);
                puntosCalle.Add(fin);
            }

            return puntosCalle;
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
        private void CargarMapaInicial()
        {
            _listaCompleta = _service.ObtenerContenedores();
            // Mostramos todos los pines para que el trabajador vea el panorama
            mapaTrabajador.CargarMarcadores(_listaCompleta);
            // Establecemos permisos de solo lectura (o nivel 1) para que no editen con clic derecho
            mapaTrabajador.EstablecerPermisos(1);
        }

        private void btnGenerarRuta_Click(object sender, EventArgs e)
        {
            // Inicializamos la lista de anteriores con lo que hay actualmente
            _idsLlenosAnteriores = _listaCompleta
                .Where(c => c.Estado != null && c.Estado.Trim().Equals("Lleno", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToList();

            if (_idsLlenosAnteriores.Count == 0)
            {
                MessageBox.Show("No hay contenedores llenos.", "Ruta Vacía");
                return;
            }

            _modoRutaActivo = true;
            _ultimaUbicacionCamion = null; // Reseteamos ubicación

            // Calculamos la primera ruta estándar
            RecalcularRutaDinamica();

            MessageBox.Show("Ruta dinámica activada.", "Iniciando");
        }
        private double ObtenerDistancia(PointLatLng p1, double lat2, double lon2)
        {
            return Math.Sqrt(Math.Pow(p1.Lat - lat2, 2) + Math.Pow(p1.Lng - lon2, 2));
        }
    }
}
