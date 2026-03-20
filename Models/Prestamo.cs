using System;

namespace BibliotecaApp.Models
{
    public enum EstadoPrestamo { Activo, Devuelto, Vencido }

    public class Prestamo : EntidadBase
    {
        public string LibroId { get; set; }
        public string UsuarioId { get; set; }
        public string TituloLibro { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVence { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public EstadoPrestamo Estado { get; set; }
        public string Observaciones { get; set; }

        public bool EstaVencido
        {
            get { return Estado == EstadoPrestamo.Activo && DateTime.Now > FechaVence; }
        }

        public int DiasRetraso
        {
            get { return EstaVencido ? (int)(DateTime.Now - FechaVence).TotalDays : 0; }
        }

        public Prestamo()
        {
            LibroId = string.Empty;
            UsuarioId = string.Empty;
            TituloLibro = string.Empty;
            NombreUsuario = string.Empty;
            FechaPrestamo = DateTime.Now;
            FechaVence = DateTime.Now.AddDays(14);
            Estado = EstadoPrestamo.Activo;
            Observaciones = string.Empty;
        }

        public override string ObtenerDescripcion()
        {
            return TituloLibro + " -> " + NombreUsuario;
        }
    }
}
