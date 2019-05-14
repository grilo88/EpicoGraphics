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

namespace RotateString
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            angle++;
            if (angle > 360) angle = 1;

            Refresh();
        }

        int angle = 0;
        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Font font = new Font("Microsoft Sans Serif", 10);
            string text = "Hello World!!!";
            SizeF size = g.MeasureString(text, font);

            int CenterX = Width / 2;
            int CenterY = Height / 2;
            int width = 200;
            int height = 200;

            g.TranslateTransform(CenterX, CenterY);
            g.RotateTransform(angle);

            PointF drawPoint = new PointF(-(size.Width / 2), -(size.Height /2)); // Ponto no centro do texto
            g.DrawString(text, font, Brushes.Black, drawPoint);
            g.ResetTransform();

            g.DrawEllipse(new Pen(new SolidBrush(Color.Blue), 1), new Rectangle(CenterX - width / 2, CenterY - height / 2, width, height));
            g.DrawEllipse(new Pen(new SolidBrush(Color.Blue), 1), new Rectangle(CenterX-3, CenterY-3, 6, 6));
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}
