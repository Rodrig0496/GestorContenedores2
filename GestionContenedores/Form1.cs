
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

         
            CargarDatos();
        }


        private void MiVistaMapa_EliminarSolicitado(object sender, int idContenedor)
        {
            EliminarContenedor(idContenedor);
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
