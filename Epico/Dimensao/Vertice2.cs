using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public class Vertice
    {
    
    }


    /// <summary>
    /// Vértice de 2 dimensões
    /// </summary>
    public class Vertice2 : Eixos2
    {
        public Vertice2() : base() { }
        public Vertice2(Eixos2 eixos2) : base(eixos2) { }
        public Vertice2(float X, float Y) : base(X, Y) { }

        public Vertice2(ObjetoEpico Obj) : base(Obj) { }
        public Vertice2(ObjetoEpico Obj, float X, float Y) : base(Obj, X, Y) { }

        public float Raio { get; set; }
        public float Ang { get; set; }
        public float Rad { get; set; }

        public override Eixos NovaInstancia() => new Vertice2();

        public override Eixos NovaInstancia(ObjetoEpico epico)
        {
            throw new NotImplementedException();
        }
    }
}
