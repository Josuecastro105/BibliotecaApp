using System;
using System.Collections.Generic;
using System.Linq;
using BibliotecaApp.Data;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services
{
    // ----------------------------------------
    // Servicio de Libros
    // ----------------------------------------
    public class ServicioLibros
    {
        private List<Libro> _lista;

        public ServicioLibros()
        {
            _lista = Db.Cargar<Libro>("libros");
        }

        public List<Libro> ObtenerTodos() { return new List<Libro>(_lista); }

        public Libro ObtenerPorId(string id)
        {
            return _lista.FirstOrDefault(l => l.Id == id);
        }

        public void Agregar(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                throw new Exception("El titulo es obligatorio.");
            if (string.IsNullOrWhiteSpace(libro.Autor))
                throw new Exception("El autor es obligatorio.");
            if (!string.IsNullOrWhiteSpace(libro.ISBN) && _lista.Any(l => l.ISBN == libro.ISBN))
                throw new Exception("Ya existe un libro con ese ISBN.");
            libro.Disponibles = libro.Cantidad;
            _lista.Add(libro);
            Db.Guardar("libros", _lista);
        }

        public void Actualizar(Libro libro)
        {
            int idx = _lista.FindIndex(l => l.Id == libro.Id);
            if (idx < 0) throw new Exception("Libro no encontrado.");
            _lista[idx] = libro;
            Db.Guardar("libros", _lista);
        }

        public void Eliminar(string id)
        {
            Libro libro = ObtenerPorId(id);
            if (libro == null) throw new Exception("Libro no encontrado.");
            int prestados = libro.Cantidad - libro.Disponibles;
            if (prestados > 0)
                throw new Exception("No se puede eliminar: hay " + prestados + " ejemplar(es) prestado(s).");
            _lista.RemoveAll(l => l.Id == id);
            Db.Guardar("libros", _lista);
        }

        public void ReducirDisponible(string id)
        {
            Libro l = ObtenerPorId(id);
            if (l == null) throw new Exception("Libro no encontrado.");
            if (l.Disponibles <= 0) throw new Exception("No hay ejemplares disponibles.");
            l.Disponibles--;
            l.TotalPrestamos++;
            Actualizar(l);
        }

        public void AumentarDisponible(string id)
        {
            Libro l = ObtenerPorId(id);
            if (l == null) throw new Exception("Libro no encontrado.");
            l.Disponibles++;
            Actualizar(l);
        }

        public List<Libro> ObtenerDisponibles()
        {
            return _lista.Where(l => l.EstaDisponible).ToList();
        }

        public List<Libro> TopPrestados(int n)
        {
            return _lista.OrderByDescending(l => l.TotalPrestamos).Take(n).ToList();
        }
    }

    // ----------------------------------------
    // Servicio de Usuarios
    // ----------------------------------------
    public class ServicioUsuarios
    {
        private List<Usuario> _lista;

        public ServicioUsuarios()
        {
            _lista = Db.Cargar<Usuario>("usuarios");
        }

        public List<Usuario> ObtenerTodos() { return new List<Usuario>(_lista); }

        public Usuario ObtenerPorId(string id)
        {
            return _lista.FirstOrDefault(u => u.Id == id);
        }

        public void Agregar(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.Nombre))
                throw new Exception("El nombre es obligatorio.");
            if (!string.IsNullOrWhiteSpace(u.Carnet) && _lista.Any(x => x.Carnet == u.Carnet))
                throw new Exception("Ya existe un usuario con ese carnet.");
            _lista.Add(u);
            Db.Guardar("usuarios", _lista);
        }

        public void Actualizar(Usuario u)
        {
            int idx = _lista.FindIndex(x => x.Id == u.Id);
            if (idx < 0) throw new Exception("Usuario no encontrado.");
            _lista[idx] = u;
            Db.Guardar("usuarios", _lista);
        }

        public void Eliminar(string id)
        {
            if (!_lista.Any(u => u.Id == id)) throw new Exception("Usuario no encontrado.");
            _lista.RemoveAll(u => u.Id == id);
            Db.Guardar("usuarios", _lista);
        }

        public void IncrementarPrestamos(string id)
        {
            Usuario u = ObtenerPorId(id);
            if (u != null) { u.TotalPrestamos++; Actualizar(u); }
        }

        public List<Usuario> ObtenerActivos()
        {
            return _lista.Where(u => u.Activo).ToList();
        }

        public List<Usuario> TopActivos(int n)
        {
            return _lista.OrderByDescending(u => u.TotalPrestamos).Take(n).ToList();
        }
    }

    // ----------------------------------------
    // Servicio de Prestamos
    // ----------------------------------------
    public class ServicioPrestamos
    {
        private List<Prestamo> _lista;
        private readonly ServicioLibros _libros;
        private readonly ServicioUsuarios _usuarios;

        public ServicioPrestamos(ServicioLibros libros, ServicioUsuarios usuarios)
        {
            _libros   = libros;
            _usuarios = usuarios;
            _lista    = Db.Cargar<Prestamo>("prestamos");
        }

        public List<Prestamo> ObtenerTodos() { return new List<Prestamo>(_lista); }

        public Prestamo ObtenerPorId(string id)
        {
            return _lista.FirstOrDefault(p => p.Id == id);
        }

        public void Registrar(string libroId, string usuarioId, int dias, string obs)
        {
            Libro libro = _libros.ObtenerPorId(libroId);
            if (libro == null) throw new Exception("Libro no encontrado.");
            Usuario usuario = _usuarios.ObtenerPorId(usuarioId);
            if (usuario == null) throw new Exception("Usuario no encontrado.");
            if (!libro.EstaDisponible)
                throw new Exception("No hay ejemplares disponibles de \"" + libro.Titulo + "\".");
            if (!usuario.Activo)
                throw new Exception("El usuario esta inactivo.");
            if (_lista.Any(x => x.LibroId == libroId && x.UsuarioId == usuarioId && x.Estado == EstadoPrestamo.Activo))
                throw new Exception("El usuario ya tiene este libro prestado.");

            var p = new Prestamo
            {
                LibroId       = libroId,
                UsuarioId     = usuarioId,
                TituloLibro   = libro.Titulo,
                NombreUsuario = usuario.NombreCompleto,
                FechaPrestamo = DateTime.Now,
                FechaVence    = DateTime.Now.AddDays(dias),
                Observaciones = obs
            };
            _libros.ReducirDisponible(libroId);
            _usuarios.IncrementarPrestamos(usuarioId);
            _lista.Add(p);
            Db.Guardar("prestamos", _lista);
        }

        public void Devolver(string prestamoId)
        {
            Prestamo p = ObtenerPorId(prestamoId);
            if (p == null) throw new Exception("Prestamo no encontrado.");
            if (p.Estado == EstadoPrestamo.Devuelto)
                throw new Exception("Este prestamo ya fue devuelto.");
            p.Estado = EstadoPrestamo.Devuelto;
            p.FechaDevolucion = DateTime.Now;
            _libros.AumentarDisponible(p.LibroId);
            int idx = _lista.FindIndex(x => x.Id == prestamoId);
            _lista[idx] = p;
            Db.Guardar("prestamos", _lista);
        }

        public int ActualizarVencidos()
        {
            int n = 0;
            for (int i = 0; i < _lista.Count; i++)
            {
                if (_lista[i].Estado == EstadoPrestamo.Activo && DateTime.Now > _lista[i].FechaVence)
                {
                    _lista[i].Estado = EstadoPrestamo.Vencido;
                    n++;
                }
            }
            if (n > 0) Db.Guardar("prestamos", _lista);
            return n;
        }

        public List<Prestamo> ObtenerActivos()
        {
            return _lista.Where(p => p.Estado == EstadoPrestamo.Activo).ToList();
        }

        public List<Prestamo> ObtenerVencidos()
        {
            return _lista.Where(p => p.Estado == EstadoPrestamo.Vencido).ToList();
        }
    }
}
