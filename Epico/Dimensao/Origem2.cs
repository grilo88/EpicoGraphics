using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Origem de 2 dimensões
    /// </summary>
    public class Origem2 : Eixos2
    {
        public Origem2() : base() { }
        public Origem2(Geometria Obj) : base(Obj) { }
        public Origem2(Geometria Obj, float X, float Y) : base(Obj, X, Y) { }
        public Origem2(float X, float Y) : base(X, Y) { }
        public override Eixos NovaInstancia() => new Origem2();
    }
}
