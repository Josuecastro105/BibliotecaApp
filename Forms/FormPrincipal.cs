using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Forms
{
    public class FormPrincipal : Form
    {
        private ServicioLibros    _libros;
        private ServicioUsuarios  _usuarios;
        private ServicioPrestamos _prestamos;

        private DataGridView dgvLibros    = new DataGridView();
        private DataGridView dgvUsuarios  = new DataGridView();
        private DataGridView dgvPrestamos = new DataGridView();
        private TabPage      _tabEstadisticas;

        public FormPrincipal()
        {
            _libros    = new ServicioLibros();
            _usuarios  = new ServicioUsuarios();
            _prestamos = new ServicioPrestamos(_libros, _usuarios);
            _prestamos.ActualizarVencidos();

            Text          = "Sistema de Gestion de Biblioteca";
            Size          = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize   = new Size(900, 600);
            BackColor     = Color.FromArgb(245, 247, 250);

            ConstruirUI();
            CargarLibros();
            CargarUsuarios();
            CargarPrestamos();
        }

        private void ConstruirUI()
        {
            var header = new Panel();
            header.Dock      = DockStyle.Top;
            header.Height    = 55;
            header.BackColor = Color.FromArgb(30, 58, 138);

            var lblHeader = new Label();
            lblHeader.Text      = "  Sistema de Gestion de Biblioteca";
            lblHeader.ForeColor = Color.White;
            lblHeader.Font      = new Font("Segoe UI", 15, FontStyle.Bold);
            lblHeader.Dock      = DockStyle.Fill;
            lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            header.Controls.Add(lblHeader);

            var tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            tabs.Font = new Font("Segoe UI", 10);

            _tabEstadisticas = CrearTabEstadisticas();
            tabs.TabPages.Add(CrearTabLibros());
            tabs.TabPages.Add(CrearTabUsuarios());
            tabs.TabPages.Add(CrearTabPrestamos());
            tabs.TabPages.Add(_tabEstadisticas);

            tabs.SelectedIndexChanged += delegate
            {
                if (tabs.SelectedTab == _tabEstadisticas)
                    RefrescarEstadisticas();
            };

            Controls.Add(tabs);
            Controls.Add(header);
        }

        // =============
        // TAB libros
        // =============
        private TabPage CrearTabLibros()
        {
            var tab = new TabPage("  Libros  ");

            var btnNuevo   = CrearBoton("+ Nuevo",  Color.FromArgb(16, 185, 129));
            var btnEditar  = CrearBoton("Editar",   Color.FromArgb(59, 130, 246));
            var btnElim    = CrearBoton("Eliminar", Color.FromArgb(239, 68, 68));
            var txtBuscar  = new TextBox();
            txtBuscar.Width = 220;
            txtBuscar.Font  = new Font("Segoe UI", 10);
            txtBuscar.Margin = new Padding(0, 0, 8, 0);

            var barra = new FlowLayoutPanel();
            barra.Dock          = DockStyle.Top;
            barra.Height        = 48;
            barra.BackColor     = Color.White;
            barra.Padding       = new Padding(8, 8, 8, 0);
            barra.FlowDirection = FlowDirection.LeftToRight;
            barra.Controls.AddRange(new Control[] { txtBuscar, btnNuevo, btnEditar, btnElim });

            ConfigurarGrilla(dgvLibros);
            dgvLibros.Columns.Add("Id",          "ID");
            dgvLibros.Columns.Add("Titulo",       "Titulo");
            dgvLibros.Columns.Add("Autor",        "Autor");
            dgvLibros.Columns.Add("ISBN",         "ISBN");
            dgvLibros.Columns.Add("Genero",       "Genero");
            dgvLibros.Columns.Add("Anio",         "Publicacion");
            dgvLibros.Columns.Add("Total",        "Total");
            dgvLibros.Columns.Add("Disponibles",  "Disponibles");
            dgvLibros.Columns.Add("Prestamos",    "Prestamos");
            dgvLibros.Columns["Id"].Visible = false;

            btnNuevo.Click += delegate
            {
                var f = new FormLibro(null, _libros);
                if (f.ShowDialog() == DialogResult.OK) CargarLibros();
            };
            btnEditar.Click += delegate
            {
                string id = ObtenerIdSeleccionado(dgvLibros);
                if (id == null) return;
                Libro libro = _libros.ObtenerPorId(id);
                if (libro == null) return;
                var f = new FormLibro(libro, _libros);
                if (f.ShowDialog() == DialogResult.OK) CargarLibros();
            };
            btnElim.Click += delegate
            {
                string id = ObtenerIdSeleccionado(dgvLibros);
                if (id == null) return;
                Libro libro = _libros.ObtenerPorId(id);
                if (libro == null) return;
                if (MessageBox.Show("Eliminar \"" + libro.Titulo + "\"?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                try { _libros.Eliminar(id); CargarLibros(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            };
            txtBuscar.TextChanged += delegate { CargarLibros(txtBuscar.Text); };
            dgvLibros.CellDoubleClick += delegate { btnEditar.PerformClick(); };

            tab.Controls.Add(dgvLibros);
            tab.Controls.Add(barra);
            return tab;
        }

        private void CargarLibros(string filtro = "")
        {
            dgvLibros.Rows.Clear();
            List<Libro> lista = _libros.ObtenerTodos();
            foreach (Libro l in lista)
            {
                if (!string.IsNullOrEmpty(filtro) &&
                    !l.Titulo.ToLower().Contains(filtro.ToLower()) &&
                    !l.Autor.ToLower().Contains(filtro.ToLower()) &&
                    !l.ISBN.ToLower().Contains(filtro.ToLower()))
                    continue;

                int i = dgvLibros.Rows.Add(l.Id, l.Titulo, l.Autor, l.ISBN, l.Genero,
                    l.Anio, l.Cantidad, l.Disponibles, l.TotalPrestamos);
                if (l.Disponibles == 0)
                    dgvLibros.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
            }
        }

        // ========================
        // TAB usuarios
        // ========================
        private TabPage CrearTabUsuarios()
        {
            var tab = new TabPage("  Usuarios  ");

            var btnNuevo  = CrearBoton("+ Nuevo",  Color.FromArgb(16, 185, 129));
            var btnEditar = CrearBoton("Editar",   Color.FromArgb(59, 130, 246));
            var btnElim   = CrearBoton("Eliminar", Color.FromArgb(239, 68, 68));
            var txtBuscar = new TextBox();
            txtBuscar.Width  = 220;
            txtBuscar.Font   = new Font("Segoe UI", 10);
            txtBuscar.Margin = new Padding(0, 0, 8, 0);

            var barra = new FlowLayoutPanel();
            barra.Dock          = DockStyle.Top;
            barra.Height        = 48;
            barra.BackColor     = Color.White;
            barra.Padding       = new Padding(8, 8, 8, 0);
            barra.FlowDirection = FlowDirection.LeftToRight;
            barra.Controls.AddRange(new Control[] { txtBuscar, btnNuevo, btnEditar, btnElim });

            ConfigurarGrilla(dgvUsuarios);
            dgvUsuarios.Columns.Add("Id",        "ID");
            dgvUsuarios.Columns.Add("Nombre",    "Nombre Completo");
            dgvUsuarios.Columns.Add("Carnet",    "Carnet");
            dgvUsuarios.Columns.Add("Email",     "Email");
            dgvUsuarios.Columns.Add("Telefono",  "Telefono");
            dgvUsuarios.Columns.Add("Tipo",      "Tipo");
            dgvUsuarios.Columns.Add("Prestamos", "Prestamos");
            dgvUsuarios.Columns.Add("Estado",    "Estado");
            dgvUsuarios.Columns["Id"].Visible = false;

            btnNuevo.Click += delegate
            {
                var f = new FormUsuario(null, _usuarios);
                if (f.ShowDialog() == DialogResult.OK) CargarUsuarios();
            };
            btnEditar.Click += delegate
            {
                string id = ObtenerIdSeleccionado(dgvUsuarios);
                if (id == null) return;
                Usuario u = _usuarios.ObtenerPorId(id);
                if (u == null) return;
                var f = new FormUsuario(u, _usuarios);
                if (f.ShowDialog() == DialogResult.OK) CargarUsuarios();
            };
            btnElim.Click += delegate
            {
                string id = ObtenerIdSeleccionado(dgvUsuarios);
                if (id == null) return;
                Usuario u = _usuarios.ObtenerPorId(id);
                if (u == null) return;
                if (MessageBox.Show("Eliminar a " + u.NombreCompleto + "?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                try { _usuarios.Eliminar(id); CargarUsuarios(); }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            };
            txtBuscar.TextChanged += delegate { CargarUsuarios(txtBuscar.Text); };
            dgvUsuarios.CellDoubleClick += delegate { btnEditar.PerformClick(); };

            tab.Controls.Add(dgvUsuarios);
            tab.Controls.Add(barra);
            return tab;
        }

        private void CargarUsuarios(string filtro = "")
        {
            dgvUsuarios.Rows.Clear();
            List<Usuario> lista = _usuarios.ObtenerTodos();
            foreach (Usuario u in lista)
            {
                if (!string.IsNullOrEmpty(filtro) &&
                    !u.NombreCompleto.ToLower().Contains(filtro.ToLower()) &&
                    !u.Carnet.ToLower().Contains(filtro.ToLower()))
                    continue;
                dgvUsuarios.Rows.Add(u.Id, u.NombreCompleto, u.Carnet, u.Email,
                    u.Telefono, u.Tipo.ToString(), u.TotalPrestamos,
                    u.Activo ? "Activo" : "Inactivo");
            }
        }

        // ================================================================
        // TAB PRESTAMOS
        // ================================================================
        private TabPage CrearTabPrestamos()
        {
            var tab = new TabPage("  Prestamos  ");

            var btnNuevo  = CrearBoton("+ Nuevo Prestamo", Color.FromArgb(16, 185, 129), 150);
            var btnDev    = CrearBoton("Devolver",          Color.FromArgb(245, 158, 11));
            var cmbEstado = new ComboBox();
            cmbEstado.Width         = 130;
            cmbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstado.Font          = new Font("Segoe UI", 10);
            cmbEstado.Margin        = new Padding(0, 2, 8, 0);
            cmbEstado.Items.AddRange(new object[] { "Todos", "Activo", "Devuelto", "Vencido" });
            cmbEstado.SelectedIndex = 0;

            var barra = new FlowLayoutPanel();
            barra.Dock          = DockStyle.Top;
            barra.Height        = 48;
            barra.BackColor     = Color.White;
            barra.Padding       = new Padding(8, 8, 8, 0);
            barra.FlowDirection = FlowDirection.LeftToRight;
            barra.Controls.AddRange(new Control[] { cmbEstado, btnNuevo, btnDev });

            ConfigurarGrilla(dgvPrestamos);
            dgvPrestamos.Columns.Add("Id",       "ID");
            dgvPrestamos.Columns.Add("Libro",    "Libro");
            dgvPrestamos.Columns.Add("Usuario",  "Usuario");
            dgvPrestamos.Columns.Add("Fecha",    "Fecha Prestamo");
            dgvPrestamos.Columns.Add("Vence",    "Vence");
            dgvPrestamos.Columns.Add("Devuelto", "Devuelto");
            dgvPrestamos.Columns.Add("Estado",   "Estado");
            dgvPrestamos.Columns.Add("Retraso",  "Dias Retraso");
            dgvPrestamos.Columns["Id"].Visible = false;

            btnNuevo.Click += delegate
            {
                var f = new FormPrestamo(_libros, _usuarios);
                if (f.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _prestamos.Registrar(f.LibroId, f.UsuarioId, f.Dias, f.Observaciones);
                        CargarPrestamos();
                        MessageBox.Show("Prestamo registrado.", "Exito");
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
                }
            };
            btnDev.Click += delegate
            {
                string id = ObtenerIdSeleccionado(dgvPrestamos);
                if (id == null) return;
                if (MessageBox.Show("Registrar devolucion?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                try
                {
                    _prestamos.Devolver(id);
                    CargarPrestamos();
                    MessageBox.Show("Devolucion registrada.", "Exito");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            };
            cmbEstado.SelectedIndexChanged += delegate
            {
                string sel = cmbEstado.SelectedItem != null ? cmbEstado.SelectedItem.ToString() : "Todos";
                CargarPrestamos(sel);
            };

            tab.Controls.Add(dgvPrestamos);
            tab.Controls.Add(barra);
            return tab;
        }

        private void CargarPrestamos(string filtro = "Todos")
        {
            dgvPrestamos.Rows.Clear();
            List<Prestamo> lista = _prestamos.ObtenerTodos();
            lista.Sort((a, b) => b.FechaPrestamo.CompareTo(a.FechaPrestamo));
            foreach (Prestamo p in lista)
            {
                if (filtro != "Todos" && p.Estado.ToString() != filtro) continue;
                int i = dgvPrestamos.Rows.Add(
                    p.Id, p.TituloLibro, p.NombreUsuario,
                    p.FechaPrestamo.ToString("dd/MM/yyyy"),
                    p.FechaVence.ToString("dd/MM/yyyy"),
                    p.FechaDevolucion.HasValue ? p.FechaDevolucion.Value.ToString("dd/MM/yyyy") : "-",
                    p.Estado.ToString(),
                    p.DiasRetraso > 0 ? p.DiasRetraso.ToString() : "-");

                if (p.Estado == EstadoPrestamo.Vencido)
                    dgvPrestamos.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                else if (p.Estado == EstadoPrestamo.Devuelto)
                    dgvPrestamos.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 230);
            }
        }

        // ================================================================
        // TAB ESTADISTICAS
        // ================================================================
        private TabPage CrearTabEstadisticas()
        {
            var tab = new TabPage("  Estadisticas  ");
            RefrescarContenidoEstadisticas(tab);
            return tab;
        }

        private void RefrescarEstadisticas()
        {
            if (_tabEstadisticas == null) return;
            _tabEstadisticas.Controls.Clear();
            RefrescarContenidoEstadisticas(_tabEstadisticas);
        }

        private void RefrescarContenidoEstadisticas(TabPage tab)
        {
            var scroll = new Panel();
            scroll.Dock       = DockStyle.Fill;
            scroll.AutoScroll = true;
            scroll.BackColor  = Color.FromArgb(245, 247, 250);

            string[] textos = { "Total Libros", "Libros Disponibles", "Total Usuarios", "Usuarios Activos", "Prestamos Activos", "Prest. Vencidos" };
            string[] valores = {
                _libros.ObtenerTodos().Sum(l => l.Cantidad).ToString(),
                _libros.ObtenerDisponibles().Count.ToString(),
                _usuarios.ObtenerTodos().Count.ToString(),
                _usuarios.ObtenerActivos().Count.ToString(),
                _prestamos.ObtenerActivos().Count.ToString(),
                _prestamos.ObtenerVencidos().Count.ToString()
            };
            Color[] colores = {
                Color.FromArgb(30,58,138), Color.FromArgb(16,185,129),
                Color.FromArgb(124,58,237), Color.FromArgb(59,130,246),
                Color.FromArgb(245,158,11), Color.FromArgb(239,68,68)
            };

            var flowKpi = new FlowLayoutPanel();
            flowKpi.Location      = new Point(8, 8);
            flowKpi.Size          = new Size(1020, 120);
            flowKpi.FlowDirection = FlowDirection.LeftToRight;
            flowKpi.BackColor     = Color.FromArgb(245, 247, 250);

            for (int k = 0; k < textos.Length; k++)
            {
                Color color = colores[k];
                var tarjeta = new Panel();
                tarjeta.Size      = new Size(158, 100);
                tarjeta.BackColor = Color.White;
                tarjeta.Margin    = new Padding(0, 0, 8, 0);

                var barra = new Panel();
                barra.Width     = 5;
                barra.Dock      = DockStyle.Left;
                barra.BackColor = color;

                var lblVal = new Label();
                lblVal.Text      = valores[k];
                lblVal.Font      = new Font("Segoe UI", 26, FontStyle.Bold);
                lblVal.ForeColor = color;
                lblVal.Location  = new Point(12, 10);
                lblVal.Size      = new Size(140, 48);
                lblVal.TextAlign = ContentAlignment.MiddleLeft;

                var lblTxt = new Label();
                lblTxt.Text      = textos[k];
                lblTxt.Font      = new Font("Segoe UI", 9);
                lblTxt.ForeColor = Color.FromArgb(100, 116, 139);
                lblTxt.Location  = new Point(12, 62);
                lblTxt.Size      = new Size(140, 24);
                lblTxt.TextAlign = ContentAlignment.MiddleLeft;

                tarjeta.Controls.Add(barra);
                tarjeta.Controls.Add(lblVal);
                tarjeta.Controls.Add(lblTxt);
                flowKpi.Controls.Add(tarjeta);
            }
            scroll.Controls.Add(flowKpi);

            var lblG1 = new Label();
            lblG1.Text      = "Top 5 Libros mas Prestados";
            lblG1.Font      = new Font("Segoe UI", 11, FontStyle.Bold);
            lblG1.ForeColor = Color.FromArgb(30, 58, 138);
            lblG1.Location  = new Point(8, 138);
            lblG1.AutoSize  = true;
            scroll.Controls.Add(lblG1);

            List<(string, double)> datosLibros = new List<(string, double)>();
            foreach (Libro l in _libros.TopPrestados(5))
            {
                string titulo = l.Titulo.Length > 22 ? l.Titulo.Substring(0, 22) + "..." : l.Titulo;
                datosLibros.Add((titulo, (double)l.TotalPrestamos));
            }
            scroll.Controls.Add(CrearGrafico(datosLibros, Color.FromArgb(59, 130, 246), new Point(8, 165), new Size(490, 280)));

            var lblG2 = new Label();
            lblG2.Text      = "Top 5 Usuarios mas Activos";
            lblG2.Font      = new Font("Segoe UI", 11, FontStyle.Bold);
            lblG2.ForeColor = Color.FromArgb(30, 58, 138);
            lblG2.Location  = new Point(510, 138);
            lblG2.AutoSize  = true;
            scroll.Controls.Add(lblG2);

            List<(string, double)> datosUsuarios = new List<(string, double)>();
            foreach (Usuario u in _usuarios.TopActivos(5))
            {
                string nombre = u.NombreCompleto.Length > 20 ? u.NombreCompleto.Substring(0, 20) + "..." : u.NombreCompleto;
                datosUsuarios.Add((nombre, (double)u.TotalPrestamos));
            }
            scroll.Controls.Add(CrearGrafico(datosUsuarios, Color.FromArgb(16, 185, 129), new Point(510, 165), new Size(490, 280)));

            tab.Controls.Add(scroll);
        }

        private Panel CrearGrafico(List<(string etiq, double val)> datos, Color color, Point loc, Size sz)
        {
            var p = new Panel();
            p.Location  = loc;
            p.Size      = sz;
            p.BackColor = Color.White;

            p.Paint += delegate(object sender, PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.TextRenderingHint  = TextRenderingHint.AntiAlias;

                int pl = 40, pb = 90, pt = 25, pr = 15;
                int w = p.Width - pl - pr;
                int h = p.Height - pt - pb;

                g.FillRectangle(new SolidBrush(Color.FromArgb(248, 250, 252)), pl, pt, w, h);

                if (datos.Count == 0)
                {
                    g.DrawString("Sin datos aun", new Font("Segoe UI", 9), Brushes.Gray, pl + 10, pt + 10);
                    return;
                }

                double max = 1;
                foreach (var d in datos) if (d.val > max) max = d.val;

                int slot = w / datos.Count;
                int bw   = (int)(slot * 0.55);
                Font fe  = new Font("Segoe UI", 8);
                Font fv  = new Font("Segoe UI", 8, FontStyle.Bold);

                for (int i = 0; i < datos.Count; i++)
                {
                    double prop = datos[i].val / max;
                    int bh = Math.Max(2, (int)(h * prop));
                    int x  = pl + i * slot + (slot - bw) / 2;
                    int y  = pt + h - bh;

                    g.FillRectangle(new SolidBrush(color), x, y, bw, bh);

                    string vs   = datos[i].val.ToString("0");
                    SizeF  vsz  = g.MeasureString(vs, fv);
                    g.DrawString(vs, fv, new SolidBrush(Color.FromArgb(15, 23, 42)),
                        x + bw / 2f - vsz.Width / 2, y - vsz.Height - 2);

                    float  cx    = x + bw / 2f;
                    float  cy    = pt + h + 6;
                    var    state = g.Save();
                    g.TranslateTransform(cx, cy);
                    g.RotateTransform(38);
                    g.DrawString(datos[i].etiq, fe, new SolidBrush(Color.FromArgb(60, 60, 60)), 0, 0);
                    g.Restore(state);
                }

                for (int n = 1; n <= 4; n++)
                {
                    int yl = pt + h - (int)(h * n / 4.0);
                    g.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), pl, yl, pl + w, yl);
                    g.DrawString(((int)(max * n / 4)).ToString(), fe,
                        new SolidBrush(Color.FromArgb(100, 116, 139)), 2, yl - 7);
                }
                g.DrawRectangle(new Pen(Color.FromArgb(226, 232, 240)), pl, pt, w, h);
            };
            return p;
        }

        // ================================================================
        // HELPERS
        // ================================================================
        private Button CrearBoton(string texto, Color color, int ancho = 110)
        {
            var btn = new Button();
            btn.Text      = texto;
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Size      = new Size(ancho, 32);
            btn.Margin    = new Padding(0, 0, 6, 0);
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void ConfigurarGrilla(DataGridView dgv)
        {
            dgv.Dock                  = DockStyle.Fill;
            dgv.AllowUserToAddRows    = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly              = true;
            dgv.SelectionMode         = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect           = false;
            dgv.AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor       = Color.White;
            dgv.BorderStyle           = BorderStyle.None;
            dgv.RowHeadersVisible     = false;
            dgv.Font                  = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height    = 32;
            dgv.ColumnHeadersHeight   = 38;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 58, 138);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        }

        private string ObtenerIdSeleccionado(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una fila primero.", "Aviso");
                return null;
            }
            object val = dgv.SelectedRows[0].Cells["Id"].Value;
            return val != null ? val.ToString() : null;
        }
    }
}
