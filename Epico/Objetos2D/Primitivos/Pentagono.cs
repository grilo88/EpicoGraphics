using Epico.Sistema;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public class Pentagono : Primitivo2D
    {
        public Pentagono(float raio)
        {
            Nome = "Pentágono";
            GerarGeometriaRadial(0, raio, 5);
        }
    }
}