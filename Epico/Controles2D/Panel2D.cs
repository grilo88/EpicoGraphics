using Epico.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epico.Controles
{
    public class Panel2D : Controle2D
    {
        private string _nomePadrao = "Panel";

        public Panel2D(EpicoGraphics engine, Controle2D parent)
        {
            _epico = engine;

            Nome = _nomePadrao;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));

            var list = ObterObjetosDesteContainer()
                .Select(obj => new { obj, mult = obj.Pos.X * obj.Pos.Y })
                .OrderByDescending(x => x.mult).ToList();

            var last = ObterObjetosDesteContainer()
                .Select(obj => new { obj, mult = obj.Pos.X * obj.Pos.Y })
                .OrderByDescending(x => x.mult).FirstOrDefault();

            Location loc = new Location();
            if (last != null)
            {
                loc.X = last.obj.Pos.X + 20;
                loc.Y = last.obj.Pos.X + 20;
            }

            GerarControle(loc.X, loc.Y, 100, 100);
            Mat_render.CorSolida = new RGBA(200, 0, 200, 88);
        }
    }
}
