using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema3D
{
    /// <summary>
    /// Classe abstrata que indica a existência dos eixos XYZ
    /// </summary>
    public abstract class EixoXYZ
    {
        /// <summary>
        /// Objeto 2D da qual esta posição2D está associada
        /// </summary>
        public Objeto3D obj;

        /// <summary>Posição local na coordenada x</summary>
        public float X { get; set; }

        /// <summary>Posição local na coordenada y</summary>
        public float Y { get; set; }
        /// <summary>Posição local na coordenada z</summary>
        public float Z { get; set; }

        [Browsable(false)]
        /// <summary>Seleção</summary>
        public bool Sel { get; set; }

        /// <summary>Posição global na coordenada X</summary>
        public float GlobalX => obj.Pos.X + X;
        /// <summary>Posição global na coordenada Y</summary>
        public float GlobalY => obj.Pos.Y + Y;
        /// <summary>Posição global na coordenada Z</summary>
        public float GlobalZ => obj.Pos.Z + Z;

        public EixoXYZ Subtrair(EixoXYZ origem)
        {
            X -= origem.X;
            Y -= origem.Y;
            Z -= origem.Z;
            return this;
        }

        public EixoXYZ Global => new XYZ(GlobalX, GlobalY, GlobalZ);

        public static EixoXYZ operator -(EixoXYZ a, EixoXYZ b)
        {
            return new XYZ(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public float ProdutoPontual(EixoXYZ vetor)
        {
            return this.X * vetor.X + this.Y * vetor.Y + this.Z * vetor.Z;
        }

        public float Magnitude
        {
            get => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public EixoXYZ ObterNormalizado()
        {
            float magnitude = Magnitude;
            return new XYZ(X / magnitude, Y / magnitude, Z / magnitude);
        }

        public void Normalizar()
        {
            float magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
            Z /= magnitude;
        }

        public float DistanciaAte(EixoXYZ vetor)
        {
            return (float)Math.Sqrt(Math.Pow(vetor.X - this.X, 2) + Math.Pow(vetor.Y - this.Y, 2));
        }

        public static Vetor3D operator *(EixoXYZ a, float b)
        {
            return new Vetor3D(a.obj, a.X * b, a.Y * b, a.Z * b);
        }
    }


    public sealed class XYZ : EixoXYZ
    {
        public XYZ() { }
        public XYZ(float x, float y, float z)
        {
            base.X = x;
            base.Y = y;
            base.Z = z;
        }
    }

    public sealed class Origem3D : EixoXYZ
    {
        /// <summary>
        /// Utilizado ao gerar a forma geométrica
        /// </summary>
        /// <param name="obj">Objeto2D da qual o Centro2D está associado</param>
        /// <param name="x">Local x</param>
        /// <param name="y">Local y</param>
        public Origem3D(Objeto3D obj, float x, float y, float z)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
            base.Z = z;
        }
    }

    public sealed class Vetor3D : EixoXYZ
    {
        public Vetor3D() { }

        public Vetor3D(Objeto3D obj) { base.obj = obj; }

        public Vetor3D(EixoXYZ xyz)
        {
            base.obj = xyz.obj;
            base.X = xyz.X;
            base.Y = xyz.Y;
            base.Z = xyz.Z;
        }

        public Vetor3D(Objeto3D obj, EixoXYZ eixo)
        {
            base.obj = obj;
            base.X = eixo.X;
            base.Y = eixo.Y;
            base.Z = eixo.Z;
        }

        public Vetor3D(float x, float y, float z)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
            base.Z = z;
        }

        /// <summary>
        /// Novo vetor 3D
        /// </summary>
        /// <param name="obj">Objeto2D da qual o Vetor2D está associado</param>
        /// <param name="x">Local x</param>
        /// <param name="y">Local y</param>
        /// <param name="z">Local z</param>
        public Vetor3D(Objeto3D obj, float x, float y, float z)
        {
            base.obj = obj;
            base.X = x;
            base.Y = y;
            base.Z = z;
        }

        public static Vetor3D operator +(Vetor3D a, Vetor3D b)
        {
            return new Vetor3D(a.obj, a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vetor3D operator +(Vetor3D a, EixoXYZ b)
        {
            return new Vetor3D(a.obj, a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vetor3D operator -(Vetor3D a)
        {
            return new Vetor3D(a.obj, -a.X, -a.Y, -a.Z);
        }

        public static Vetor3D operator -(Vetor3D a, Vetor3D b)
        {
            return new Vetor3D(a.obj, a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vetor3D operator *(Vetor3D a, float b)
        {
            return new Vetor3D(a.obj, a.X * b, a.Y * b, a.Z * b);
        }

        public static Vetor3D operator *(Vetor3D a, int b)
        {
            return new Vetor3D(a.obj, a.X * b, a.Y * b, a.Z * b);
        }

        public static Vetor3D operator *(Vetor3D a, double b)
        {
            return new Vetor3D(a.obj, (float)(a.X * b), (float)(a.Y * b), (float)(a.Z * b));
        }

        public override bool Equals(object obj)
        {
            Vetor3D v = (Vetor3D)obj;

            return X == v.X && Y == v.Y && Z == v.Z;
        }

        public bool Equals(Vetor3D v)
        {
            return X == v.X && Y == v.Y && Z == v.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(Vetor3D a, Vetor3D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vetor3D a, Vetor3D b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public string ToString(bool arredondado)
        {
            if (arredondado)
            {
                return (int)Math.Round(X) + ", " + (int)Math.Round(Y) + ", " +(int)Math.Round(Z);
            }
            else
            {
                return ToString();
            }
        }
    }

    public sealed class Vertice3D : EixoXYZ
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
        public Vertice3D(Objeto3D obj)
        {
            base.obj = obj;
        }

        public Vertice3D(float x, float y, float z) : this(null, x, y, z) { }
        public Vertice3D(EixoXYZ xyz) : this(null, xyz.X, xyz.Y, xyz.Z) { }

        public Vertice3D(Objeto3D obj, float x, float y, float z)
        {
            base.obj = obj ?? base.obj;
            base.X = x;
            base.Y = y;
            base.Z = z;
        }
    }
}
