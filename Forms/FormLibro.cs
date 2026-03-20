using System;
using System.Drawing;
using System.Windows.Forms;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Forms
{
    public class FormLibro : Form
    {
        private readonly Libro _existente;
        private readonly ServicioLibros _servicio;

        private TextBox txtTitulo   = new TextBox();
        private TextBox txtAutor    = new TextBox();
        private TextBox txtISBN     = new TextBox();
        private TextBox txtGenero   = new TextBox();
        private NumericUpDown nudAnio     = new NumericUpDown();
        private NumericUpDown nudCantidad = new NumericUpDown();

        public FormLibro(Libro libro, ServicioLibros servicio)
        {
            _existente = libro;
            _servicio  = servicio;

            Text            = libro == null ? "Nuevo Libro" : "Editar Libro";
            Size            = new Size(420, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.White;

            ConstruirUI();
            if (libro != null) Rellenar(libro);
        }

        private void ConstruirUI()
        {
            var layout = new TableLayoutPanel();
            layout.Dock        = DockStyle.Fill;
            layout.Padding     = new Padding(16);
            layout.RowCount    = 8;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            nudAnio.Minimum = 1000;
            nudAnio.Maximum = DateTime.Now.Year + 1;
            nudAnio.Value   = DateTime.Now.Year;
            nudCantidad.Minimum = 0;
            nudCantidad.Maximum = 9999;
            nudCantidad.Value   = 1;

            AgregarFila(layout, 0, "Titulo *",    txtTitulo);
            AgregarFila(layout, 1, "Autor *",     txtAutor);
            AgregarFila(layout, 2, "ISBN",        txtISBN);
            AgregarFila(layout, 3, "Genero",      txtGenero);
            AgregarFila(layout, 4, "Fecha Pub.",  nudAnio);
            AgregarFila(layout, 5, "Cantidad",    nudCantidad);

            var pnl = new FlowLayoutPanel();
            pnl.Dock            = DockStyle.Fill;
            pnl.FlowDirection   = FlowDirection.RightToLeft;
            pnl.Padding         = new Padding(0, 4, 0, 0);

            var btnCancelar = CrearBoton("Cancelar", Color.FromArgb(100, 116, 139));
            var btnGuardar  = CrearBoton("Guardar",  Color.FromArgb(30, 58, 138));
            btnCancelar.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            btnGuardar.Click  += Guardar;

            pnl.Controls.Add(btnCancelar);
            pnl.Controls.Add(btnGuardar);
            layout.SetColumnSpan(pnl, 2);
            layout.Controls.Add(pnl, 0, 7);

            Controls.Add(layout);
        }

        private Button CrearBoton(string texto, Color color)
        {
            var btn = new Button();
            btn.Text      = texto;
            btn.Size      = new Size(90, 32);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void AgregarFila(TableLayoutPanel t, int row, string lbl, Control ctrl)
        {
            var l = new Label();
            l.Text      = lbl;
            l.Dock      = DockStyle.Fill;
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.Font      = new Font("Segoe UI", 10);
            ctrl.Dock   = DockStyle.Fill;
            ctrl.Font   = new Font("Segoe UI", 10);
            t.Controls.Add(l, 0, row);
            t.Controls.Add(ctrl, 1, row);
        }

        private void Rellenar(Libro l)
        {
            txtTitulo.Text     = l.Titulo;
            txtAutor.Text      = l.Autor;
            txtISBN.Text       = l.ISBN;
            txtGenero.Text     = l.Genero;
            nudAnio.Value      = l.Anio > 0 ? l.Anio : DateTime.Now.Year;
            nudCantidad.Value  = l.Cantidad;
        }

        private void Guardar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text) || string.IsNullOrWhiteSpace(txtAutor.Text))
            {
                MessageBox.Show("Titulo y Autor son obligatorios.", "Error");
                return;
            }
            try
            {
                if (_existente == null)
                {
                    var nuevo = new Libro();
                    nuevo.Titulo  = txtTitulo.Text.Trim();
                    nuevo.Autor   = txtAutor.Text.Trim();
                    nuevo.ISBN    = txtISBN.Text.Trim();
                    nuevo.Genero  = txtGenero.Text.Trim();
                    nuevo.Anio    = (int)nudAnio.Value;
                    nuevo.Cantidad = (int)nudCantidad.Value;
                    _servicio.Agregar(nuevo);
                }
                else
                {
                    int diff = (int)nudCantidad.Value - _existente.Cantidad;
                    _existente.Titulo      = txtTitulo.Text.Trim();
                    _existente.Autor       = txtAutor.Text.Trim();
                    _existente.ISBN        = txtISBN.Text.Trim();
                    _existente.Genero      = txtGenero.Text.Trim();
                    _existente.Anio        = (int)nudAnio.Value;
                    _existente.Cantidad    = (int)nudCantidad.Value;
                    _existente.Disponibles = Math.Max(0, _existente.Disponibles + diff);
                    _servicio.Actualizar(_existente);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }
    }
}
