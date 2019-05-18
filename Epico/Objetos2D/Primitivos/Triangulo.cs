using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Triangulo : Primitivo2D
    {
        public Triangulo(float raio)
        {
            Nome = "Triângulo";
            GerarGeometriaRadial(10, raio, 3);
        }
    }
}