using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public sealed class Hexagono : Primitivo2D
    {
        public Hexagono()
        {
            Nome = "Hexágono";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 6);
        }
    }
}
