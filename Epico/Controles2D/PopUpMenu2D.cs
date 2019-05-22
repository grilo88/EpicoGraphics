using Epico.Sistema2D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epico.Controles
{
    public class PopUpMenu2D : Controle2D
    {
        private string _nomePadrao = "PopUpMenu";

        public PopUpMenu2D(Controle2D parent)
        {
            Nome = _nomePadrao;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            GerarControle(0, 0, 100, 100);
            Mat_render.CorSolida = new RGBA(200, 0, 200, 88);
        }

        // TODO: Urgente: A necessidade desse controle é urgente visto que deverá ser usado pelo Editor 2D
    }
}
