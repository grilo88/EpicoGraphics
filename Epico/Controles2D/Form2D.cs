using EpicoGraphics.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics.Controles
{
    public class Form2D : Controle2D
    {
        private string _nomePadrao = "Form";

        public Form2D()
        {
            Nome = _nomePadrao;
            GerarControle(0, 0, 640, 480);
        }
    }
}
