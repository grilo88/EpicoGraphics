using EpicoGraphics.Sistema2D;
using System;

namespace EpicoGraphics.Objetos2D.Primitivos
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
