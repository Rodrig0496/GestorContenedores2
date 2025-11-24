using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionContenedores.Services
{
    internal class LinqService
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        // 1. VALIDAR LOGIN
        public int ValidarUsuario(string user, string pass)
        {
            // Buscamos el primer usuario que coincida (LINQ puro)
            var usuarioEncontrado = db.Usuarios
                                      .FirstOrDefault(u => u.Username == user && u.Password == pass);

            if (usuarioEncontrado != null)
            {
                return usuarioEncontrado.NivelPermiso;
            }
            return -1; // No encontrado
        }

        // 2. OBTENER TODOS
        public List<Contenedores> ObtenerContenedores()
        {
            // Así de fácil se hace un SELECT * FROM
            return db.Contenedores.ToList();
        }

        // 3. GUARDAR
        public void GuardarContenedor(string nombre, string direccion, double lat, double lon, string estado)
        {
            db.sp_InsertarContenedor(nombre, direccion, lat, lon, estado);
            //db.SubmitChanges(); // Ejecuta el SQL en la base de datos
        }

        // 4. ACTUALIZAR ESTADO
        public void ActualizarEstado(int id, string nuevoEstado)
        {
            // Primero buscamos el objeto en la BD por su ID
            db.sp_ActualizarEstado(id, nuevoEstado);
        }

        public void EliminarContenedor(int id)
        {
            db.sp_EliminarContenedor(id);
        }
    }
}
