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
    public class Origem3 : Eixos3
    {
        public Origem3() : base() { }
        public Origem3(float X, float Y, float Z) : base(X, Y, Z) { }
        public override Eixos NovaInstancia() => new Origem3();
    }
}
