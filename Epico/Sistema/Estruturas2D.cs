using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    /// <summary>
    /// Classe abstrata que indica a existência dos eixos X e Y
    /// </summary>
    public abstract class EixoXY
    {
        /// <summary>
        /// Objeto 2D da qual esta posição2D está associada
        /// </summary>
        protected Objeto2D obj;

        /// <summary>Posição local na coordenada x</summary>
        public float X { get; set; }

        /// <summary>Posição local na coordenada y</summary>
        public float Y { get; set; }

        /// <summary>Seleção</summary>
        public bool Sel { get; set; }

        /// <summary>Posição global na coordenada X</summary>
        public float GlobalX => obj.Pos.X + X;

        /// <summary>Posição global na coordenada Y</summary>
        public float GlobalY => obj.Pos.Y + Y;
    }

    public sealed class XY : EixoXY
    {
        public XY() { }
        public XY(float x, float y)
        {
            base.X = x;
            base.Y = y;
        }
    }

    public sealed class Origem2D : EixoXY
    {
        /// <summary>
        /// Utilizado ao gerar a forma geométrica
        /// </summary>
        /// <param name="obj">Objeto2D da qual o Centro2D está associado</param>
        /// <param name="x">Local x</param>
        /// <param name="y">Local y</param>
        public Origem2D(Objeto2D obj, float x, float y)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
        }
    }

    public sealed class Vetor2D : EixoXY
    {
        public Vetor2D(float x, float y)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
        }

        /// <summary>
        /// Utilizado ao gerar a forma geométrica
        /// </summary>
        /// <param name="obj">Objeto2D da qual o Vetor2D está associado</param>
        /// <param name="x">Local x</param>
        /// <param name="y">Local y</param>
        public Vetor2D(Objeto2D obj, float x, float y)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
        }
    }

    public sealed class Vertice2D : EixoXY
    {
        public string Nome { get; set; }

        /// <summary>Radiano</summary>
        public float Rad { get; set; }
        /// <summary>Raio</summary>
        public float Raio { get; set; }
        /// <summary>Ângulo</summary>
        public float Ang { get; set; }

        /// <summary>
        /// Utilizado ao gerar a forma geométrica
        /// </summary>
        /// <param name="obj">Objeto2D da qual o Vertice2D está associado</param>
        public Vertice2D(Objeto2D obj)
        {
            base.obj = obj;
        }

        public Vertice2D(float x, float y)
        {
            base.X = x;
            base.Y = y;
        }
    }
}
