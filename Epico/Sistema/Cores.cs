using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    public struct RGBA
    {
        /// <summary>Red (Vermelho)</summary>
        public byte R;
        /// <summary>Green (Verde)</summary>
        public byte G;
        /// <summary>Blue (Azul)</summary>
        public byte B;
        /// <summary>Alpha (Opacidade)</summary>
        public byte A;

        public RGBA(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
    }
}
