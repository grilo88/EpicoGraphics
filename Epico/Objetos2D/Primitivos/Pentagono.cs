using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Pentagono : Primitivo2D
    {
        public Pentagono()
        {
            Nome = "Pentágono";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 5);
        }
    }
}