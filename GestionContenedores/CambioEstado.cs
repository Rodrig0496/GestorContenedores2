
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

namespace GestionContenedores
{
    public partial class CambioEstado : Form
    {
        LinqService _service = new LinqService();
        private List<Contenedores> contenedores;
        private Contenedores contenedorSeleccionado;
        public event EventHandler EstadoCambiado;
        public event EventHandler ContenedorAgregado;
        public CambioEstado(List<Contenedores> contenedoresList)
        {
            InitializeComponent();
            contenedores = contenedoresList;
            CargarComboBox();
            ConfigurarFormularioAgregar();
        }

        public void PrellenarCoordenadas(double latitud, double longitud)
        {
            // Asignamos los valores a los TextBox
            txtLatitud.Text = latitud.ToString();
            txtLongitud.Text = longitud.ToString();

            // Opcional: Seleccionar automáticamente el estado "Util" para un nuevo contenedor
            cmbEstado.SelectedIndex = 0;
        }

        public void SeleccionarContenedorPorId(int id)
        {
            // Recorremos los items del ComboBox para encontrar el que coincide con el ID
            foreach (var item in cmbContenedores.Items)
            {
                // Como el item es un string tipo "4 - Parque...", verificamos si empieza con el ID
                if (item.ToString().StartsWith(id.ToString() + " -"))
                {
                    cmbContenedores.SelectedItem = item;
                    break; // Ya lo encontramos, dejamos de buscar
                }
            }
        }
        private void ConfigurarFormularioAgregar()
        {
            
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Util");
            cmbEstado.Items.Add("Lleno");
            cmbEstado.SelectedIndex = 0; 

            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtId.Text = "Auto"; // El ID lo genera SQL, así que ponemos esto visualmente
            txtId.Enabled = false; // Desactivamos el campo para que no escriban

            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtLatitud.Text = "";
            txtLongitud.Text = "";
            cmbEstado.SelectedIndex = 0;
        }

        private void CargarComboBox()
        {
            cmbContenedores.Items.Clear();

            foreach (var contenedor in contenedores)
            {
                cmbContenedores.Items.Add($"{contenedor.Id} - {contenedor.Nombre}");
            }

            if (cmbContenedores.Items.Count > 0)
            {
                cmbContenedores.SelectedIndex = 0;
            }
        }

        private void cmbContenedores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbContenedores.SelectedIndex >= 0)
            {
                // Extraemos el ID del texto del combo (ej: "4 - Parque")
                int contenedorId = int.Parse(cmbContenedores.SelectedItem.ToString().Split('-')[0].Trim());

                // Buscamos en la lista local que recibimos
                contenedorSeleccionado = contenedores.FirstOrDefault(c => c.Id == contenedorId);

                if (contenedorSeleccionado != null)
                {
                    MostrarDatosContenedor();
                }
            }
        }

        private void MostrarDatosContenedor()
        {

            if (contenedorSeleccionado.Estado != null &&
                contenedorSeleccionado.Estado.Trim().Equals("Util", StringComparison.OrdinalIgnoreCase))
            {
                rbtnUtilizable.Checked = true;
            }
            else
            {
                rbtnLleno.Checked = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (contenedorSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un contenedor", "Advertencia",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string nuevoEstado = rbtnUtilizable.Checked ? "Util" : "Lleno";

                // CAMBIO: Llamamos a la BD para actualizar
                _service.ActualizarEstado(contenedorSeleccionado.Id, nuevoEstado);

                // Disparar evento para que Form1 se entere y recargue
                EstadoCambiado?.Invoke(this, EventArgs.Empty);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar en BD: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos (QUITAMOS LA VALIDACIÓN DEL ID porque es automático)
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                    string.IsNullOrWhiteSpace(txtLatitud.Text) ||
                    string.IsNullOrWhiteSpace(txtLongitud.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos", "Advertencia",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Recoger datos
                string nombre = txtNombre.Text;
                string direccion = txtDireccion.Text;
                double lat = double.Parse(txtLatitud.Text);
                double lon = double.Parse(txtLongitud.Text);
                string estado = cmbEstado.SelectedItem.ToString();

                // CAMBIO: Guardamos directo en BD usando el servicio
                _service.GuardarContenedor(nombre, direccion, lat, lon, estado);

                // Disparar evento para recargar mapa
                ContenedorAgregado?.Invoke(this, EventArgs.Empty);

                LimpiarCampos();

                MessageBox.Show("Contenedor guardado en Base de Datos", "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Opcional: Cerrar después de agregar o recargar combo
                // this.Close(); 
            }
            catch (FormatException)
            {
                MessageBox.Show("Latitud y Longitud deben ser números válidos", "Error de formato",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en BD: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }
    }
}
