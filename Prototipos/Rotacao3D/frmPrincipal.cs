using Epico.Sistema3D;
using Epico;

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
using Epico.Sistema2D;

namespace Rotacao3D
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        Vetor3D cameraPos = new Vetor3D();
        List<Vertice3D> Vertices = new List<Vertice3D>();
        Vetor2D pontoTela = new Vetor2D();

        private Vetor3D Centro()
        {
            float TotalX = Vertices.Sum(x => x.X);
            float TotalY = Vertices.Sum(x => x.Y);
            float TotalZ = Vertices.Sum(x => x.Z);

            return new Vetor3D(TotalX / Vertices.Count(), TotalY / Vertices.Count(), TotalZ / Vertices.Count());
        }


        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            // Parte Inferior
            Vertices.Add(new Vertice3D(0, 0, 0));
            Vertices.Add(new Vertice3D(0, 50, 0));
            Vertices.Add(new Vertice3D(50, 50, 0));
            Vertices.Add(new Vertice3D(50, 0, 0));

            // Parte Superior
            Vertices.Add(new Vertice3D(0, 0, 50));
            Vertices.Add(new Vertice3D(0, 50, 50));
            Vertices.Add(new Vertice3D(50, 50, 50));
            Vertices.Add(new Vertice3D(50, 0, 50));
        }

        private void FrmPrincipal_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // O fator de zoom é definido com a largura do monitor para impedir que o cubo seja distorcido
            float zoom = (float)(Screen.PrimaryScreen.Bounds.Width / 1.5);

            //Calcule a posição da câmera Z para permanecer constante apesar da rotação            
            Vetor3D pontoAncora = new Vetor3D(Vertices[4]); //anchor point
            float cameraZ = -(((pontoAncora.X - Centro().X) * zoom) / Centro().X) + pontoAncora.Z;
            cameraPos = new Vetor3D(Centro().X, Centro().Y, cameraZ);

            g = Graphics.FromImage(tmpBmp);

            // Converter pontos 3D para 2D
            Vetor3D vec;
            for (int i = 0; i < Vertices.Count(); i++)
            {
                vec = new Vetor3D(Vertices[i]);
                if (vec.Z - cameraPos.Z >= 0)
                {
                    point3D[i].X = (int)(-(vec.X - cameraPos.X) / (-0.1f) * zoom) + pontoTela.X;
                    point3D[i].Y = (int)((vec.Y - cameraPos.Y) / (-0.1f) * zoom) + pontoTela.Y;
                }
                else
                {
                    tmpOrigin.X = (int)((Centro().X - cameraPos.X) / (double)(Centro().Z - cameraPos.Z) * zoom) + pontoTela.X;
                    tmpOrigin.Y = (int)(-(Centro().Y - cameraPos.Y) / (double)(Centro().Z - cameraPos.Z) * zoom) + pontoTela.Y;

                    point3D[i].X = ((vec.X - cameraPos.X) / (vec.Z - cameraPos.Z) * zoom + Centro().X);
                    point3D[i].Y = (-(vec.Y - cameraPos.Y) / (vec.Z - cameraPos.Z) * zoom + Centro().Y);

                    point3D[i].X = (int)point3D[i].X;
                    point3D[i].Y = (int)point3D[i].Y;
                }
            }

            for (int i = 1; i < Vertices.Count(); i++)
            {
                PointF pontoA = new PointF(Vertices[i].X, Vertices[i].Y);
                PointF pontoB = new PointF(Vertices[i].X, Vertices[i].Y);

                g.DrawLine(new Pen(new SolidBrush(Color.Black)), pontoA, pontoB);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
