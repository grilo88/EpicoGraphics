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

        float _angX, _angY, _angZ;

        float AngX { get => _angX; set => NovoAnguloX(value); }
        float AngY { get => _angY; set => NovoAnguloY(value); }
        float AngZ { get => _angZ; set => NovoAnguloZ(value); }

        private Vetor3D Centro()
        {
            float TotalX = Vertices.Sum(x => x.X);
            float TotalY = Vertices.Sum(x => x.Y);
            float TotalZ = Vertices.Sum(x => x.Z);

            return new Vetor3D(TotalX / Vertices.Count(), TotalY / Vertices.Count(), TotalZ / Vertices.Count());
        }


        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            NumZ.Minimum = decimal.MinValue;
            NumZ.Maximum = decimal.MaxValue;

            NumX.Minimum = decimal.MinValue;
            NumX.Maximum = decimal.MaxValue;

            NumY.Minimum = decimal.MinValue;
            NumY.Maximum = decimal.MaxValue;

            // Inferior
            Vertices.Add(new Vertice3D(0, 0, 0));
            Vertices.Add(new Vertice3D(1, 0, 0));
            Vertices.Add(new Vertice3D(1, 1, 0));
            Vertices.Add(new Vertice3D(0, 1, 0));

            // Esquerdo
            Vertices.Add(new Vertice3D(0, 0, 0));
            Vertices.Add(new Vertice3D(0, 1, 0));
            Vertices.Add(new Vertice3D(0, 1, 1));
            Vertices.Add(new Vertice3D(0, 0, 1));

            // Superior
            Vertices.Add(new Vertice3D(0, 0, 1));
            Vertices.Add(new Vertice3D(1, 0, 1));
            Vertices.Add(new Vertice3D(1, 1, 1));
            Vertices.Add(new Vertice3D(0, 1, 1));

            // Aplica o tamanho no objeto 3D
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].X *= 300;
                Vertices[i].Y *= 300;
                Vertices[i].Z *= 300;
            }

            AngX = 30;
            AngY = 30;
            AngZ = 30;

            Rotacionar(
                (float)(NumX.Value = (decimal)AngX),
                (float)(NumY.Value = (decimal)AngY),
                (float)(NumZ.Value = (decimal)AngZ));
        }

        Vetor2D pontoTela = new Vetor2D();
        private void FrmPrincipal_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            pontoTela.X = ClientSize.Width / 2;
            pontoTela.Y = ClientSize.Height / 2;

            // O fator de zoom é definido com a largura do monitor para impedir que o cubo seja distorcido
            float zoom = (float)(Screen.PrimaryScreen.Bounds.Width / 1.5);

            // Calcule a posição da câmera Z para permanecer constante apesar da rotação            
            Vetor3D pontoAncora = new Vetor3D(Vertices[4]); //anchor point
            float cameraZ = -(((pontoAncora.X - Centro().X) * zoom) / Centro().X) + pontoAncora.Z;
            cameraPos = new Vetor3D(Centro().X, Centro().Y, cameraZ);

            // 
            Vetor2D[] tela = Vertices.Select(x => new Vetor2D(0, 0)).ToArray();

            // Converte pontos 3D em 2D
            Vetor2D origem = new Vetor2D(0, 0);
            Vetor3D vec;
            for (int i = 0; i < Vertices.Count(); i++)
            {
                vec = new Vetor3D(Vertices[i]);
                if (vec.Z - cameraPos.Z >= 0)
                {
                    tela[i].X = (int)(-(vec.X - cameraPos.X) / (-0.1f) * zoom) + pontoTela.X;
                    tela[i].Y = (int)((vec.Y - cameraPos.Y) / (-0.1f) * zoom) + pontoTela.Y;
                }
                else
                {
                    origem.X = (int)((Centro().X - cameraPos.X) / (double)(Centro().Z - cameraPos.Z) * zoom) + pontoTela.X;
                    origem.Y = (int)(-(Centro().Y - cameraPos.Y) / (double)(Centro().Z - cameraPos.Z) * zoom) + pontoTela.Y;

                    tela[i].X = ((vec.X - cameraPos.X) / (vec.Z - cameraPos.Z) * zoom + Centro().X);
                    tela[i].Y = (-(vec.Y - cameraPos.Y) / (vec.Z - cameraPos.Z) * zoom + Centro().Y);

                    tela[i].X = (int)tela[i].X;
                    tela[i].Y = (int)tela[i].Y;
                }
            }

            // Renderização
            for (int i = 1; i < Vertices.Count() + 1; i++)
            {
                PointF pontoA, pontoB;
                if (i == Vertices.Count)
                {
                    pontoA = new PointF(Vertices[i - 1].X, Vertices[i - 1].Y);
                    pontoB = new PointF(Vertices[0].X, Vertices[0].Y);
                }
                else
                {
                    pontoA = new PointF(Vertices[i - 1].X, Vertices[i - 1].Y);
                    pontoB = new PointF(Vertices[i].X, Vertices[i].Y);
                }

                g.DrawLine(new Pen(new SolidBrush(Color.Black)), pontoA, pontoB);
            }
        }

        private void NovoAnguloX(float novoAngX)
        {
            float graus = novoAngX - _angX;
            _angX += novoAngX - _angX;
            Rotacionar(graus, 0, 0);
        }

        private void NovoAnguloY(float novoAngY)
        {
            float graus = novoAngY - _angY;
            _angY += novoAngY - _angY;
            Rotacionar(0, graus, 0);
        }

        private void NovoAnguloZ(float novoAngZ)
        {
            float graus = novoAngZ - _angZ;
            _angZ += novoAngZ - _angZ;
            Rotacionar(0, 0, graus);
        }

        private void Rotacionar(float grausX, float grausY, float grausZ)
        {
            for (int i = 0; i < Vertices.Count(); i++)
            {
                Vertices[i].EulerRotacionarX(new Vetor3D(0, 0, 0), grausX);
                Vertices[i].EulerRotacionarY(new Vetor3D(0, 0, 0), grausY);
                Vertices[i].EulerRotacionarZ(new Vetor3D(0, 0, 0), grausZ);
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                AngX += 1;
                AngY += 1;
                AngZ += 1;
            }

            Refresh();
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void NumZ_ValueChanged(object sender, EventArgs e)
        {
            AngZ = (float)NumZ.Value;
        }

        private void FrmPrincipal_SizeChanged(object sender, EventArgs e)
        {
        }

        private void NumX_ValueChanged(object sender, EventArgs e)
        {
            AngX = (float)NumX.Value;
        }

        private void NumY_ValueChanged(object sender, EventArgs e)
        {
            AngY = (float)NumY.Value;
        }
    }
}
