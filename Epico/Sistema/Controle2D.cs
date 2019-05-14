using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    public delegate void MouseClick(Object sender, EventArgs e);
    public delegate void MouseUp(Object sender, EventArgs e);
    public delegate void MouseDown(Object sender, EventArgs e);
    public delegate void MouseMove(Object sender, EventArgs e);

    public class Controle2D : Objeto2DRenderizar
    {
        public float Top { get; set; }      // Direita
        public float Left { get; set; }     // Esquerda
        public float Width { get; set; }    // Largura
        public float Height { get; set; }   // Altura

        public Controle2D Parent { get; set; }

        #region Eventos
        public event EventHandler MouseClick;
        public event EventHandler MouseUp;
        public event EventHandler MouseDown;
        public event EventHandler MouseMove;

        protected virtual void OnMouseClick(EventArgs e)
        {
            EventHandler handler = MouseClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnMouseUp(EventArgs e)
        {
            EventHandler handler = MouseUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnMouseDown(EventArgs e)
        {
            EventHandler handler = MouseDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnMouseMove(EventArgs e)
        {
            EventHandler handler = MouseMove;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
