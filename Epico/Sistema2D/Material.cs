using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics.Sistema2D
{
    public class Material
    {
        [DisplayName("Borda")]
        public float LarguraBorda { get; set; } = 2F;
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public RGBA CorBorda { get; set; } = new RGBA(255, 255, 255, 255);    // Branco
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public RGBA CorSolida { get; set; } = new RGBA(0, 0, 0, 0);     // Transparente
        
        public object Textura { get; set; } = null;
    }
}
