using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Vetor de 3 dimensões
    /// </summary>
    public class Vetor3 : Eixos3
    {
        public Vetor3() : base() { }
        public Vetor3(object Obj) : base(Obj) { }
        public Vetor3(object Obj, Vetor3 vetor) : base(Obj, vetor) { }
        public Vetor3(float X, float Y, float Z) : base(X, Y, Z) { }
        public Vetor3(object Obj, float X, float Y, float Z) : base(Obj, X, Y, Z) { }

        public override Eixos NovaInstancia() => new Vetor3();

        public static Vetor3 operator -(Vetor3 a, Vetor3 b)
        {
            return new Vetor3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
    }
}
