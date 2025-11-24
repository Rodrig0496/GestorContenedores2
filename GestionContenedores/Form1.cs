
using GestionContenedores.Services;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;


namespace GestionContenedores
{
    public partial class Form1 : Form
    {
        LinqService _service = new LinqService();
        private List<Contenedores> contenedores;
        private int nivelPermiso;
        private string usuarioActual;
       
        public Form1(int nivelPermiso, string usuario)
        {
            InitializeComponent();
            this.nivelPermiso = nivelPermiso;
            this.usuarioActual = usuario;

            AplicarPermisosEnInterfaz();
            SuscribirEventosDelMapa();
            CargarDatos();
        }

        private void SuscribirEventosDelMapa()
        {
            //// Le decimos al mapa quién es el usuario actual
            //miVistaMapa.EstablecerPermisos(this.nivelPermiso);

            //// Conectamos los eventos del mapa a métodos de este formulario
            miVistaMapa.EditarSolicitado += MiVistaMapa_EditarSolicitado;
            miVistaMapa.EliminarSolicitado += MiVistaMapa_EliminarSolicitado;
            miVistaMapa.NuevoContenedorSolicitado += MiVistaMapa_NuevoContenedorSolicitado;
            miVistaMapa.LoginSolicitado += MiVistaMapa_LoginSolicitado;
        }

        private void MiVistaMapa_EditarSolicitado(object sender, int idContenedor)
        {
            AbrirFormularioEdicion(idContenedor);
        }

        private void MiVistaMapa_EliminarSolicitado(object sender, int idContenedor)
        {
            EliminarContenedor(idContenedor);
        }

        private void MiVistaMapa_NuevoContenedorSolicitado(object sender, PointLatLng punto)
        {
            var formNuevo = new CambioEstado(this.contenedores);
            formNuevo.PrellenarCoordenadas(punto.Lat, punto.Lng);
            formNuevo.ContenedorAgregado += FormCambioEstado_ContenedorAgregado;
            formNuevo.ShowDialog();
        }

        private void MiVistaMapa_LoginSolicitado(object sender, EventArgs e)
        {
            // Manejamos el flujo de Login aquí, que es lógica de aplicación principal
            Login login = new Login();
            if (login.ShowDialog() == DialogResult.OK)
            {
                this.usuarioActual = login.UsuarioActual;
                this.nivelPermiso = login.NivelPermiso;
                // Importante: Actualizar la interfaz y avisar al mapa del nuevo permiso
                AplicarPermisosEnInterfaz();
            }
        }

        // --- MÉTODOS DE LÓGICA DE NEGOCIO Y UI ---

        private void EliminarContenedor(int idToDelete)
        {
            DialogResult confirmacion = MessageBox.Show(
                $"¿Estás seguro de que deseas ELIMINAR permanentemente el contenedor ID: {idToDelete}?",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    _service.EliminarContenedor(idToDelete);
                    CargarDatos(); // Recargar todo para reflejar cambios
                    MessageBox.Show("Contenedor eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        private void AbrirFormularioEdicion(int idToEdit)
        {
            var formEditar = new CambioEstado(this.contenedores);
            formEditar.SeleccionarContenedorPorId(idToEdit);
            formEditar.EstadoCambiado += FormCambioEstado_EstadoCambiado;
            formEditar.ShowDialog();
        }

        private void AplicarPermisosEnInterfaz()
        {
            // Actualizamos el mapa con el nuevo permiso
            miVistaMapa.EstablecerPermisos(this.nivelPermiso);

            if (this.nivelPermiso == 1) // Invitado
            {
                btnCambiarEstado.Enabled = false;
                this.Text = $"Gestor de Contenedores - Modo: Invitado ({usuarioActual})";
            }
            else // Admin (0)
            {
                btnCambiarEstado.Enabled = true;
                this.Text = $"Gestor de Contenedores - Modo: ADMINISTRADOR ({usuarioActual})";
            }
        }

        private void CargarDatos()
        {
            _service = new LinqService();
            contenedores = _service.ObtenerContenedores();
            ActualizarDataGridView();
            miVistaMapa.CargarMarcadores(contenedores);
            
        }

        private void ActualizarDataGridView()
        {
            dgvContenedores.DataSource = null;
            dgvContenedores.DataSource = contenedores;

            if (dgvContenedores.Columns.Contains("Id")) dgvContenedores.Columns["Id"].HeaderText = "ID";
            if (dgvContenedores.Columns.Contains("Nombre")) dgvContenedores.Columns["Nombre"].HeaderText = "Nombre";

            // Ocultar columnas técnicas si es necesario
            if (dgvContenedores.Columns.Contains("Latitud")) dgvContenedores.Columns["Latitud"].Visible = false;
            if (dgvContenedores.Columns.Contains("Longitud")) dgvContenedores.Columns["Longitud"].Visible = false;

            dgvContenedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            var formCambioEstado = new CambioEstado(contenedores);
            formCambioEstado.ContenedorAgregado += FormCambioEstado_ContenedorAgregado;
            formCambioEstado.ShowDialog();
        }

        private void FormCambioEstado_EstadoCambiado(object sender, EventArgs e)
        {
            CargarDatos();

            MessageBox.Show("Estado actualizado visualmente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FormCambioEstado_ContenedorAgregado(object sender, EventArgs e)
        {
            CargarDatos();

            MessageBox.Show("Contenedor visualizado en mapa", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chartBarras_Click(object sender, EventArgs e)
        {

        }
    }
}
