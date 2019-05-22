using EpicoGraphics.Sistema2D;
using System;

namespace EpicoGraphics.Objetos2D.Primitivos
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
