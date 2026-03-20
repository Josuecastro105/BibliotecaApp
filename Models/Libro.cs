using System;

namespace BibliotecaApp.Models
{
    public class Libro : EntidadBase
    {
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public string Genero { get; set; }
        public int Anio { get; set; }
        public int Cantidad { get; set; }
        public int Disponibles { get; set; }
        public int TotalPrestamos { get; set; }
        public bool EstaDisponible { get { return Disponibles > 0; } }

        public Libro()
        {
            Titulo = string.Empty;
            Autor = string.Empty;
            ISBN = string.Empty;
            Genero = string.Empty;
            Anio = DateTime.Now.Year;
            Cantidad = 1;
            Disponibles = 1;
            TotalPrestamos = 0;
        }

        public override string ObtenerDescripcion() { return Titulo; }
    }
}
