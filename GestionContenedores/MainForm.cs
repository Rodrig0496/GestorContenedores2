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
    public partial class MainForm : Form
    {
        private int _nivelPermisoUsuarioActual;
        private string _nombreUsuarioActual;
        public MainForm(int nivelPermiso, string nombreUsuario)
        {
            InitializeComponent();
            _nivelPermisoUsuarioActual = nivelPermiso;
            _nombreUsuarioActual = nombreUsuario;
        }
        private void AbrirFormularioHijo(Control controlHijo) // Nota: cambié Form por Control para que sea más genérico
        {
            if (this.panelContenedor.Controls.Count > 0)
                this.panelContenedor.Controls.Clear();

            controlHijo.Dock = DockStyle.Fill;
            this.panelContenedor.Controls.Add(controlHijo);
            this.panelContenedor.Tag = controlHijo;
            controlHijo.Show();
        }

        private void MostrarDashboard()
        {
            // Usamos las variables privadas de la clase (_nivelPermisoUsuarioActual, etc.)
            VistaDashboard dashboard = new VistaDashboard(
                _nivelPermisoUsuarioActual,
                _nombreUsuarioActual,
                this.AsegurarPermisoAdmin // Pasamos la función verificadora
            );

            // Lo cargamos en el panel principal usando tu método auxiliar
            AbrirFormularioHijo(dashboard);
        }
        // En el evento Click del botón "Dashboard" en tu menú lateral:


        public bool AsegurarPermisoAdmin()
        {
            // 1. Si ya es admin (nivel 0), no hacemos nada y devolvemos true.
            if (_nivelPermisoUsuarioActual == 0)
            {
                return true;
            }

            // 2. Si es invitado (nivel 1), mostramos el formulario de Login.
            DialogResult respuesta = MessageBox.Show("Esta acción requiere permisos de Administrador.\n¿Desea iniciar sesión ahora?",
                                                     "Permiso requerido", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (respuesta == DialogResult.Yes)
            {
                Login formLogin = new Login();
                // Mostramos el login de forma modal (bloquea la ventana de atrás)
                if (formLogin.ShowDialog() == DialogResult.OK)
                {
                    // 3. ¡Éxito! Actualizamos el estado del MainForm con los nuevos datos
                    _nivelPermisoUsuarioActual = formLogin.NivelPermiso;
                    _nombreUsuarioActual = formLogin.UsuarioActual;

                    MessageBox.Show($"Sesión iniciada como: {_nombreUsuarioActual}", "Bienvenido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Opcional: Actualizar algún label en el MainForm que diga el nombre del usuario

                    return true; // Ahora sí es admin
                }
            }

            // Si llegamos aquí, es que dijo que "No" al login, o cerró la ventana de login sin éxito.
            return false;
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            MostrarDashboard();
        }
    }
}
