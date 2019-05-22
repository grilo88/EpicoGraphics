using EpicoGraphics.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics.Luzes
{
    public sealed class LuzPonto : Luz2D
    {
        public LuzPonto(byte intensidade, float raio)
        {
            Nome = "LuzPonto";
            Intensidade = intensidade;
            GerarLuzPonto(0, raio);
        }
    }
}
