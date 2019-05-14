using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public sealed class Losango : Primitivo2D
    {
        public Losango()
        {
            Nome = "Losango";
        }

        public void GerarGeometria(int angulo, int raio)
        {
            GerarGeometriaRadial(angulo, raio, 4);
        }
    }
}
