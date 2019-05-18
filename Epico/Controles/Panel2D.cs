using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epico.Controles
{
    public class Panel2D : Controle2D
    {
        private string _nomePadrao = "Panel";

        public Panel2D(Controle2D parent)
        {
            Nome = _nomePadrao;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            GerarControle(new Location(0, 0), new Size(100, 100));
        }
    }
}
