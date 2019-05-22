using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema2D
{
    public class RGBA
    {
        [DisplayName("Red")]
        /// <summary>Red (Vermelho)</summary>
        public byte R { get; set; }
        [DisplayName("Green")]
        /// <summary>Green (Verde)</summary>
        public byte G { get; set; }
        [DisplayName("Blue")]
        /// <summary>Blue (Azul)</summary>
        public byte B { get; set; }
        [DisplayName("Alpha")]
        /// <summary>Alpha (Opacidade)</summary>
        public byte A { get; set; }

        public RGBA(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
}
