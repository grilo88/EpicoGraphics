using Epico.Sistema2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epico.Sistema
{
    public abstract class ObjetoEpico
    {
        public EpicoGraphics _epico { get; set; }
        public string Nome { get; set; } = "Objeto2D";
        public bool Selecionado { get; set; }
        public virtual RGBA Cor { get; set; }
    }
}
