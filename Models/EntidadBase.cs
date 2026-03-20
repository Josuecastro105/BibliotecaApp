using System;

namespace BibliotecaApp.Models
{
    public abstract class EntidadBase
    {
        public string Id { get; set; }
        public DateTime FechaCreacion { get; set; }

        protected EntidadBase()
        {
            Id = Guid.NewGuid().ToString();
            FechaCreacion = DateTime.Now;
        }

        public abstract string ObtenerDescripcion();
    }
}
