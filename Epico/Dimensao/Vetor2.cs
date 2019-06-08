using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    

    /// <summary>
    /// Vetor de 2 dimensões
    /// </summary>
    public class Vetor2 : Eixos2
    {
        public Vetor2() : base() { }
        public Vetor2(Eixos2 eixos2) : base(eixos2) { }
        public Vetor2(ObjetoEpico Obj) : base(Obj) { }
        public Vetor2(ObjetoEpico Obj, Vetor2 vetor) : base(Obj, vetor) { }
        public Vetor2(float X, float Y) : base(X, Y) { }
        public Vetor2(ObjetoEpico Obj, float X, float Y) : base(Obj, X, Y) { }

        public override Eixos NovaInstancia() => new Vetor2();

        public override Eixos NovaInstancia(ObjetoEpico epico)
        {
            throw new NotImplementedException();
        }

        public static Vetor2 operator -(Vetor2 a)
        {
            a.X = -a.X;
            a.Y = -a.Y;
            return a; 
        }

        public static Vetor2 operator +(Vetor2 a, Eixos2 b)
        {
            Vetor2 ret = new Vetor2
            {
                X = a.X + b.X,
                Y = a.Y + a.Y
            };
            return ret;
        }

        public static Vetor2 operator -(Vetor2 a, Vetor2 b)
        {
            Vetor2 ret = (Vetor2)a.NovaInstancia();
            ret.X = a.X - b.X;
            ret.Y = a.Y - b.Y;
            return ret;
        }

        public static Vetor2 operator *(Vetor2 a, float b)
        {
            Vetor2 ret = (Vetor2)a.NovaInstancia();
            ret.X = a.X * b;
            ret.Y = a.Y * b;
            return ret;
        }

        public static Vetor2 operator /(Vetor2 a, float b)
        {
            Vetor2 ret = (Vetor2)a.NovaInstancia();
            ret.X = a.X / b;
            ret.Y = a.Y / b;
            return ret;
        }
    }
}
