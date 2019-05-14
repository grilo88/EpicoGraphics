namespace PonteiroGravidade
{
    partial class frmMain
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
            this.lblAngulo = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.lblVelocidade = new System.Windows.Forms.Label();
            this.chkLento = new System.Windows.Forms.CheckBox();
            this.lblAnguloMouse = new System.Windows.Forms.Label();
            this.chkPerseguirMouse = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // lblAngulo
            // 
            this.lblAngulo.AutoSize = true;
            this.lblAngulo.Location = new System.Drawing.Point(12, 9);
            this.lblAngulo.Name = "lblAngulo";
            this.lblAngulo.Size = new System.Drawing.Size(43, 13);
            this.lblAngulo.TabIndex = 0;
            this.lblAngulo.Text = "Ângulo:";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1;
            this.timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // lblVelocidade
            // 
            this.lblVelocidade.AutoSize = true;
            this.lblVelocidade.Location = new System.Drawing.Point(162, 9);
            this.lblVelocidade.Name = "lblVelocidade";
            this.lblVelocidade.Size = new System.Drawing.Size(63, 13);
            this.lblVelocidade.TabIndex = 1;
            this.lblVelocidade.Text = "Velocidade:";
            // 
            // chkLento
            // 
            this.chkLento.AutoSize = true;
            this.chkLento.Location = new System.Drawing.Point(472, 12);
            this.chkLento.Name = "chkLento";
            this.chkLento.Size = new System.Drawing.Size(53, 17);
            this.chkLento.TabIndex = 2;
            this.chkLento.Text = "Lento";
            this.chkLento.UseVisualStyleBackColor = true;
            this.chkLento.CheckedChanged += new System.EventHandler(this.ChkLento_CheckedChanged);
            // 
            // lblAnguloMouse
            // 
            this.lblAnguloMouse.AutoSize = true;
            this.lblAnguloMouse.Location = new System.Drawing.Point(12, 30);
            this.lblAnguloMouse.Name = "lblAnguloMouse";
            this.lblAnguloMouse.Size = new System.Drawing.Size(43, 13);
            this.lblAnguloMouse.TabIndex = 3;
            this.lblAnguloMouse.Text = "Ângulo:";
            // 
            // chkPerseguirMouse
            // 
            this.chkPerseguirMouse.AutoSize = true;
            this.chkPerseguirMouse.Location = new System.Drawing.Point(472, 35);
            this.chkPerseguirMouse.Name = "chkPerseguirMouse";
            this.chkPerseguirMouse.Size = new System.Drawing.Size(105, 17);
            this.chkPerseguirMouse.TabIndex = 4;
            this.chkPerseguirMouse.Text = "Perseguir Mouse";
            this.chkPerseguirMouse.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkPerseguirMouse);
            this.Controls.Add(this.lblAnguloMouse);
            this.Controls.Add(this.chkLento);
            this.Controls.Add(this.lblVelocidade);
            this.Controls.Add(this.lblAngulo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblAngulo;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label lblVelocidade;
        private System.Windows.Forms.CheckBox chkLento;
        private System.Windows.Forms.Label lblAnguloMouse;
        private System.Windows.Forms.CheckBox chkPerseguirMouse;
    }
}

