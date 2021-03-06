﻿using Epico.Sistema2D;
using System;

namespace Epico.Objetos2D.Primitivos
{
    public sealed class TrianguloRetangulo : Primitivo2D
    {
        public TrianguloRetangulo(float raio)
        {
            Nome = "TriânguloRetângulo";
            GerarGeometriaRadial(45, raio, 3);
        }

        protected override void GerarGeometriaRadial(float angulo, float raio, int lados)
        {
#warning Verificar lados+1 se está duplicando a última vértice. Se sim modificar solução.
            float rad = (float)(Math.PI * 2 / /*Verificar*/(lados + 1));
            for (int i = 0; i < lados; i++)
            {
                Vertice2 v = new Vertice2(this);
                v.X = (float)(Math.Sin(i * rad + Util2D.Angulo2Radiano(angulo)) * raio);
                v.Y = (float)(Math.Cos(i * rad + Util2D.Angulo2Radiano(angulo)) * raio);
                AdicionarVertice(v);
            }
        }

    }
}
