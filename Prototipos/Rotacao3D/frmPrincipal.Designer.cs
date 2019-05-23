namespace Rotacao3D
{
    partial class frmPrincipal
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.NumZ = new System.Windows.Forms.NumericUpDown();
            this.NumX = new System.Windows.Forms.NumericUpDown();
            this.NumY = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumY)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // NumZ
            // 
            this.NumZ.Location = new System.Drawing.Point(398, 64);
            this.NumZ.Name = "NumZ";
            this.NumZ.Size = new System.Drawing.Size(120, 20);
            this.NumZ.TabIndex = 0;
            this.NumZ.ValueChanged += new System.EventHandler(this.NumZ_ValueChanged);
            // 
            // NumX
            // 
            this.NumX.Location = new System.Drawing.Point(398, 12);
            this.NumX.Name = "NumX";
            this.NumX.Size = new System.Drawing.Size(120, 20);
            this.NumX.TabIndex = 1;
            this.NumX.ValueChanged += new System.EventHandler(this.NumX_ValueChanged);
            // 
            // NumY
            // 
            this.NumY.Location = new System.Drawing.Point(398, 38);
            this.NumY.Name = "NumY";
            this.NumY.Size = new System.Drawing.Size(120, 20);
            this.NumY.TabIndex = 2;
            this.NumY.ValueChanged += new System.EventHandler(this.NumY_ValueChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(420, 106);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Auto";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 532);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.NumY);
            this.Controls.Add(this.NumX);
            this.Controls.Add(this.NumZ);
            this.Name = "frmPrincipal";
            this.Text = "Rotação 3D";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmPrincipal_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.NumZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.NumericUpDown NumZ;
        private System.Windows.Forms.NumericUpDown NumX;
        private System.Windows.Forms.NumericUpDown NumY;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

