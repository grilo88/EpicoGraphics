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
    public partial class RetaRetangulo : Form
    {
        public RetaRetangulo()
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

        Rectangle rect = new Rectangle(new Point(), new Size(100, 100));
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            a.X = CentroX + 30;
            a.Y = CentroX + 50;
            b.X = CentroX + 200;
            b.Y = CentroY + 200;

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 3), a, b);

            c = PointToClient(Cursor.Position);

            rect.X = (int)c.X;
            rect.Y = (int)c.Y;

            e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black), 3), rect);
            label1.Text = $"MousePos: {c.X},{c.Y}";

            bool colisao = IntersecaoRetaRetangulo(a, b, rect.X, rect.Y, rect.Right, rect.Bottom);

            if (colisao)
            {
                //e.Graphics.FillEllipse(new SolidBrush(Color.Red), new RectangleF(new PointF(intersecao.X - 4, intersecao.Y - 4), new SizeF(8, 8)));
                label2.Text = "Colisão!";
            }
            else
            {
                label2.Text = string.Empty;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        public bool IntersecaoRetaRetangulo(PointF a, PointF b, float minX, float minY, float maxX, float maxY)
        {
            // Completamente fora
            if ((a.X <= minX && b.X <= minX) || (a.Y <= minY && b.Y <= minY) ||
                (a.X >= maxX && b.X >= maxX) || (a.Y >= maxY && b.Y >= maxY))
                return false;

            float m = (b.Y - a.Y) / (b.X - a.X);

            float y = m * (minX - a.X) + a.Y;
            if (y >= minY && y <= maxY) return true;

            y = m * (maxX - a.X) + a.Y;
            if (y >= minY && y <= maxY) return true;

            float x = (minY - a.Y) / m + a.X;
            if (x >= minX && x <= maxX) return true;

            x = (maxY - a.Y) / m + a.X;
            if (x >= minX && x <= maxX) return true;

            return false;
        }
    }
}
