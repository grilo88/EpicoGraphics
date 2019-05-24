using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Vetor de 4 dimensões
    /// </summary>
    public class Vetor4 : Eixos4
    {
        public Vetor4() : base() { }
        public Vetor4(float W, float X, float Y, float Z) : base(W, X, Y, Z) { }

        public override Eixos NovaInstancia() => new Vetor4();
    }
}
