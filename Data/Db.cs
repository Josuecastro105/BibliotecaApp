using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BibliotecaApp.Data
{
    public static class Db
    {
        private static readonly string Carpeta =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos");

        static Db()
        {
            if (!Directory.Exists(Carpeta))
                Directory.CreateDirectory(Carpeta);
        }

        public static List<T> Cargar<T>(string nombre)
        {
            var ruta = Path.Combine(Carpeta, nombre + ".json");
            if (!File.Exists(ruta)) return new List<T>();
            var json = File.ReadAllText(ruta);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public static void Guardar<T>(string nombre, List<T> lista)
        {
            var ruta = Path.Combine(Carpeta, nombre + ".json");
            File.WriteAllText(ruta, JsonConvert.SerializeObject(lista, Formatting.Indented));
        }
    }
}
