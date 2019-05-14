using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rotate2D
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        // Objeto 1
        PointF origem = new PointF();
        PointF pontoA = new PointF();
        PointF pontoB = new PointF();

        // Objeto 2
        PointF pontoC = new PointF();
        PointF pontoD = new PointF();

        /// <summary>Ângulo do PontoB do objeto 1</summary>
        float ang = 0;
        /// <summary>Raio do PontoB do objeto 1</summary>
        float raio = 50;

        /// <summary>Ângulo do PontoD do objeto 1</summary>
        float ang2 = 0;
        /// <summary>Raio do PontoD do objeto 1</summary>
        float raio2 = 50;

        private void Form1_Load(object sender, EventArgs e)
        {
            origem.X = ClientRectangle.Width / 2;
            origem.Y = ClientRectangle.Height / 2;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            ang = trackBar1.Value;
            Refresh();
        }
        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            ang2 = trackBar2.Value;
            Refresh();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            #region Objeto 1
            pontoA.X = origem.X;
            pontoA.Y = origem.Y;

            pontoB.X = origem.X + (float)Math.Sin(DegreeToRadian(ang)) * raio; // Define o tamanho do objeto 1
            pontoB.Y = origem.Y + (float)Math.Cos(DegreeToRadian(ang)) * raio; // Define o tamanho do objeto 1
            #endregion

            #region Objeto 2
            // Acopla objeto2 no objeto1 definindo 
            // a origem do objeto 2 igual ao ponto b do objeto 1 (Imagine uma espada nas mãos de um ator)
            PointF origem2 = new PointF();

            float distX = (float)Math.Sin(DegreeToRadian(ang)) * 20;    // Aumenta raio 20 de distância do ponto b entre objeto 2 e 1
            float distY = (float)Math.Cos(DegreeToRadian(ang)) * 20;    // Aumenta raio 20 de distância do ponto b entre objeto 2 e 1
            // Obs.: O ângulo acima obtém do ponto b do objeto 1 como referência pois a mesma será aplicada na origem do objeto 2

            origem2.X = pontoB.X + distX; 
            origem2.Y = pontoB.Y + distY; 

            pontoC.X = origem2.X;
            pontoC.Y = origem2.Y;

            pontoD.X = origem2.X + (float)Math.Sin(DegreeToRadian(ang2)) * raio2; // Define o tamanho do objeto 2
            pontoD.Y = origem2.Y + (float)Math.Cos(DegreeToRadian(ang2)) * raio2; // Define o tamanho do objeto 2 

            pontoD = RotacionarPonto(origem2, pontoD, ang2 + ang /* Soma de ângulos */);
            #endregion

            #region Renderização
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Black), 2), pontoA, pontoB);    // Objeto 1
            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 4), pontoC, pontoD);     // Objeto 2
            #endregion
        }

        /// <summary>
        /// Rotaciona um ponto 2D
        /// </summary>
        /// <param name="origem">Ponto de origem do objeto</param>
        /// <param name="p">Ponto a ser rotacionado</param>
        /// <param name="ang">Soma dos ângulos</param>
        /// <returns></returns>
        private PointF RotacionarPonto(PointF origem, PointF p, float ang)
        {
            float rad = DegreeToRadian(-ang);
            float rotX = (float)(Math.Cos(rad) * (p.X - origem.X) - Math.Sin(rad) * (p.Y - origem.Y) + origem.X);
            float rotY = (float)(Math.Sin(rad) * (p.X - origem.X) + Math.Cos(rad) * (p.Y - origem.Y) + origem.Y);

            return new PointF(rotX, rotY);
        }

        private float DegreeToRadian(float angle)
        {
            return (float)Math.PI * angle / 180;
        }
        private float RadianToDegree(float angle)
        {
            return angle * (180 / (float)Math.PI);
        }

       
    }
}
