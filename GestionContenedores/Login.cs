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
using System.Runtime.InteropServices;

namespace GestionContenedores
{
    public partial class Login : Form
    {

        public int NivelPermiso { get; private set; } = -1; 
        public string UsuarioActual { get; private set; } = "";
        public bool EsModoTrabajador { get; set; } = false;
        private LinqService _service = new LinqService();
        public Login()
        {
            InitializeComponent();
        }


        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        // Enlaza este evento al evento "MouseDown" del Formulario y del Panel Izquierdo
        private void Login_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // LÓGICA DIVIDIDA
            if (EsModoTrabajador)
            {
                // --- LOGICA PARA TRABAJADORES ---
                if (_service.ValidarTrabajador(txtUsuario.Text, txtContraseña.Text))
                {
                    this.UsuarioActual = txtUsuario.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Credenciales de TRABAJADOR incorrectas o cuenta inactiva.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // --- LOGICA ORIGINAL (ADMIN/VECINOS) ---
                int permiso = _service.ValidarUsuario(txtUsuario.Text, txtContraseña.Text);

                if (permiso != -1)
                {
                    this.NivelPermiso = permiso;
                    this.UsuarioActual = txtUsuario.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
