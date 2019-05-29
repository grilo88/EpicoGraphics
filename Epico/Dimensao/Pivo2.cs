using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epico.Sistema;

namespace Epico
{
    /// <summary>
    /// Pivô de 2 dimensões
    /// </summary>
    public class Pivo2 : Eixos2
    {
        public Pivo2() : base() { }
        public Pivo2(float X, float Y) : base(X, Y) { }

        public override Eixos NovaInstancia() => new Pivo2();

        public override Eixos NovaInstancia(ObjetoEpico epico)
        {
            throw new NotImplementedException();
        }
    }
}
