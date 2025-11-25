namespace GestionContenedores
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panelMenu = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnEstadistica = new System.Windows.Forms.Button();
            this.btnRutaTrabajador = new System.Windows.Forms.Button();
            this.btnCambiarEstado = new System.Windows.Forms.Button();
            this.panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(55)))), ((int)(((byte)(75)))));
            this.panelMenu.Controls.Add(this.pictureBox1);
            this.panelMenu.Controls.Add(this.btnEstadistica);
            this.panelMenu.Controls.Add(this.btnRutaTrabajador);
            this.panelMenu.Controls.Add(this.btnCambiarEstado);
            this.panelMenu.Controls.Add(this.label1);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(250, 681);
            this.panelMenu.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = " Gestor de Contenedores";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(250, 0);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(1014, 681);
            this.panelContenedor.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 537);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(250, 144);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // btnEstadistica
            // 
            this.btnEstadistica.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEstadistica.FlatAppearance.BorderSize = 0;
            this.btnEstadistica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstadistica.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstadistica.ForeColor = System.Drawing.Color.White;
            this.btnEstadistica.Image = ((System.Drawing.Image)(resources.GetObject("btnEstadistica.Image")));
            this.btnEstadistica.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEstadistica.Location = new System.Drawing.Point(0, 230);
            this.btnEstadistica.Name = "btnEstadistica";
            this.btnEstadistica.Size = new System.Drawing.Size(250, 80);
            this.btnEstadistica.TabIndex = 3;
            this.btnEstadistica.Text = "Estadistica";
            this.btnEstadistica.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEstadistica.UseVisualStyleBackColor = true;
            this.btnEstadistica.Click += new System.EventHandler(this.btnEstadistica_Click);
            // 
            // btnRutaTrabajador
            // 
            this.btnRutaTrabajador.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRutaTrabajador.FlatAppearance.BorderSize = 0;
            this.btnRutaTrabajador.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRutaTrabajador.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRutaTrabajador.ForeColor = System.Drawing.Color.White;
            this.btnRutaTrabajador.Image = ((System.Drawing.Image)(resources.GetObject("btnRutaTrabajador.Image")));
            this.btnRutaTrabajador.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRutaTrabajador.Location = new System.Drawing.Point(0, 150);
            this.btnRutaTrabajador.Name = "btnRutaTrabajador";
            this.btnRutaTrabajador.Size = new System.Drawing.Size(250, 80);
            this.btnRutaTrabajador.TabIndex = 2;
            this.btnRutaTrabajador.Text = "Ruta Recolección";
            this.btnRutaTrabajador.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRutaTrabajador.UseVisualStyleBackColor = true;
            this.btnRutaTrabajador.Click += new System.EventHandler(this.btnRutaTrabajador_Click);
            // 
            // btnCambiarEstado
            // 
            this.btnCambiarEstado.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCambiarEstado.FlatAppearance.BorderSize = 0;
            this.btnCambiarEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCambiarEstado.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarEstado.ForeColor = System.Drawing.Color.White;
            this.btnCambiarEstado.Image = ((System.Drawing.Image)(resources.GetObject("btnCambiarEstado.Image")));
            this.btnCambiarEstado.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCambiarEstado.Location = new System.Drawing.Point(0, 70);
            this.btnCambiarEstado.Name = "btnCambiarEstado";
            this.btnCambiarEstado.Size = new System.Drawing.Size(250, 80);
            this.btnCambiarEstado.TabIndex = 1;
            this.btnCambiarEstado.Text = "Home";
            this.btnCambiarEstado.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCambiarEstado.UseVisualStyleBackColor = true;
            this.btnCambiarEstado.Click += new System.EventHandler(this.btnCambiarEstado_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.Controls.Add(this.panelContenedor);
            this.Controls.Add(this.panelMenu);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button btnCambiarEstado;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.Button btnEstadistica;
        private System.Windows.Forms.Button btnRutaTrabajador;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}