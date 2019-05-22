using EpicoGraphics.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics.Objetos2D.Avancados
{
    public sealed class Estrela : Avancado2D
    {
        #region Campos
        private string _nomePadrao  = "Estrela";
        private int _raioPadrao = 50;
        private float _anguloPadrao = 10;
        private int _ladosPadrao = 10;
        #endregion

        #region Construtor
        /// <summary>
        /// Gera a forma estrela
        /// </summary>
        public Estrela()
        {
            GerarEstrela(_anguloPadrao, null, _raioPadrao, _ladosPadrao);
        }

        /// <summary>
        /// Gera a forma estrela
        /// </summary>
        /// <param name="raio">Raio da estrela</param>
        public Estrela(float raio)
        {
            GerarEstrela(_anguloPadrao, null, raio, _ladosPadrao);
        }

        /// <summary>
        /// Gera a forma estrela
        /// </summary>
        /// <param name="raio_min">Raio mínimo</param>
        /// <param name="raio_max">Raio máximo</param>
        public Estrela(float raio_min, float raio_max)
        {
            GerarEstrela(_anguloPadrao, raio_min, raio_max, _ladosPadrao);
        }

        /// <summary>
        /// Gera a forma estrela
        /// </summary>
        /// <param name="raio_min">Raio mínimo</param>
        /// <param name="raio_max">Raio máximo</param>
        /// <param name="lados">Quantidade de lados</param>
        public Estrela(float? raio_min, float raio_max, int lados)
        {
            GerarEstrela(_anguloPadrao, raio_min, raio_max, lados);
        }
        #endregion

        /// <summary>
        /// Gera a forma estrela
        /// </summary>
        /// <param name="angulo">Ângulo inicial</param>
        /// <param name="raio_min">Raio mínimo</param>
        /// <param name="raio_max">Raio máximo</param>
        /// <param name="lados">Quantidade de lados</param>
        internal void GerarEstrela(float angulo, float? raio_min, float raio_max, int lados)
        {
            Nome = _nomePadrao;
            if (raio_min != null)
            {
                GerarGeometriaRadial(angulo, raio_min.Value, raio_max, lados);
            }
            else
            {
                float tmp = (raio_max / 2.5F);
                GerarGeometriaRadial(angulo, tmp, raio_max, lados);
            }
        }

        protected override void GerarGeometriaRadial(float angulo, float raio_min, float raio_max, int lados)
        {
            float rad = (float)(Math.PI * 2 / lados);
            for (int i = 0; i < lados; i++)
            {
                Vertice2D v = new Vertice2D(this);

                if (i % 2 == 0)
                {
                    v.X = (float)(Math.Cos(i * rad + Util2D.Angulo2Radiano(angulo)) * raio_min);
                    v.Y = (float)(Math.Sin(i * rad + Util2D.Angulo2Radiano(angulo)) * raio_min);
                    v.Raio = raio_min;
                }
                else
                {
                    v.X = (float)(Math.Cos(i * rad + Util2D.Angulo2Radiano(angulo)) * raio_max);
                    v.Y = (float)(Math.Sin(i * rad + Util2D.Angulo2Radiano(angulo)) * raio_max);
                    v.Raio = raio_max;
                }
                v.Rad = i * rad;
                AdicionarVertice(v);
            }

            CriarArestasConvexo();
        }
    }
}
