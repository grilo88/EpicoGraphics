using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Pivô de 4 dimensões
    /// </summary>
    public class Pivo4 : Eixos4
    {
        public Pivo4() : base() { }
        public Pivo4(float W, float X, float Y, float Z) : base(W, X, Y, Z) { }

        public override Eixos NovaInstancia() => new Pivo4();
    }
}
