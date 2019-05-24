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
        public Vetor3(Geometria Obj) : base(Obj) { }
        public Vetor3(Geometria Obj, Vetor3 vetor) : base(Obj, vetor) { }
        public Vetor3(float X, float Y, float Z) : base(X, Y, Z) { }
        public Vetor3(Geometria Obj, float X, float Y, float Z) : base(Obj, X, Y, Z) { }

        public override Eixos NovaInstancia() => new Vetor3();
    }
}
