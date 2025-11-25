using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.Contenedores);
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
        public int ObtenerProximoId()
        {
            // Busca el ID más alto actual. Si no hay ninguno (null), devuelve 0.
            int maxId = db.Contenedores
                          .Select(c => (int?)c.Id) // Casteamos a int? por si la tabla está vacía
                          .Max() ?? 0;

            return maxId + 1; // El siguiente será Max + 1
        }
        public bool ValidarTrabajador(string usuario, string password)
        {
            // 1. Encriptamos lo que escribió el usuario
            string passwordEncriptada = EncriptarPassword(password);

            // 2. Comparamos hash con hash
            var trabajador = db.Trabajadores
                .FirstOrDefault(t => t.UsuarioLogin == usuario
                                  && t.PasswordHash == passwordEncriptada // <--- CAMBIO AQUÍ
                                  && t.Estado == true);

            return trabajador != null;
        }
        private string EncriptarPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir el string a array de bytes
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir los bytes a string hexadecimal (que es lo que tienes en la BD)
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" para formato hexadecimal
                }
                return builder.ToString().ToUpper(); // ToUpper() porque tu hash está en mayúsculas
            }
        }
    }
}
