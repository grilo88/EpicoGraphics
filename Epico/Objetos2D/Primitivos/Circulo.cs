using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Circulo : Primitivo2D
    {
        public Circulo()
        {
            Nome = "Círculo";
        }

        public void GerarGeometria(int angulo, int raio, int lados = 20)
        {
            GerarGeometriaRadial(angulo, raio, lados);
        }
    }
}
