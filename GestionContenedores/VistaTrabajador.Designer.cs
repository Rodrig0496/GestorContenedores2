namespace GestionContenedores
{
    partial class VistaTrabajador
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.mapaTrabajador = new GestionContenedores.VistaMapa();
            this.btnGenerarRuta = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGenerarRuta);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 60);
            this.panel1.TabIndex = 0;
            // 
            // mapaTrabajador
            // 
            this.mapaTrabajador.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapaTrabajador.Location = new System.Drawing.Point(0, 60);
            this.mapaTrabajador.Name = "mapaTrabajador";
            this.mapaTrabajador.Size = new System.Drawing.Size(150, 90);
            this.mapaTrabajador.TabIndex = 1;
            // 
            // btnGenerarRuta
            // 
            this.btnGenerarRuta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(55)))), ((int)(((byte)(75)))));
            this.btnGenerarRuta.FlatAppearance.BorderSize = 0;
            this.btnGenerarRuta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarRuta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarRuta.ForeColor = System.Drawing.Color.White;
            this.btnGenerarRuta.Location = new System.Drawing.Point(3, 3);
            this.btnGenerarRuta.Name = "btnGenerarRuta";
            this.btnGenerarRuta.Size = new System.Drawing.Size(144, 51);
            this.btnGenerarRuta.TabIndex = 0;
            this.btnGenerarRuta.Text = "GENERAR RUTA OPTIMIZADA";
            this.btnGenerarRuta.UseVisualStyleBackColor = false;
            this.btnGenerarRuta.Click += new System.EventHandler(this.btnGenerarRuta_Click);
            // 
            // VistaTrabajador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapaTrabajador);
            this.Controls.Add(this.panel1);
            this.Name = "VistaTrabajador";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private VistaMapa mapaTrabajador;
        private System.Windows.Forms.Button btnGenerarRuta;
    }
}
