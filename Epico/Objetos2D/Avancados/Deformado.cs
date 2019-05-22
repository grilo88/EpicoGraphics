using EpicoGraphics.Sistema2D;
using System;

namespace EpicoGraphics.Objetos2D.Avancados
{
    public class Deformado : Avancado2D
    {
        public Deformado()
        {
            Nome = "Deformado";
        }

        public void GerarGeometria(int angulo, int raio_min, int raio_max)
        {
            GerarGeometriaRadial(angulo, raio_min, raio_max, new Random(Environment.TickCount).Next(7, 15));
        }
    }
}
