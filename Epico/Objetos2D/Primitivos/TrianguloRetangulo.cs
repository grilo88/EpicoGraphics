using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public sealed class TrianguloRetangulo : Primitivo2D
    {
        public TrianguloRetangulo()
        {
            Nome = "TriânguloRetângulo";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 3);
        }
        protected override void GerarGeometriaRadial(int angulo, int raio, int lados)
        {
            float rad = (float)(Math.PI * 2 / (lados + 1));
            for (int i = 0; i < lados; i++)
            {
                Vertice2D v = new Vertice2D(this);
                v.X = (float)(Math.Sin(i * rad + Util.Angulo2Radiano(angulo)) * raio);
                v.Y = (float)(Math.Cos(i * rad + Util.Angulo2Radiano(angulo)) * raio);
                AdicionarVertice(v);
            }
        }

    }
}
