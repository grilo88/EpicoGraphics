using EpicoGraphics.Sistema2D;
using System;

namespace EpicoGraphics.Objetos2D.Primitivos
{
    public sealed class Losango : Primitivo2D
    {
        public Losango(float raio)
        {
            Nome = "Losango";
            GerarGeometriaRadial(0, raio, 4);
        }
    }
}
