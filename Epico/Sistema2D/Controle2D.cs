using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema2D
{
    public class Location : INotifyPropertyChanged
    {
        private float _x;
        private float _y;

        public float X { get => _x; set
            {
                if (value != _x) // Alterou?
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }
        public float Y { get => _y; set
            {
                if (value != _y) // Alterou?
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        public Location() { }
        public Location(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Size : INotifyPropertyChanged
    {
        private float _width;
        private float _heigth;

        public float Width { get => _width; set
            {
                if (value != _width) // Alterou?
                {
                    _width = value; OnPropertyChanged();
                }
            }
        }
        public float Height { get => _heigth; set
            {
                if (value != _heigth) // Alterou?
                {
                    _heigth = value; OnPropertyChanged();
                }
            }
        }

        public Size(float width, float height)
        {
            _width = width;
            _heigth = height;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public delegate void MouseClick(Object sender, EventArgs e);
    public delegate void MouseUp(Object sender, EventArgs e);
    public delegate void MouseDown(Object sender, EventArgs e);
    public delegate void MouseMove(Object sender, EventArgs e);

    public class Controle2D : Objeto2DRenderizar
    {
        /// <summary>
        /// Container é o objeto2d base principal que encapsula todos os controles filhos, ex.: Form2D, Panel2D, 
        /// quer serve como referência para cálculos e manipulação de OrdemZ local.
        /// </summary>
        public Controle2D Container { get; private set; }

        private Vertice2 _vTopLeft;            // Superior Esquerdo
        private Vertice2 _vTopRigth;           // Superior Direito
        private Vertice2 _vBottomRight;        // Inferior Direito
        private Vertice2 _vBottomLeft;         // Inferior Esquerdo

        private Location _location = new Location(0, 0);
        private Size _size = new Size(300, 300);
        private Controle2D _parent;

        [Category("Layout")]
        public bool Visible { get; set; }

        [Category("Layout")]
        public Vetor2 LocalPos { get; set; }

        public override Vetor2 Pos { get => base.Pos;
            set {
                var tmp = value;
                if (base.Pos == null)
                    base.Pos = new Vetor2(this, tmp.X, tmp.Y);
                else
                {
                    base.Pos.X = tmp.X;
                    base.Pos.Y = tmp.Y;
                }

                AtualizarLayout();
            }
        }

        [Category("Layout")]
        [Description("As coordenadas do canto superior esquerdo do controle em relação ao canto superior esquerdo do seu recipiente.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Location Location
        {
            get => _location; set
            {
                var val = value;
                _location.X = val.X;
                _location.Y = val.Y;
                AtualizarLayout();
            }
        }

        [Category("Layout")]
        [Description("O tamanho do controle em pixels")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Size Size
        {
            get => _size; set
            {
                var val = value;
                _size.Width = val.Width;
                _size.Height = val.Height;
                AtualizarLayout();
            }
        }

        [Category("Ordenação")]
        public int OrdemZ {
            get {
                var objs = ObterObjetosDesteContainer().ToList();
                return objs.FindIndex(x => x == this);
            } set { } }

        [Category("Ordenação")]
        public int OrdemZMax { get {

                return ObterObjetosDesteContainer().Count() - 1;
            } set { } }


        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AtualizarLayout();
        }

        private void Size_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AtualizarLayout();
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

        public IEnumerable<Controle2D> ObterObjetosDesteContainer()
        {
            return _epico.objetos2D.OfType<Controle2D>()
                    .Where(x => x.Container == this.Container);
        }

        public Controle2D Parent { get => _parent; set {
                _parent = value;

                // Adiciona este controle filho na lista de controles do parent (Controle Pai)
                if (!_parent.Controls.Contains(this))
                {
                    _parent.Controls.Add(this);
                    this.Container = _parent.Container ?? _parent; // Define o container
                }
            }
        }

        [Browsable(false)]
        public ColecaoControles Controls { get; private set; } = new ColecaoControles();

        [Category("Layout")]
        [Description("Controles filhos deste controle.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public List<Controle2D> Controles { get => Controls.Cast<Controle2D>().ToList(); }
        public Controle2D()
        {
            if (Pos == null)
                Pos = new Vetor2(this, 0, 0);
            //InicializarSize();
            //InicializarLocation();
        }

        private void InicializarLocation()
        {
            // Define posição x e y do objeto 2d
            Location = new Location(_location.X, _location.Y);
            Location.PropertyChanged += Location_PropertyChanged;
        }

        private void InicializarSize()
        {
            Size = new Size(_size.Width, _size.Height);
            Size.PropertyChanged += Size_PropertyChanged;
        }

        protected virtual void GerarControle(float x, float y, float width, float height)
        {
            _location.X = x;
            _location.Y = y;
            _size.Width = width;
            _size.Height = height;

            InicializarLocation();
            InicializarSize();

            AdicionarVertice(_vTopLeft = new Vertice2(this));
            AdicionarVertice(_vTopRigth = new Vertice2(this));
            AdicionarVertice(_vBottomRight = new Vertice2(this));
            AdicionarVertice(_vBottomLeft = new Vertice2(this));

            Mat_render.CorSolida = new RGBA(200, 150, 80, 230);
            Mat_render.CorBorda = new RGBA(255, 255, 255, 255);
            Mat_render.LarguraBorda = 1;

            AtualizarLayout();

            // Centraliza o ponto de origem
            Origens[0].X = (_location.X + _size.Width) / 2;
            Origens[0].Y = (_location.Y + _size.Height) / 2;
        }

        private void AtualizarLayout()
        {
            if (_location != null && _size != null)
            {
                //Pos.X = _location.X;
                //Pos.Y = _location.Y;

                if (_vTopLeft != null)
                {
                    _vTopLeft.X = _location.X;
                    _vTopLeft.Y = _location.Y;
                }
                if (_vTopRigth != null)
                {
                    _vTopRigth.X = _location.X + _size.Width;
                    _vTopRigth.Y = _location.Y;
                }
                if (_vBottomRight != null)
                {
                    _vBottomRight.X = _location.X + _size.Width;
                    _vBottomRight.Y = _location.Y + _size.Height;
                }
                if (_vBottomLeft != null)
                {
                    _vBottomLeft.X = _location.X;
                    _vBottomLeft.Y = _location.Y + _size.Height;
                }

                if (Vertices.Count > 0)
                {
                    AtualizarMinMax();
                }
            }
        }

        private Vetor2 _proxPosCtrl = new Vetor2(10, 10);

        /// <summary>
        /// Nova posição em cascata para controles adicionados com duplo clique
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public Vetor2 ProximoPosControle()
        {
            if (_proxPosCtrl.X + 50 > Container.Width) _proxPosCtrl.X = 10;
            if (_proxPosCtrl.Y + 50 > Container.Height) _proxPosCtrl.Y = 10;

            _proxPosCtrl.X += 20;
            _proxPosCtrl.Y += 20;

            return _proxPosCtrl;
        }

        #region Eventos
        public event EventHandler MouseClick;
        public event EventHandler MouseUp;
        public event EventHandler MouseDown;
        public event EventHandler MouseMove;

        public virtual void OnMouseClick(EventArgs e)
        {
            EventHandler handler = MouseClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnMouseUp(EventArgs e)
        {
            EventHandler handler = MouseUp;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnMouseDown(EventArgs e)
        {
            EventHandler handler = MouseDown;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public virtual void OnMouseMove(EventArgs e)
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
