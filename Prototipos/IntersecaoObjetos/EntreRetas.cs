using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntersecaoObjetos
{
    public partial class EntreRetas : Form
    {
        public EntreRetas()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        float CentroX = 80;
        float CentroY = 80;

        PointF a = new PointF();
        PointF b = new PointF();
        PointF c = new PointF();
        PointF d = new PointF();


        float rad = 0F;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            rad += 0.05F;

            a.X = CentroX + 30;
            a.Y = CentroX + 50 + (float)Math.Sin(rad) * 50;
            b.X = CentroX + 200;
            b.Y = CentroY + 200;

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 3), a, b);

            c = PointToClient(Cursor.Position);
            d = new PointF(c.X + 80, c.Y - 50);

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 3), c, d);
            label1.Text = $"MousePos: {c.X},{c.Y}";

            bool colisao = IntersecaoEntreDuasRetas(a, b, c, d, out PointF intersecao, out bool linhas_intersecao, out PointF aa, out PointF bb);

            if (colisao)
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new PointF(intersecao.X - 4, intersecao.Y - 4), new SizeF(8, 8)));
                label2.Text = "Colisão!";
            }
            else
            {
                label2.Text = string.Empty;
                if (linhas_intersecao)
                {
                    e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green)), aa, bb);
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }


        /// <summary>
        /// Detecta interseção entre dois segmentos de retas
        /// </summary>
        /// <param name="a">Ponto A do segmento 1</param>
        /// <param name="b">Ponto B do segmento 1</param>
        /// <param name="c">Ponto A do segmento 2</param>
        /// <param name="d">Ponto B do segmento 2</param>
        /// <param name="intersecao">Ponto de interseção entre as retas</param>
        /// <param name="linhas_intersecao">Linha de interseção</param>
        /// <param name="aa">Ponto A da linha de interseç+ão</param>
        /// <param name="bb">Ponto B da linha de interseção</param>
        /// <returns></returns>
        private bool IntersecaoEntreDuasRetas(PointF a, PointF b, PointF c, PointF d,
            out PointF intersecao, out bool linhas_intersecao, out PointF aa, out PointF bb)
        {
            bool colisao = false;

            // Obtém os parâmetros dos segmentos
            float dxAB = b.X - a.X;
            float dyAB = b.Y - a.Y;
            float dxCD = d.X - c.X;
            float dyCD = d.Y - c.Y;

            // Resolve para t1 e t2
            float denominador = (dyAB * dxCD - dxAB * dyCD);

            float t1 =
                ((a.X - c.X) * dyCD + (c.Y - a.Y) * dxCD) / denominador;
            if (float.IsInfinity(t1))
            {
                // As linhas são paralelas (ou próximas o suficiente)
                linhas_intersecao = false;
                intersecao = new PointF(float.NaN, float.NaN);
                aa = new PointF(float.NaN, float.NaN);
                bb = new PointF(float.NaN, float.NaN);
                return colisao;
            }
            linhas_intersecao = true;

            float t2 = ((c.X - a.X) * dyAB + (a.Y - c.Y) * dxAB) / -denominador;

            // Ponto de interseção
            intersecao = new PointF(a.X + dxAB * t1, a.Y + dyAB * t1);

            // Os segmentos se cruzam se t1 e t2 estiverem entre 0 e 1
            colisao =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Encontre os pontos mais próximos nos segmentos
            if (t1 < 0) t1 = 0; else if (t1 > 1) t1 = 1;
            if (t2 < 0) t2 = 0; else if (t2 > 1) t2 = 1;

            aa = new PointF(a.X + dxAB * t1, a.Y + dyAB * t1);
            bb = new PointF(c.X + dxCD * t2, c.Y + dyCD * t2);

            return colisao;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            RetaRetangulo frm = new RetaRetangulo();
            frm.Show();
        }
    }
}
