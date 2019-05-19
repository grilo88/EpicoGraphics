using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Controles
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
