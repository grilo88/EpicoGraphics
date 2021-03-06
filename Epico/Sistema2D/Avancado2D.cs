﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema2D
{
    public class Avancado2D : Objeto2DRenderizar
    {
        protected virtual void GerarGeometriaRadial(float angulo, float raio_min, float raio_max, int lados)
        {
            Random rnd = new Random(Environment.TickCount + lados);
            float rad = (float)(Math.PI * 2 / lados);
            for (int i = 0; i < lados; i++)
            {
                float raio = rnd.Next((int)raio_min, (int)raio_max);
                Vertice2 v = new Vertice2(this);
                v.X = (float)(Math.Sin(i * rad + Util2D.Angulo2Radiano(angulo)) * raio);
                v.Y = (float)(Math.Cos(i * rad + Util2D.Angulo2Radiano(angulo)) * raio);
                AdicionarVertice(v);
            }

            CriarArestasConvexa();
        }
    }
}
