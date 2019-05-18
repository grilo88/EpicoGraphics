using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public sealed class Hexagono : Primitivo2D
    {
        public Hexagono(float raio)
        {
            Nome = "Hexágono";
            GerarGeometriaRadial(0, raio, 6);
        }
    }
}
