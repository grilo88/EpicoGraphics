using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PonteiroGravidade
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        float angulo = 45;
        float compPonteiro = 200;
        float velocidade;

        float centerX;
        float centerY;

        float x2, y2;
        float fatorDesaceleracao = 0.5F;
        float fatorVelocidade = 0.1F;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            #region Gravidade no Fator Ângulo
            if (angulo >= 0)
            {
                velocidade -= fatorVelocidade;
                angulo += -velocidade;
                if (angulo > 179)
                {
                    // Atingiu o lado esquerdo
                    angulo = -180;
                    velocidade += fatorDesaceleracao;
                }
            }
            if (angulo < 0)
            {
                velocidade += fatorVelocidade;
                angulo += -velocidade;
                if (angulo < -179)
                {
                    // Atingiu o lado direito
                    angulo = 180;
                    velocidade += -fatorDesaceleracao;
                }
            }
            #endregion

            CalcPonteiro();

            Refresh();
            timer1.Start();
        }

        private void CalcPonteiro(float diff = 0F)
        {
            float rad = Angulo2Radiano(-angulo + 180 - diff);

            float local_x2 = (float)Math.Sin(rad) * compPonteiro;
            float local_y2 = (float)Math.Cos(rad) * compPonteiro;

            x2 = centerX + local_x2;
            y2 = centerY + local_y2;
        }

        private float Angulo2Radiano(float angulo)
        {
            return angulo * (float)Math.PI / 180;
        }

        private float AnguloEntreDoisPontos(float x1, float y1, float x2, float y2)
        {
            return (float)(Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI);
        }

        private float DistanciaEntreDoisPontos(int x1, int y1, int x2, int y2)
        {
            return (float)(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
        }

        private float PorCento(float valor, float max)
        {
            return (valor / 100 * max);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            lblAngulo.Text = "Ângulo: " + angulo;
            lblVelocidade.Text = "Velocidade: " + velocidade;
        }

        private void ChkLento_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLento.Checked)
                timer1.Interval = 100;
            else
                timer1.Interval = 10;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            centerX = (ClientRectangle.Width / 2);
            centerY = (ClientRectangle.Height / 2) - 50;

            ChkLento_CheckedChanged(sender, e);
        }

        float aceleracaoMouse = 0F;
        int mouseDownX, mouseDownY;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                #region Cáculo de aceleração orientada pela distância entre os pontos mas o correto deve ser orientado entre as distâncias de ângulos do objeto
                aceleracaoMouse = DistanciaEntreDoisPontos(
                    mouseDownX, mouseDownY, e.X, e.Y);

                // Identifica o sentido da aceleração (pra cima ou pra baixo)
                if (mouseDownX - e.X > 0) aceleracaoMouse = -aceleracaoMouse;
                
                mouseDownX = e.X;
                mouseDownY = e.Y;
                #endregion

                float angulo = AnguloEntreDoisPontos(centerX, centerY, e.X, e.Y);
                this.angulo = angulo + 90;
                CalcPonteiro();
                Refresh();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                timer1.Stop();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (angulo >= 0)
                    velocidade = aceleracaoMouse / 5;
                else if (angulo < 0)
                    velocidade = -(aceleracaoMouse / 5);

                timer1.Start();
            }
        }

        /// <summary>
        /// Renderização
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            #region Ponteiro
            // Linha Vermelha do meio
            g.DrawLine(new Pen(new SolidBrush(Color.Red), 3), 
                new Point((int)this.centerX, (int)this.centerY), new Point((int)this.x2, (int)this.y2));

            // Comprimento das linhas laterais do ponteiro
            float left_x1, left_y1;
            float right_x1, right_y1;

            int distPonta = 80; // Percentual de 0 a 100%

            // Linha Lateral Esquerda
            float rad = Angulo2Radiano(-angulo + 180 - 5);
            float local_x2 = (float)Math.Sin(rad) * PorCento(distPonta, compPonteiro);
            float local_y2 = (float)Math.Cos(rad) * PorCento(distPonta, compPonteiro);

            float x2a = left_x1 = centerX + local_x2;
            float y2a = left_y1 = centerY + local_y2;

            g.DrawLine(new Pen(new SolidBrush(Color.Black), 3),
                new Point((int)centerX, (int)centerY), new Point((int)x2a, (int)y2a));

            // Linha Lateral Direita
            rad = Angulo2Radiano(-angulo + 180 + 5);
            local_x2 = (float)Math.Sin(rad) * PorCento(distPonta, compPonteiro);
            local_y2 = (float)Math.Cos(rad) * PorCento(distPonta, compPonteiro);

            float x2b = right_x1 = centerX + local_x2;
            float y2b = right_y1 = centerY + local_y2;

            g.DrawLine(new Pen(new SolidBrush(Color.Black), 3),
                new Point((int)centerX, (int)centerY), new Point((int)x2b, (int)y2b));

            // Ponta do ponteiro
            g.DrawLine(new Pen(new SolidBrush(Color.Blue), 3),
                new Point((int)(left_x1), (int)(left_y1)), new Point((int)this.x2, (int)this.y2));
            g.DrawLine(new Pen(new SolidBrush(Color.Blue), 3),
                new Point((int)(right_x1), (int)(right_y1)), new Point((int)this.x2, (int)this.y2));
            #endregion

            #region Costas do ponteiro
            // Linha Lateral Esquerda
            rad = Angulo2Radiano(-angulo + 180 + 50);
            local_x2 = (float)Math.Sin(rad) * (-30);
            local_y2 = (float)Math.Cos(rad) * (-30);

            x2a = centerX + local_x2;
            y2a = centerY + local_y2;

            g.DrawLine(new Pen(new SolidBrush(Color.Black), 3),
                new Point((int)centerX, (int)centerY), new Point((int)x2a, (int)y2a));

            // Linha Lateral Direita
            rad = Angulo2Radiano(-angulo + 180 - 50);
            local_x2 = (float)Math.Sin(rad) * (-30);
            local_y2 = (float)Math.Cos(rad) * (-30);

            x2b = centerX + local_x2;
            y2b = centerY + local_y2;

            g.DrawLine(new Pen(new SolidBrush(Color.Black), 3),
                new Point((int)centerX, (int)centerY), new Point((int)x2b, (int)y2b));

            // Linha que conecta as parte das costas
            g.DrawLine(new Pen(new SolidBrush(Color.Black), 3), 
                new Point((int)x2a, (int)y2a), new Point((int)x2b, (int)y2b));
            #endregion
        }
    }
}
