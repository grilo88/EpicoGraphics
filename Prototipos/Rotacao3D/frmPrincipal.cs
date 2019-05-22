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

namespace Rotacao3D
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        struct Vetor3D
        {
            public float X, Y, Z;
            public Vetor3D(float X, float Y, float Z)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
            }
        }

        Vetor3D pontoA = new Vetor3D(0, 0, 0);
        Vetor3D pontoB = new Vetor3D(-50, 0, 0);
        Vetor3D pontoC = new Vetor3D(0, 50, 0);
        Vetor3D pontoD = new Vetor3D(-50, 50, 50);
        Vetor3D pontoE = new Vetor3D(-50, 50, 50);

        private void FrmPrincipal_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;


        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        Vetor3D RotateX(Vetor3D point3D, double degrees)
        {
            //Here we use Euler's matrix formula for rotating a 3D point x degrees around the x-axis

            //[ a  b  c ] [ x ]   [ x*a + y*b + z*c ]
            //[ d  e  f ] [ y ] = [ x*d + y*e + z*f ]
            //[ g  h  i ] [ z ]   [ x*g + y*h + z*i ]

            //[ 1    0        0   ]
            //[ 0   cos(x)  sin(x)]
            //[ 0   -sin(x) cos(x)]

            float rad = (float)((Math.PI * degrees) / 180.0f);
            float cosDegrees = (float)Math.Cos(rad);
            float sinDegrees = (float)Math.Sin(rad);

            float y = (point3D.Y * cosDegrees) + (point3D.Z * sinDegrees);
            float z = (point3D.Y * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Vetor3D(point3D.X, y, z);
        }

        Vetor3D RotateY(Vetor3D point3D, double degrees)
        {
            //Y-axis

            //[ cos(x)   0    sin(x)]
            //[   0      1      0   ]
            //[-sin(x)   0    cos(x)]

            float rad = (float)((Math.PI * degrees) / 180.0); //Radians
            float cosDegrees = (float)Math.Cos(rad);
            float sinDegrees = (float)Math.Sin(rad);

            float x = (point3D.X * cosDegrees) + (point3D.Z * sinDegrees);
            float z = (point3D.X * -sinDegrees) + (point3D.Z * cosDegrees);

            return new Vetor3D(x, point3D.Y, z);
        }

        Vetor3D RotateZ(Vetor3D point3D, double degrees)
        {
            //Z-axis

            //[ cos(x)  sin(x) 0]
            //[ -sin(x) cos(x) 0]
            //[    0     0     1]

            float rad = (float)((Math.PI * degrees) / 180.0); //Radians
            float cosDegrees = (float)Math.Cos(rad);
            float sinDegrees = (float)Math.Sin(rad);

            float x = (point3D.X * cosDegrees) + (point3D.Y * sinDegrees);
            float y = (point3D.X * -sinDegrees) + (point3D.Y * cosDegrees);

            return new Vetor3D(x, y, point3D.Z);
        }
    }
}
