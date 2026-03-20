using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Forms
{
    public class FormPrestamo : Form
    {
        public string LibroId       { get; private set; }
        public string UsuarioId     { get; private set; }
        public int    Dias          { get; private set; }
        public string Observaciones { get; private set; }

        private ComboBox      cmbLibro   = new ComboBox();
        private ComboBox      cmbUsuario = new ComboBox();
        private NumericUpDown nudDias    = new NumericUpDown();
        private TextBox       txtObs     = new TextBox();
        private Label         lblDisp    = new Label();

        private List<Libro>   _libros;
        private List<Usuario> _usuarios;

        public FormPrestamo(ServicioLibros servicioLibros, ServicioUsuarios servicioUsuarios)
        {
            _libros   = servicioLibros.ObtenerDisponibles();
            _usuarios = servicioUsuarios.ObtenerActivos();

            Dias          = 14;
            Observaciones = string.Empty;

            Text            = "Registrar Prestamo";
            Size            = new Size(420, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.White;

            ConstruirUI();
        }

        private void ConstruirUI()
        {
            var layout = new TableLayoutPanel();
            layout.Dock        = DockStyle.Fill;
            layout.Padding     = new Padding(16);
            layout.RowCount    = 7;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cmbLibro.DropDownStyle   = ComboBoxStyle.DropDownList;
            cmbUsuario.DropDownStyle = ComboBoxStyle.DropDownList;
            nudDias.Minimum = 1;
            nudDias.Maximum = 90;
            nudDias.Value   = 14;
            txtObs.Multiline = true;
            txtObs.Height    = 50;
            lblDisp.ForeColor = Color.FromArgb(16, 185, 129);
            lblDisp.Font      = new Font("Segoe UI", 9);

            foreach (Libro l in _libros)
                cmbLibro.Items.Add(l.Titulo + " [" + l.Disponibles + " disp.]");
            foreach (Usuario u in _usuarios)
                cmbUsuario.Items.Add(u.NombreCompleto + " [" + u.Carnet + "]");

            if (cmbLibro.Items.Count > 0)   cmbLibro.SelectedIndex   = 0;
            if (cmbUsuario.Items.Count > 0) cmbUsuario.SelectedIndex = 0;

            cmbLibro.SelectedIndexChanged += delegate
            {
                if (cmbLibro.SelectedIndex >= 0 && cmbLibro.SelectedIndex < _libros.Count)
                {
                    Libro l = _libros[cmbLibro.SelectedIndex];
                    lblDisp.Text = l.Disponibles + " de " + l.Cantidad + " disponibles";
                }
            };
            if (cmbLibro.Items.Count > 0)
            {
                Libro l0 = _libros[0];
                lblDisp.Text = l0.Disponibles + " de " + l0.Cantidad + " disponibles";
            }

            var lblVence = new Label();
            lblVence.Dock      = DockStyle.Fill;
            lblVence.Font      = new Font("Segoe UI", 9);
            lblVence.ForeColor = Color.FromArgb(100, 116, 139);
            lblVence.Text      = "Vence: " + DateTime.Now.AddDays(14).ToString("dd/MM/yyyy");

            nudDias.ValueChanged += delegate
            {
                lblVence.Text = "Vence: " + DateTime.Now.AddDays((double)nudDias.Value).ToString("dd/MM/yyyy");
            };

            AgregarFila(layout, 0, "Libro *",     cmbLibro);
            AgregarFila(layout, 1, "Disponibles", lblDisp);
            AgregarFila(layout, 2, "Usuario *",   cmbUsuario);
            AgregarFila(layout, 3, "Dias",        nudDias);
            AgregarFila(layout, 4, "Observacion", txtObs);
            AgregarFila(layout, 5, "",            lblVence);

            var pnl = new FlowLayoutPanel();
            pnl.Dock          = DockStyle.Fill;
            pnl.FlowDirection = FlowDirection.RightToLeft;

            var btnCancelar = CrearBoton("Cancelar", Color.FromArgb(100, 116, 139));
            var btnGuardar  = CrearBoton("Registrar", Color.FromArgb(30, 58, 138));
            btnCancelar.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            btnGuardar.Click  += Guardar;

            pnl.Controls.Add(btnCancelar);
            pnl.Controls.Add(btnGuardar);
            layout.SetColumnSpan(pnl, 2);
            layout.Controls.Add(pnl, 0, 6);

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

        private void Guardar(object sender, EventArgs e)
        {
            if (cmbLibro.SelectedIndex < 0 || cmbUsuario.SelectedIndex < 0)
            {
                MessageBox.Show("Selecciona un libro y un usuario.", "Error");
                return;
            }
            LibroId       = _libros[cmbLibro.SelectedIndex].Id;
            UsuarioId     = _usuarios[cmbUsuario.SelectedIndex].Id;
            Dias          = (int)nudDias.Value;
            Observaciones = txtObs.Text;
            DialogResult  = DialogResult.OK;
            Close();
        }
    }
}
