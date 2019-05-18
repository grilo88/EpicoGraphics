using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Avancados
{
    public class Quadrilatero : Avancado2D
    {
        public Quadrilatero()
        {
            Nome = "Quadrilátero";
        }

        public void GerarGeometria(int angulo, int raio_min, int raio_max)
        {
            GerarGeometriaRadial(angulo, raio_min, raio_max, 4);
        }
    }
}
