using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Avancados
{
    class Retangulo : Primitivo2D
    {

        public Retangulo()
        {
            Nome = "Retângulo";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 4);
        }

        protected override void GerarGeometriaRadial(int angulo, int raio, int lados)
        {
            float rad = (float)(Math.PI * 2 / lados);
            for (int i = 0; i < lados; i++)
            {
                Vertice2D v = new Vertice2D(this);
                v.X = (float)(Math.Cos(i * rad + Util.Angulo2Radiano(angulo)) * (raio * 1.5));
                v.Y = (float)(Math.Sin(i * rad + Util.Angulo2Radiano(angulo)) * raio);
                AdicionarVertice(v);
            }
        }
    }
}
