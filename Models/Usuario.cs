using System;

namespace BibliotecaApp.Models
{
    public enum TipoUsuario { Estudiante, Docente, Administrativo, Externo }

    public class Usuario : EntidadBase
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto { get { return (Nombre + " " + Apellido).Trim(); } }
        public string Carnet { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public TipoUsuario Tipo { get; set; }
        public bool Activo { get; set; }
        public int TotalPrestamos { get; set; }

        public Usuario()
        {
            Nombre = string.Empty;
            Apellido = string.Empty;
            Carnet = string.Empty;
            Email = string.Empty;
            Telefono = string.Empty;
            Tipo = TipoUsuario.Estudiante;
            Activo = true;
            TotalPrestamos = 0;
        }

        public override string ObtenerDescripcion() { return NombreCompleto; }
    }
}
