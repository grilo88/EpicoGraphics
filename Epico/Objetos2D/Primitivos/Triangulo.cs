using Epico.Sistema2D;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Triangulo : Primitivo2D
    {
        public Triangulo(float raio)
        {
            Nome = "Triângulo";
            GerarGeometriaRadial(0, raio, 3);
        }
    }
}