using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Objetos2D.Avancados
{
    public sealed class Estrela : Avancado2D
    {
        public Estrela()
        {
            Nome = "Estrela";
        }

        public void GerarGeometria(int angulo, int raio_min, int raio_max, int lados)
        {
            GerarGeometriaRadialVariante(angulo, raio_min, raio_max, lados);
        }

        protected override void GerarGeometriaRadialVariante(int angulo, int raio_min, int raio_max, int lados)
        {
            raio_min = (int)(raio_max / 2.5F);
            float rad = (float)(Math.PI * 2 / lados);
            for (int i = 0; i < lados; i++)
            {
                Vertice2D v = new Vertice2D(this);

                if (i % 2 == 0)
                {
                    v.X = (float)(Math.Cos(i * rad + Util.Angulo2Radiano(angulo)) * raio_min);
                    v.Y = (float)(Math.Sin(i * rad + Util.Angulo2Radiano(angulo)) * raio_min);
                    v.Raio = raio_min;
                }
                else
                {
                    v.X = (float)(Math.Cos(i * rad + Util.Angulo2Radiano(angulo)) * raio_max);
                    v.Y = (float)(Math.Sin(i * rad + Util.Angulo2Radiano(angulo)) * raio_max);
                    v.Raio = raio_max;
                }
                v.Rad = i * rad;
                AdicionarVertice(v);
            }
        }
    }
}
