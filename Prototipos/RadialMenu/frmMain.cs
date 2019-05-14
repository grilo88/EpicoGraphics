using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadialMenu
{
    public partial class frmMain : Form
    {
        int Angulo = 0;

        public frmMain()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void TrackAngulo_Scroll(object sender, EventArgs e)
        {
            Angulo = trackAngulo.Value;
            Refresh();
        }

        int xCentro = 0;
        int yCentro = 0;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Desenha o círculo externo
            Brush brush = new SolidBrush(Color.Blue);
            int heightExt = Convert.ToInt32(trackTamExt.Value);   // Altura
            int widthExt = Convert.ToInt32(trackTamExt.Value);    // Comprimento

            int xExt = xCentro - widthExt / 2;     // Centraliza na coordenada x
            int yExt = yCentro - heightExt / 2;    // Centraliza na coordenada y
            g.FillEllipse(brush, new Rectangle(xExt, yExt, widthExt, heightExt)); // Desenha o círculo

            // Desenha o círculo interno
            brush = new SolidBrush(Color.White);
            int heightInt = Convert.ToInt32(trackTamInt.Value);    // Altura
            int widthInt = Convert.ToInt32(trackTamInt.Value);     // Comprimento
            int xInt = xCentro - widthInt / 2;       // Centraliza na coordenada x
            int yInt = yCentro - heightInt / 2;    // Centraliza na coordenada y
            g.FillEllipse(brush, new Rectangle(xInt, yInt, widthInt, heightInt)); // Desenha o círculo

            int quantBotoes = Convert.ToInt32(trackQuant.Value);
            float raio_pedaco = (float)(Math.PI * 2) / quantBotoes;

            int AnguloTexto = Angulo;
            if (radioOrientacaoLegendaFixa.Checked)
            {
                 AnguloTexto = 0;
            }
            else if (radioOrientacaoLegendaDisco.Checked)
            {
                AnguloTexto = AnguloTexto + 90;
            }

            Pen linha = new Pen(new SolidBrush(Color.White), 10);
            for (int i = 1; i < quantBotoes + 1; i++)
            {
                float dir = (float)((Math.PI * 2 / 360) * Angulo);

                int x2 = 0, y2 = 0;
                float distExt = CalcularDistancia(xCentro, yCentro, xExt, yExt);
                x2 = (int)(Math.Sin((raio_pedaco * i) + dir) * distExt);
                y2 = (int)(Math.Cos((raio_pedaco * i) + dir) * distExt);

                int x1 = 0, y1 = 0;
                float distInt = CalcularDistancia(xCentro, yCentro, xInt, yInt);
                x1 = (int)(Math.Sin((raio_pedaco * i) + dir) * distInt);
                y1 = (int)(Math.Cos((raio_pedaco * i) + dir) * distInt);

                g.ResetTransform();
                g.DrawLine(linha, new Point(xCentro + x1, yCentro + y1), new Point(xCentro + x2, yCentro + y2));

                Font font = new Font("Microsoft Sans Serif", 10);
                using (StringFormat string_format = new StringFormat())
                {
                    string_format.Alignment = StringAlignment.Center;
                    string_format.LineAlignment = StringAlignment.Center;

                    int xText = (int)(Math.Sin((raio_pedaco * i) + dir + raio_pedaco / 2) * (distExt / 2));
                    int yText = (int)(Math.Cos((raio_pedaco * i) + dir + raio_pedaco / 2) * (distExt / 2));

                    bool negativo = true;
                    if (radioOrientacaoLegendaGravidade.Checked)
                    {
                        float raio = (raio_pedaco * i) + dir + raio_pedaco / 2;
                        AnguloTexto = Convert.ToInt32(raio / (Math.PI * 2) * 360);
                        negativo = true;
                    }
                    else if (radioOrientacaoLegendaGravidadeInvertido.Checked)
                    {
                        float raio = (raio_pedaco * i) + dir + raio_pedaco / 2;
                        AnguloTexto = Convert.ToInt32(raio / (Math.PI * 2) * 360) - 180;
                        negativo = true;
                    }

                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    DrawText(e.Graphics, font, Brushes.White, new Point(xCentro + xText, yCentro + yText), string_format, ObterLegendaMenu(i), AnguloTexto, negativo);
                }
            }
        }

        private float CalcularDistancia(int x1, int y1, int x2, int y2)
        {
            return (float)(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
        }

        private string ObterLegendaMenu(int botao)
        {
            switch (botao)
            {
                case 1:
                    return "Arquivo";
                case 2:
                    return "Editar";
                case 3:
                    return "Exibir";
                case 4:
                    return "Produtos";
                case 5:
                    return "Estoque";
                case 6:
                    return "Notas Fiscais";
                case 7:
                    return "Pessoas";
                case 8:
                    return "Balcão";
                case 9:
                    return "Administração";
                case 10:
                    return "Gerência";
                default:
                    return "Outros " + botao;
            }
        }

        private void DrawText(Graphics gr, Font font,
            Brush brush, Point point, StringFormat string_format, string txt, float angulo, bool negativo)
        {
            // Traduz o movimento do retângulo para a posição correta
            gr.TranslateTransform(point.X, point.Y);

            gr.RotateTransform(negativo ? -angulo : angulo);

            SizeF size = gr.MeasureString(txt, font);
            PointF drawPoint = new PointF(-(size.Width / 2), -(size.Height / 2)); // Ponto fica no centro do texto

            // Desenha o texto
            gr.DrawString(txt, font, brush, drawPoint);
        }

        private void TrackQuant_Scroll(object sender, EventArgs e)
        {
            Refresh();
        }

        private void TrackTamExt_Scroll(object sender, EventArgs e)
        {
            Refresh();
        }

        private void TrackTamInt_Scroll(object sender, EventArgs e)
        {
            Refresh();
        }

        private void BtnEfeito_Click(object sender, EventArgs e)
        {
            tmrEfeito.Start();
        }

        private void TmrEfeito_Tick(object sender, EventArgs e)
        {

        }

        int dragInicioX;
        int dragInicioY;
        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragInicioX = xCentro - e.X;
                dragInicioY = yCentro - e.Y;
            }
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                xCentro = e.X - dragInicioX;
                yCentro = e.Y - dragInicioY;

                Refresh();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            xCentro = Width / 2;
            yCentro = Height / 2;

            TrackAngulo_Scroll(sender, e);
        }

        private void RadioOrientacaoLegendaFixa_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void RadioOrientacaoLegendaDisco_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void RadioOrientacaoLegendaGravidade_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void RadioOrientacaoLegendaGravidadeInvertido_CheckedChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
