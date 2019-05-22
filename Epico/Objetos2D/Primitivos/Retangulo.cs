using Epico.Sistema2D;
using System;

namespace Epico.Objetos2D.Avancados
{
    public class Retangulo : Primitivo2D
    {

        public Retangulo(float raio)
        {
            Nome = "Retângulo";
            GerarGeometriaRadial(45, raio, 4);

        }

        protected override void GerarGeometriaRadial(float angulo, float raio, int lados)
        {
            float rad = (float)(Math.PI * 2 / lados);
            for (int i = 0; i < lados; i++)
            {
                Vertice2D v = new Vertice2D(this);
                v.X = (float)(Math.Cos(i * rad + Util2D.Angulo2Radiano(angulo)) * (raio * 1.5));
                v.Y = (float)(Math.Sin(i * rad + Util2D.Angulo2Radiano(angulo)) * raio);
                AdicionarVertice(v);
            }
        }
    }
}
