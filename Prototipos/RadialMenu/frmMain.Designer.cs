namespace RadialMenu
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackAngulo = new System.Windows.Forms.TrackBar();
            this.trackQuant = new System.Windows.Forms.TrackBar();
            this.trackTamExt = new System.Windows.Forms.TrackBar();
            this.trackTamInt = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioOrientacaoLegendaFixa = new System.Windows.Forms.RadioButton();
            this.radioOrientacaoLegendaGravidade = new System.Windows.Forms.RadioButton();
            this.tmrEfeito = new System.Windows.Forms.Timer(this.components);
            this.btnEfeito = new System.Windows.Forms.Button();
            this.radioOrientacaoLegendaDisco = new System.Windows.Forms.RadioButton();
            this.radioOrientacaoLegendaGravidadeInvertido = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.trackAngulo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackQuant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTamExt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTamInt)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Quantidade:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tamanho Externo:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tamanho Interno:";
            // 
            // trackAngulo
            // 
            this.trackAngulo.Location = new System.Drawing.Point(191, 12);
            this.trackAngulo.Maximum = 360;
            this.trackAngulo.Minimum = 1;
            this.trackAngulo.Name = "trackAngulo";
            this.trackAngulo.Size = new System.Drawing.Size(597, 45);
            this.trackAngulo.TabIndex = 7;
            this.trackAngulo.Value = 100;
            this.trackAngulo.Scroll += new System.EventHandler(this.TrackAngulo_Scroll);
            // 
            // trackQuant
            // 
            this.trackQuant.Location = new System.Drawing.Point(12, 66);
            this.trackQuant.Maximum = 30;
            this.trackQuant.Name = "trackQuant";
            this.trackQuant.Size = new System.Drawing.Size(147, 45);
            this.trackQuant.TabIndex = 8;
            this.trackQuant.Value = 6;
            this.trackQuant.Scroll += new System.EventHandler(this.TrackQuant_Scroll);
            // 
            // trackTamExt
            // 
            this.trackTamExt.Location = new System.Drawing.Point(12, 130);
            this.trackTamExt.Maximum = 400;
            this.trackTamExt.Name = "trackTamExt";
            this.trackTamExt.Size = new System.Drawing.Size(147, 45);
            this.trackTamExt.TabIndex = 9;
            this.trackTamExt.Value = 300;
            this.trackTamExt.Scroll += new System.EventHandler(this.TrackTamExt_Scroll);
            // 
            // trackTamInt
            // 
            this.trackTamInt.Location = new System.Drawing.Point(15, 194);
            this.trackTamInt.Maximum = 300;
            this.trackTamInt.Name = "trackTamInt";
            this.trackTamInt.Size = new System.Drawing.Size(144, 45);
            this.trackTamInt.TabIndex = 10;
            this.trackTamInt.Value = 110;
            this.trackTamInt.Scroll += new System.EventHandler(this.TrackTamInt_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioOrientacaoLegendaGravidadeInvertido);
            this.groupBox1.Controls.Add(this.radioOrientacaoLegendaDisco);
            this.groupBox1.Controls.Add(this.radioOrientacaoLegendaGravidade);
            this.groupBox1.Controls.Add(this.radioOrientacaoLegendaFixa);
            this.groupBox1.Location = new System.Drawing.Point(12, 259);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 116);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orientação da Legenda:";
            // 
            // radioOrientacaoLegendaFixa
            // 
            this.radioOrientacaoLegendaFixa.AutoSize = true;
            this.radioOrientacaoLegendaFixa.Checked = true;
            this.radioOrientacaoLegendaFixa.Location = new System.Drawing.Point(6, 19);
            this.radioOrientacaoLegendaFixa.Name = "radioOrientacaoLegendaFixa";
            this.radioOrientacaoLegendaFixa.Size = new System.Drawing.Size(44, 17);
            this.radioOrientacaoLegendaFixa.TabIndex = 13;
            this.radioOrientacaoLegendaFixa.TabStop = true;
            this.radioOrientacaoLegendaFixa.Text = "Fixa";
            this.radioOrientacaoLegendaFixa.UseVisualStyleBackColor = true;
            this.radioOrientacaoLegendaFixa.CheckedChanged += new System.EventHandler(this.RadioOrientacaoLegendaFixa_CheckedChanged);
            // 
            // radioOrientacaoLegendaGravidade
            // 
            this.radioOrientacaoLegendaGravidade.AutoSize = true;
            this.radioOrientacaoLegendaGravidade.Location = new System.Drawing.Point(6, 65);
            this.radioOrientacaoLegendaGravidade.Name = "radioOrientacaoLegendaGravidade";
            this.radioOrientacaoLegendaGravidade.Size = new System.Drawing.Size(74, 17);
            this.radioOrientacaoLegendaGravidade.TabIndex = 15;
            this.radioOrientacaoLegendaGravidade.Text = "Gravidade";
            this.radioOrientacaoLegendaGravidade.UseVisualStyleBackColor = true;
            this.radioOrientacaoLegendaGravidade.CheckedChanged += new System.EventHandler(this.RadioOrientacaoLegendaGravidade_CheckedChanged);
            // 
            // tmrEfeito
            // 
            this.tmrEfeito.Interval = 10;
            this.tmrEfeito.Tick += new System.EventHandler(this.TmrEfeito_Tick);
            // 
            // btnEfeito
            // 
            this.btnEfeito.Location = new System.Drawing.Point(12, 381);
            this.btnEfeito.Name = "btnEfeito";
            this.btnEfeito.Size = new System.Drawing.Size(147, 23);
            this.btnEfeito.TabIndex = 13;
            this.btnEfeito.Text = "Efeito";
            this.btnEfeito.UseVisualStyleBackColor = true;
            this.btnEfeito.Click += new System.EventHandler(this.BtnEfeito_Click);
            // 
            // radioOrientacaoLegendaDisco
            // 
            this.radioOrientacaoLegendaDisco.AutoSize = true;
            this.radioOrientacaoLegendaDisco.Location = new System.Drawing.Point(6, 42);
            this.radioOrientacaoLegendaDisco.Name = "radioOrientacaoLegendaDisco";
            this.radioOrientacaoLegendaDisco.Size = new System.Drawing.Size(52, 17);
            this.radioOrientacaoLegendaDisco.TabIndex = 16;
            this.radioOrientacaoLegendaDisco.Text = "Disco";
            this.radioOrientacaoLegendaDisco.UseVisualStyleBackColor = true;
            this.radioOrientacaoLegendaDisco.CheckedChanged += new System.EventHandler(this.RadioOrientacaoLegendaDisco_CheckedChanged);
            // 
            // radioOrientacaoLegendaGravidadeInvertido
            // 
            this.radioOrientacaoLegendaGravidadeInvertido.AutoSize = true;
            this.radioOrientacaoLegendaGravidadeInvertido.Location = new System.Drawing.Point(6, 88);
            this.radioOrientacaoLegendaGravidadeInvertido.Name = "radioOrientacaoLegendaGravidadeInvertido";
            this.radioOrientacaoLegendaGravidadeInvertido.Size = new System.Drawing.Size(124, 17);
            this.radioOrientacaoLegendaGravidadeInvertido.TabIndex = 17;
            this.radioOrientacaoLegendaGravidadeInvertido.Text = "Gravidade (Invertido)";
            this.radioOrientacaoLegendaGravidadeInvertido.UseVisualStyleBackColor = true;
            this.radioOrientacaoLegendaGravidadeInvertido.CheckedChanged += new System.EventHandler(this.RadioOrientacaoLegendaGravidadeInvertido_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 507);
            this.Controls.Add(this.btnEfeito);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.trackTamInt);
            this.Controls.Add(this.trackTamExt);
            this.Controls.Add(this.trackQuant);
            this.Controls.Add(this.trackAngulo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmMain";
            this.Text = "Menu Radial";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmMain_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmMain_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this.trackAngulo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackQuant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTamExt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackTamInt)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackAngulo;
        private System.Windows.Forms.TrackBar trackQuant;
        private System.Windows.Forms.TrackBar trackTamExt;
        private System.Windows.Forms.TrackBar trackTamInt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioOrientacaoLegendaGravidade;
        private System.Windows.Forms.RadioButton radioOrientacaoLegendaFixa;
        private System.Windows.Forms.Timer tmrEfeito;
        private System.Windows.Forms.Button btnEfeito;
        private System.Windows.Forms.RadioButton radioOrientacaoLegendaDisco;
        private System.Windows.Forms.RadioButton radioOrientacaoLegendaGravidadeInvertido;
    }
}

