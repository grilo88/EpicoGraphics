using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Quadrado : Primitivo2D
    {
        public Quadrado()
        {
            Nome = "Quadrado";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 4);
        }
    }
}
