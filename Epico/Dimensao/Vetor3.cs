using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Vetor de 3 dimensões para ambiente 2D (não 3d)
    /// </summary>
    public class Vetor3 : Eixos2
    {
        /// <summary>
        /// Coordenada Z
        /// </summary>
        public float Z { get; set; }

        public Vetor3() : base() { }
        public Vetor3(Eixos2 eixos2) : base(eixos2) { }
        public Vetor3(ObjetoEpico Obj) : base(Obj) { }
        public Vetor3(ObjetoEpico Obj, Vetor2 vetor) : base(Obj, vetor) { }
        public Vetor3(float X, float Y, float Z) : base(X, Y) { this.Z = Z; }
        public Vetor3(ObjetoEpico Obj, float X, float Y, float Z) : base(Obj, X, Y) { this.Z = Z; }

        public override Eixos NovaInstancia() => new Vetor3();

        public override Eixos NovaInstancia(ObjetoEpico epico)
        {
            throw new NotImplementedException();
        }

        public static Vetor3 operator -(Vetor3 a)
        {
            a.X = -a.X;
            a.Y = -a.Y;
            a.Z = -a.Z;
            return a; 
        }

        public static Vetor3 operator +(Vetor3 a, Vetor3 b)
        {
            Vetor3 ret = new Vetor3
            {
                X = a.X + b.X,
                Y = a.Y + a.Y,
                Z = a.Z + a.Z
            };
            return ret;
        }

        public static Vetor3 operator -(Vetor3 a, Vetor3 b)
        {
            Vetor3 ret = (Vetor3)a.NovaInstancia();
            ret.X = a.X - b.X;
            ret.Y = a.Y - b.Y;
            ret.Z = a.Z - b.Z;
            return ret;
        }

        public static Vetor3 operator *(Vetor3 a, float b)
        {
            Vetor3 ret = (Vetor3)a.NovaInstancia();
            ret.X = a.X * b;
            ret.Y = a.Y * b;
            ret.Z = a.Z * b;
            return ret;
        }

        public static Vetor3 operator /(Vetor3 a, float b)
        {
            Vetor3 ret = (Vetor3)a.NovaInstancia();
            ret.X = a.X / b;
            ret.Y = a.Y / b;
            ret.Z = a.Z / b;
            return ret;
        }
    }
}
