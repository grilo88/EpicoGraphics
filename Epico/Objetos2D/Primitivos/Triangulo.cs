using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Triangulo : Primitivo2D
    {
        public Triangulo()
        {
            Nome = "Triângulo";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 3);
        }
    }
}