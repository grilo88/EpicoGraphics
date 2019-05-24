using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Pivô de 3 dimensões
    /// </summary>
    public class Pivo3 : Eixos3
    {
        public Pivo3() : base() { }
        public Pivo3(float X, float Y, float Z) : base(X, Y, Z) { }

        public override Eixos NovaInstancia() => new Pivo3();
    }
}
