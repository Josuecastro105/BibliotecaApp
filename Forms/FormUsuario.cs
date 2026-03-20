using System;
using System.Drawing;
using System.Windows.Forms;
using BibliotecaApp.Models;
using BibliotecaApp.Services;

namespace BibliotecaApp.Forms
{
    public class FormUsuario : Form
    {
        private readonly Usuario _existente;
        private readonly ServicioUsuarios _servicio;

        private TextBox  txtNombre   = new TextBox();
        private TextBox  txtApellido = new TextBox();
        private TextBox  txtCarnet   = new TextBox();
        private TextBox  txtEmail    = new TextBox();
        private TextBox  txtTelefono = new TextBox();
        private ComboBox cmbTipo     = new ComboBox();
        private CheckBox chkActivo   = new CheckBox();

        public FormUsuario(Usuario usuario, ServicioUsuarios servicio)
        {
            _existente = usuario;
            _servicio  = servicio;

            Text            = usuario == null ? "Nuevo Usuario" : "Editar Usuario";
            Size            = new Size(420, 390);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            StartPosition   = FormStartPosition.CenterParent;
            BackColor       = Color.White;

            ConstruirUI();
            if (usuario != null) Rellenar(usuario);
        }

        private void ConstruirUI()
        {
            var layout = new TableLayoutPanel();
            layout.Dock        = DockStyle.Fill;
            layout.Padding     = new Padding(16);
            layout.RowCount    = 9;
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (TipoUsuario t in Enum.GetValues(typeof(TipoUsuario)))
                cmbTipo.Items.Add(t);
            cmbTipo.SelectedIndex = 0;

            chkActivo.Text    = "Usuario activo";
            chkActivo.Checked = true;

            AgregarFila(layout, 0, "Nombre *",  txtNombre);
            AgregarFila(layout, 1, "Apellido",  txtApellido);
            AgregarFila(layout, 2, "Carnet",    txtCarnet);
            AgregarFila(layout, 3, "Email",     txtEmail);
            AgregarFila(layout, 4, "Telefono",  txtTelefono);
            AgregarFila(layout, 5, "Tipo",      cmbTipo);
            AgregarFila(layout, 6, "Estado",    chkActivo);

            var pnl = new FlowLayoutPanel();
            pnl.Dock          = DockStyle.Fill;
            pnl.FlowDirection = FlowDirection.RightToLeft;
            pnl.Padding       = new Padding(0, 4, 0, 0);

            var btnCancelar = CrearBoton("Cancelar", Color.FromArgb(100, 116, 139));
            var btnGuardar  = CrearBoton("Guardar",  Color.FromArgb(30, 58, 138));
            btnCancelar.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            btnGuardar.Click  += Guardar;

            pnl.Controls.Add(btnCancelar);
            pnl.Controls.Add(btnGuardar);
            layout.SetColumnSpan(pnl, 2);
            layout.Controls.Add(pnl, 0, 8);

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

        private void Rellenar(Usuario u)
        {
            txtNombre.Text    = u.Nombre;
            txtApellido.Text  = u.Apellido;
            txtCarnet.Text    = u.Carnet;
            txtEmail.Text     = u.Email;
            txtTelefono.Text  = u.Telefono;
            cmbTipo.SelectedItem = u.Tipo;
            chkActivo.Checked    = u.Activo;
        }

        private void Guardar(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Error");
                return;
            }
            try
            {
                if (_existente == null)
                {
                    var nuevo = new Usuario();
                    nuevo.Nombre   = txtNombre.Text.Trim();
                    nuevo.Apellido = txtApellido.Text.Trim();
                    nuevo.Carnet   = txtCarnet.Text.Trim();
                    nuevo.Email    = txtEmail.Text.Trim();
                    nuevo.Telefono = txtTelefono.Text.Trim();
                    nuevo.Tipo     = (TipoUsuario)cmbTipo.SelectedItem;
                    nuevo.Activo   = chkActivo.Checked;
                    _servicio.Agregar(nuevo);
                }
                else
                {
                    _existente.Nombre   = txtNombre.Text.Trim();
                    _existente.Apellido = txtApellido.Text.Trim();
                    _existente.Carnet   = txtCarnet.Text.Trim();
                    _existente.Email    = txtEmail.Text.Trim();
                    _existente.Telefono = txtTelefono.Text.Trim();
                    _existente.Tipo     = (TipoUsuario)cmbTipo.SelectedItem;
                    _existente.Activo   = chkActivo.Checked;
                    _servicio.Actualizar(_existente);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
        }
    }
}
