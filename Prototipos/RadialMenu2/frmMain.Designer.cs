namespace RadialMenu2
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
            this.pnPedacoRadial = new System.Windows.Forms.Panel();
            this.pnNucleo = new System.Windows.Forms.Panel();
            this.tmrMouse = new System.Windows.Forms.Timer(this.components);
            this.lblMousePosTela = new System.Windows.Forms.Label();
            this.picTeste = new System.Windows.Forms.PictureBox();
            this.lblDistancia = new System.Windows.Forms.Label();
            this.lblMousePosForm = new System.Windows.Forms.Label();
            this.lblMousePosObjeto = new System.Windows.Forms.Label();
            this.lblAngulo = new System.Windows.Forms.Label();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picTeste)).BeginInit();
            this.SuspendLayout();
            // 
            // pnPedacoRadial
            // 
            this.pnPedacoRadial.BackColor = System.Drawing.Color.Transparent;
            this.pnPedacoRadial.Location = new System.Drawing.Point(201, 96);
            this.pnPedacoRadial.Name = "pnPedacoRadial";
            this.pnPedacoRadial.Size = new System.Drawing.Size(174, 164);
            this.pnPedacoRadial.TabIndex = 0;
            this.pnPedacoRadial.Paint += new System.Windows.Forms.PaintEventHandler(this.PnPedacoRadial_Paint);
            // 
            // pnNucleo
            // 
            this.pnNucleo.Location = new System.Drawing.Point(433, 169);
            this.pnNucleo.Name = "pnNucleo";
            this.pnNucleo.Size = new System.Drawing.Size(271, 226);
            this.pnNucleo.TabIndex = 1;
            this.pnNucleo.Paint += new System.Windows.Forms.PaintEventHandler(this.PnNucleo_Paint);
            // 
            // tmrMouse
            // 
            this.tmrMouse.Enabled = true;
            this.tmrMouse.Interval = 1;
            this.tmrMouse.Tick += new System.EventHandler(this.TmrMouse_Tick);
            // 
            // lblMousePosTela
            // 
            this.lblMousePosTela.AutoSize = true;
            this.lblMousePosTela.Location = new System.Drawing.Point(12, 9);
            this.lblMousePosTela.Name = "lblMousePosTela";
            this.lblMousePosTela.Size = new System.Drawing.Size(84, 13);
            this.lblMousePosTela.TabIndex = 2;
            this.lblMousePosTela.Text = "MousePos Tela:";
            // 
            // picTeste
            // 
            this.picTeste.BackColor = System.Drawing.Color.White;
            this.picTeste.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTeste.Location = new System.Drawing.Point(12, 141);
            this.picTeste.Name = "picTeste";
            this.picTeste.Size = new System.Drawing.Size(113, 98);
            this.picTeste.TabIndex = 3;
            this.picTeste.TabStop = false;
            this.picTeste.Paint += new System.Windows.Forms.PaintEventHandler(this.PicTeste_Paint);
            // 
            // lblDistancia
            // 
            this.lblDistancia.AutoSize = true;
            this.lblDistancia.Location = new System.Drawing.Point(581, 9);
            this.lblDistancia.Name = "lblDistancia";
            this.lblDistancia.Size = new System.Drawing.Size(54, 13);
            this.lblDistancia.TabIndex = 4;
            this.lblDistancia.Text = "Distância:";
            // 
            // lblMousePosForm
            // 
            this.lblMousePosForm.AutoSize = true;
            this.lblMousePosForm.Location = new System.Drawing.Point(207, 9);
            this.lblMousePosForm.Name = "lblMousePosForm";
            this.lblMousePosForm.Size = new System.Drawing.Size(86, 13);
            this.lblMousePosForm.TabIndex = 5;
            this.lblMousePosForm.Text = "MousePos Form:";
            // 
            // lblMousePosObjeto
            // 
            this.lblMousePosObjeto.AutoSize = true;
            this.lblMousePosObjeto.Location = new System.Drawing.Point(374, 9);
            this.lblMousePosObjeto.Name = "lblMousePosObjeto";
            this.lblMousePosObjeto.Size = new System.Drawing.Size(94, 13);
            this.lblMousePosObjeto.TabIndex = 6;
            this.lblMousePosObjeto.Text = "MousePos Objeto:";
            // 
            // lblAngulo
            // 
            this.lblAngulo.AutoSize = true;
            this.lblAngulo.Location = new System.Drawing.Point(581, 26);
            this.lblAngulo.Name = "lblAngulo";
            this.lblAngulo.Size = new System.Drawing.Size(43, 13);
            this.lblAngulo.TabIndex = 7;
            this.lblAngulo.Text = "Ângulo:";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(12, 26);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(58, 17);
            this.chkDebug.TabIndex = 8;
            this.chkDebug.Text = "Debug";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(737, 450);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.lblAngulo);
            this.Controls.Add(this.lblMousePosObjeto);
            this.Controls.Add(this.lblMousePosForm);
            this.Controls.Add(this.lblDistancia);
            this.Controls.Add(this.picTeste);
            this.Controls.Add(this.lblMousePosTela);
            this.Controls.Add(this.pnNucleo);
            this.Controls.Add(this.pnPedacoRadial);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FrmMain_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.picTeste)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnPedacoRadial;
        private System.Windows.Forms.Panel pnNucleo;
        private System.Windows.Forms.Timer tmrMouse;
        private System.Windows.Forms.Label lblMousePosTela;
        private System.Windows.Forms.PictureBox picTeste;
        private System.Windows.Forms.Label lblDistancia;
        private System.Windows.Forms.Label lblMousePosForm;
        private System.Windows.Forms.Label lblMousePosObjeto;
        private System.Windows.Forms.Label lblAngulo;
        private System.Windows.Forms.CheckBox chkDebug;
    }
}

