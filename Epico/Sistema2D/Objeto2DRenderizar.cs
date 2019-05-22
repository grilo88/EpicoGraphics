using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema2D
{
    /// <summary>
    /// Objetos que herdarem este tipo serão materializados na Câmera
    /// </summary>
    public class Objeto2DRenderizar : Objeto2D
    {
        public List<Material> Materiais = new List<Material>();
        [Category("Aparência")]
        [DisplayName("Material")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        /// <summary>Material de renderização do objeto</summary>
        public Material Mat_render { get; set; } = new Material();
        [Category("Aparência")]
        [DisplayName("Seleção")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        /// <summary>Material de renderização quando o objeto estiver selecionado no editor</summary>
        public Material Mat_render_sel { get; set; } = new Material();
    }
}
