using Epico.Sistema2D;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Circulo : Primitivo2D
    {
        public Circulo(float raio)
        {
            Nome = "Círculo";
            GerarGeometriaRadial(0, raio, 20);
        }
    }
}
