using System;
using System.Collections.Generic;

#if EtoForms
using Eto.Drawing;
#else
using System.Drawing;
#endif

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    /// <summary>
    /// Vértice de 3 dimensões
    /// </summary>
    public class Vertice3 : Eixos3
    {
        public Vertice3() : base() { }
        public Vertice3(float X, float Y, float Z) : base(X, Y, Z) { }

        public float Raio { get; set; }
        public float Ang { get; set; }
        public float Rad { get; set; }

        public override Eixos NovaInstancia() => new Vertice3();

        /// <summary>
        /// Distância delta ao quadrado entre este e outros vértices
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float QuadradoDeltaXY(Vertice3 t)
        {
            float dx = (X - t.X);
            float dy = (Y - t.Y);
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Retângulo dentro da região?
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool DentroXY(RectangleF region)
        {
            if (X < region.Left) return false;
            if (X > region.Right) return false;
            if (Y < region.Top) return false;
            if (Y > region.Bottom) return false;
            return true;
        }
    }
}
