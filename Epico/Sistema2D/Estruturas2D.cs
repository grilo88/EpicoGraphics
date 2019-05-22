using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics.Sistema2D
{
    /// <summary>
    /// Classe abstrata que indica a existência dos eixos X e Y
    /// </summary>
    public abstract class EixoXY
    {
        /// <summary>
        /// Objeto 2D da qual esta posição2D está associada
        /// </summary>
        public Objeto2D obj;

        /// <summary>Posição local na coordenada x</summary>
        public float X { get; set; }

        /// <summary>Posição local na coordenada y</summary>
        public float Y { get; set; }

        [Browsable(false)]
        /// <summary>Seleção</summary>
        public bool Sel { get; set; }

        /// <summary>Posição global na coordenada X</summary>
        public float GlobalX => obj.Pos.X + X;

        /// <summary>Posição global na coordenada Y</summary>
        public float GlobalY => obj.Pos.Y + Y;

        public EixoXY Global => new XY(GlobalX, GlobalY);

        public static EixoXY operator -(EixoXY a, EixoXY b)
        {
            return new XY(a.X - b.X, a.Y - b.Y);
        }

        public float ProdutoPontual(EixoXY vetor)
        {
            return this.X * vetor.X + this.Y * vetor.Y;
        }

        public float Magnitude
        {
            get => (float)Math.Sqrt(X * X + Y * Y);
        }

        public EixoXY ObterNormalizado()
        {
            float magnitude = Magnitude;

            return new XY(X / magnitude, Y / magnitude);
        }

        public void Normalizar()
        {
            float magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
        }

        public float DistanciaAte(EixoXY vetor)
        {
            return (float)Math.Sqrt(Math.Pow(vetor.X - this.X, 2) + Math.Pow(vetor.Y - this.Y, 2));
        }

        public static Vetor2D operator *(EixoXY a, float b)
        {
            return new Vetor2D(a.obj, a.X * b, a.Y * b);
        }
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
        public Vetor2D() { }

        public Vetor2D(Objeto2D obj) { base.obj = obj; }

        public Vetor2D(EixoXY xy)
        {
            base.X = xy.X;
            base.Y = xy.Y;
        }

        public Vetor2D(Objeto2D obj, EixoXY eixo)
        {
            base.obj = obj;
            base.X = eixo.X;
            base.Y = eixo.Y;
        }

        public Vetor2D(float x, float y)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
        }

        /// <summary>
        /// Novo vetor 2D
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


        public static Vetor2D operator +(Vetor2D a, Vetor2D b)
        {
            return new Vetor2D(a.obj, a.X + b.X, a.Y + b.Y);
        }

        public static Vetor2D operator +(Vetor2D a, EixoXY b)
        {
            return new Vetor2D(a.obj, a.X + b.X, a.Y + b.Y);
        }

        public static Vetor2D operator -(Vetor2D a)
        {
            return new Vetor2D(a.obj, -a.X, -a.Y);
        }

        public static Vetor2D operator -(Vetor2D a, Vetor2D b)
        {
            return new Vetor2D(a.obj, a.X - b.X, a.Y - b.Y);
        }

        

        public static Vetor2D operator *(Vetor2D a, float b)
        {
            return new Vetor2D(a.obj, a.X * b, a.Y * b);
        }

        public static Vetor2D operator *(Vetor2D a, int b)
        {
            return new Vetor2D(a.obj, a.X * b, a.Y * b);
        }

        public static Vetor2D operator *(Vetor2D a, double b)
        {
            return new Vetor2D(a.obj, (float)(a.X * b), (float)(a.Y * b));
        }

        public override bool Equals(object obj)
        {
            Vetor2D v = (Vetor2D)obj;

            return X == v.X && Y == v.Y;
        }

        public bool Equals(Vetor2D v)
        {
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vetor2D a, Vetor2D b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vetor2D a, Vetor2D b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        public string ToString(bool arredondado)
        {
            if (arredondado)
            {
                return (int)Math.Round(X) + ", " + (int)Math.Round(Y);
            }
            else
            {
                return ToString();
            }
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

        public Vertice2D(float x, float y) : this(null, x, y) { }
        public Vertice2D(EixoXY xy) : this(null, xy.X, xy.Y) { }

        public Vertice2D(Objeto2D obj, float x, float y)
        {
            base.obj = obj ?? base.obj;
            base.X = x;
            base.Y = y;
        }
    }
}
