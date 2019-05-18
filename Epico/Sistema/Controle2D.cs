using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    public class Location
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Location(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size(float Width, float Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }

    public delegate void MouseClick(Object sender, EventArgs e);
    public delegate void MouseUp(Object sender, EventArgs e);
    public delegate void MouseDown(Object sender, EventArgs e);
    public delegate void MouseMove(Object sender, EventArgs e);

    public class Controle2D : Objeto2DRenderizar
    {
        private Vertice2D _vTopLeft;
        private Vertice2D _vTopRigth;
        private Vertice2D _vBottomRight;
        private Vertice2D _vBottomLeft;

        private Location _location;
        public Size _size;

        [Category("Layout")]
        [Description("As coordenadas do canto superior esquerdo do controle em relação ao canto superior esquerdo do seu recipiente.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Location Location { get => _location; set {
                _location = value;
                AtualizarLayout();
            } }

        [Category("Layout")]
        [Description("O tamanho do controle em pixels")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Size Size { get => _size; set {
                _size = value;
                AtualizarLayout();
            }
        }

        [Browsable(false)]
        [Category("Layout")]
        public float Top { get => Location.Y; set => Location.Y = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Left { get => Location.X; set => Location.X = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Width { get => Size.Width; set => Size.Width = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Height { get => Size.Height; set => Size.Height = value; }

        public Controle2D Parent { get; set; }
        public ColecaoControles Controls { get; set; }

        protected virtual void GerarControle(Location local, Size tamanho)
        {
            _location = local;
            _size = tamanho;

            _vTopLeft = new Vertice2D(this);
            _vTopRigth = new Vertice2D(this);
            _vBottomRight = new Vertice2D(this);
            _vBottomLeft = new Vertice2D(this);

            _vTopLeft.X = _location.X;
            _vTopLeft.Y = _location.Y;
            _vTopRigth.X = _location.X + _size.Width;
            _vTopRigth.Y = _location.Y;
            _vBottomRight.X = _location.X + _size.Width;
            _vBottomRight.Y = _location.Y + _size.Height;
            _vBottomLeft.X = _location.X;
            _vBottomLeft.Y = _location.Y + _size.Height;

            AdicionarVertice(_vTopLeft);
            AdicionarVertice(_vTopRigth);
            AdicionarVertice(_vBottomRight);
            AdicionarVertice(_vBottomLeft);

            Origem[0].X = (_location.X + _size.Width) / 2;
            Origem[0].Y = (_location.Y + _size.Height) / 2;

            Mat_render.CorSolida = new RGBA(200, 150, 80, 230);
            Mat_render.CorBorda = new RGBA(255, 255, 255, 255);

            Mat_render.LarguraBorda = 1;
        }

        private void AtualizarLayout()
        {
            _vTopLeft.X = _location.X;
            _vTopLeft.Y = _location.Y;
            _vTopRigth.X = _location.X + _size.Width;
            _vTopRigth.Y = _location.Y;
            _vBottomRight.X = _location.X + _size.Width;
            _vBottomRight.Y = _location.Y + _size.Height;
            _vBottomLeft.X = _location.X;
            _vBottomLeft.Y = _location.Y + _size.Height;
        }

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

    public class ColecaoControles : ICollection<Controle2D>
    {
        List<Controle2D> lista = new List<Controle2D>();

        /// <summary>
        /// O índice do controle a ser recuperado da coleção de controles 2d
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Controle2D this[int index] { get => lista[index]; }

        /// <summary>
        /// O nome do controle a ser recuperado da coleção de controles 2d
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Controle2D this[string name] { get => lista.Where(x => x.Nome == name).First(); }

        public int Count => ((ICollection<Controle2D>)lista).Count;

        public bool IsReadOnly => ((ICollection<Controle2D>)lista).IsReadOnly;

        public void Add(Controle2D item)
        {
            ((ICollection<Controle2D>)lista).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Controle2D>)lista).Clear();
        }

        public bool Contains(Controle2D item)
        {
            return ((ICollection<Controle2D>)lista).Contains(item);
        }

        public void CopyTo(Controle2D[] array, int arrayIndex)
        {
            ((ICollection<Controle2D>)lista).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Controle2D> GetEnumerator()
        {
            return ((ICollection<Controle2D>)lista).GetEnumerator();
        }

        public bool Remove(Controle2D item)
        {
            return ((ICollection<Controle2D>)lista).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<Controle2D>)lista).GetEnumerator();
        }
    }
}
