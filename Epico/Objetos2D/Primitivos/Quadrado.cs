using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Quadrado : Primitivo2D
    {
        public Quadrado(float raio)
        {
            Nome = "Quadrado";
            GerarGeometriaRadial(45, raio, 4);
        }
    }
}
