using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GestionContenedores.Services;

namespace GestionContenedores
{
    public partial class Login : Form
    {

        public int NivelPermiso { get; private set; } = -1; 
        public string UsuarioActual { get; private set; } = "";

        public Login()
        {
            InitializeComponent();
        }



        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContraseña.Text.Trim();

            if (usuario == "" || contrasena == "")
            {
                MessageBox.Show("Por favor ingresa usuario y contraseña.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Instanciamos nuestro servicio LINQ
                LinqService servicio = new LinqService();

                // 2. Validamos contra la Base de Datos
                int permiso = servicio.ValidarUsuario(usuario, contrasena);

                if (permiso != -1)
                {
                    // ¡Login Exitoso!
                    UsuarioActual = usuario;
                    NivelPermiso = permiso;

                    // Esto cierra el Login y devuelve OK a quien lo llamó (usualmente Program.cs)
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos:\n" + ex.Message,
                    "Error Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void lblContraseña_Click(object sender, EventArgs e)
        {

        }

        private void lblUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}
