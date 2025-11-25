
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
        public event EventHandler EstadoCambiado;
        public event EventHandler ContenedorAgregado;
        public CambioEstado()
        {
            InitializeComponent();
            ConfigurarFormularioAgregar();
        }

        public void PrellenarDatos(double latitud, double longitud, string direccion)
        {
            txtLatitud.Text = latitud.ToString();
            txtLongitud.Text = longitud.ToString();

            // Asignamos la dirección encontrada
            txtDireccion.Text = direccion;

            cmbEstado.SelectedIndex = 0;
        }

        private void ConfigurarFormularioAgregar()
        {
            
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Util");
            cmbEstado.Items.Add("Lleno");
            cmbEstado.SelectedIndex = 0;

            txtId.ReadOnly = true;          // El ID es automático
            txtLatitud.ReadOnly = true;     // Viene del mapa
            txtLongitud.ReadOnly = true;
            txtNombre.ReadOnly = false;

            try
            {
                int siguienteId = _service.ObtenerProximoId();
                txtId.Text = siguienteId.ToString();
            }
            catch (Exception)
            {
                txtId.Text = "1"; // Si falla, asumimos que es el primero
            }

            LimpiarCampos();
        }

        private void LimpiarCampos()
        {

            txtNombre.Text = "";
            txtDireccion.Text = "";
            txtLatitud.Text = "";
            txtLongitud.Text = "";
            cmbEstado.SelectedIndex = 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos (QUITAMOS LA VALIDACIÓN DEL ID porque es automático)
                if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                    string.IsNullOrWhiteSpace(txtDireccion.Text))
                    
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
                this.Close(); 
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
            this.Close();
        }
    }
}
