using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    public class Material
    {
        public float LarguraBorda { get; set; } = 2F;
        public RGBA CorBorda { get; set; } = new RGBA(255, 255, 255, 255);    // Branco
        public RGBA CorSolida { get; set; } = new RGBA(0, 0, 0, 0);     // Transparente
        
        public object Textura { get; set; } = null;
    }
}
