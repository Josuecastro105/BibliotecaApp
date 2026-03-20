# BibliotecaApp

Sistema de Gestion de Biblioteca desarrollado en **C# con WinForms y .NET 8**.

---

## Descripcion

BibliotecaApp es una aplicacion de escritorio que permite administrar libros, usuarios y prestamos de una biblioteca. Aplica los principios de Programacion Orientada a Objetos, persiste datos en archivos JSON y presenta graficos estadisticos integrados.

---

## Principios OOP aplicados

| Principio | Aplicacion en el proyecto |
|---|---|
| **Clases** | `Libro`, `Usuario`, `Prestamo`, `EntidadBase`, servicios, formularios |
| **Herencia** | `EntidadBase` es heredada por `Libro`, `Usuario` y `Prestamo` |
| **Encapsulamiento** | Propiedades con validacion en los servicios antes de guardar |
| **Polimorfismo** | Metodo abstracto `ObtenerDescripcion()` implementado distinto en cada modelo |

### Estructuras de datos utilizadas
- **List** `List<T>` — almacenamiento principal de entidades en memoria
- **Dictionary** — indice interno para busqueda rapida por ID en los servicios
- **Matrix** `int[,]` — calculo de estadisticas de prestamos por periodo

---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10 u 11
- Visual Studio 2022 o superior (recomendado)

### Dependencias NuGet
| Paquete | Version | Uso |
|---|---|---|
| `Newtonsoft.Json` | 13.0.3 | Serializacion y persistencia de datos en JSON |

---

## Instalacion

### Opcion A — Visual Studio (recomendado)

```bash
# 1. Clonar el repositorio
git clone https://github.com/tu-usuario/BibliotecaApp.git

# 2. Abrir Visual Studio
# Archivo -> Abrir -> Proyecto o Solucion
# Seleccionar BibliotecaApp.csproj

# 3. Restaurar paquetes (automatico al abrir)
# Visual Studio descargara Newtonsoft.Json automaticamente

# 4. Ejecutar
# Presionar F5 o el boton verde "Iniciar"
```

### Opcion B — Linea de comandos

```bash
git clone https://github.com/tu-usuario/BibliotecaApp.git
cd BibliotecaApp
dotnet restore
dotnet run
```

---

## Uso del sistema

La aplicacion se divide en 4 pestanas:

### Libros
- **+ Nuevo** — abre formulario para registrar un libro (titulo, autor, ISBN, genero, fecha de publicacion, cantidad)
- **Editar** — selecciona un libro de la lista y modifica sus datos
- **Eliminar** — elimina un libro (no permitido si tiene prestamos activos)
- **Buscar** — filtra en tiempo real por titulo, autor o ISBN
- Doble clic en una fila para editar rapidamente

### Usuarios
- **+ Nuevo** — registra un usuario (nombre, apellido, carnet, email, telefono, tipo, estado)
- **Editar / Eliminar** — igual que libros
- **Buscar** — filtra por nombre o carnet
- Tipos disponibles: Estudiante, Docente, Administrativo, Externo

### Prestamos
- **+ Nuevo Prestamo** — selecciona libro disponible y usuario activo, define dias de prestamo
- **Devolver** — selecciona un prestamo activo y registra su devolucion
- **Filtrar** — por estado: Todos, Activo, Devuelto, Vencido
- Los prestamos vencidos se marcan automaticamente al abrir la app

### Estadisticas
- Se actualiza automaticamente cada vez que entras a la pestana
- Muestra 6 indicadores clave: total libros, disponibles, usuarios, activos, prestamos activos y vencidos
- Grafico de barras: Top 5 libros mas prestados
- Grafico de barras: Top 5 usuarios mas activos

---

## Estructura del proyecto

```
BibliotecaApp/
├── Models/
│   ├── EntidadBase.cs       # Clase base abstracta (herencia + polimorfismo)
│   ├── Libro.cs             # Modelo de libro
│   ├── Usuario.cs           # Modelo de usuario con enum TipoUsuario
│   └── Prestamo.cs          # Modelo de prestamo con enum EstadoPrestamo
├── Services/
│   └── Servicios.cs         # ServicioLibros, ServicioUsuarios, ServicioPrestamos
├── Data/
│   └── Db.cs                # Persistencia generica en archivos JSON
├── Forms/
│   ├── FormPrincipal.cs     # Ventana principal con TabControl
│   ├── FormLibro.cs         # Dialogo crear/editar libro
│   ├── FormUsuario.cs       # Dialogo crear/editar usuario
│   └── FormPrestamo.cs      # Dialogo registrar prestamo
├── Program.cs               # Punto de entrada
├── BibliotecaApp.csproj     # Configuracion del proyecto
├── .gitignore
└── README.md
```

### Almacenamiento de datos
Los datos se guardan localmente en la carpeta `datos/` generada automaticamente junto al ejecutable:
```
datos/
├── libros.json
├── usuarios.json
└── prestamos.json
```
Estos archivos son locales y privados — no se sincronizan con ningun servidor.

---

## Guia de despliegue

### Ejecutar en modo desarrollo
```bash
dotnet run
```

### Publicar ejecutable para entregar o compartir

```bash
# Ejecutable unico autocontenido para Windows 64 bits
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./dist
```

El archivo `dist/BibliotecaApp.exe` se puede copiar y ejecutar en cualquier PC con Windows sin necesidad de instalar .NET.

### Publicar ejecutable para Windows 32 bits (maquinas antiguas)
```bash
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true -o ./dist32
```

---

## Flujo Git del proyecto

```
main        <- version estable final (v1.0.0)
develop     <- integracion de todas las features
feature/*   <- una rama por funcionalidad
```

### Ramas utilizadas

| Rama | Descripcion |
|---|---|
| `main` | Version estable y entregable |
| `develop` | Rama de integracion |
| `feature/modelos` | Clases EntidadBase, Libro, Usuario, Prestamo |
| `feature/servicios` | ServicioLibros, ServicioUsuarios, ServicioPrestamos, Db |
| `feature/interfaz` | FormPrincipal, FormLibro, FormUsuario, FormPrestamo |

### Convencion de commits

| Prefijo | Uso |
|---|---|
| `feat:` | Nueva funcionalidad |
| `fix:` | Correccion de error |
| `chore:` | Configuracion o estructura |
| `docs:` | Documentacion |
| `refactor:` | Mejora de codigo sin cambiar funcionalidad |

---

## Autor

Proyecto academico — Sistema de Gestion de Biblioteca
Desarrollado con C# · WinForms · .NET 8 · Newtonsoft.Json
