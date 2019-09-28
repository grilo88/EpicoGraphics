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
    public class Localizacao : INotifyPropertyChanged
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

        public Localizacao() { }
        public Localizacao(float x, float y)
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

    public class Tamanho : INotifyPropertyChanged
    {
        private float _largura;
        private float _altura;

        public float Largura { get => _largura; set
            {
                if (value != _largura) // Alterou?
                {
                    _largura = value; OnPropertyChanged();
                }
            }
        }
        public float Altura { get => _altura; set
            {
                if (value != _altura) // Alterou?
                {
                    _altura = value; OnPropertyChanged();
                }
            }
        }

        public Tamanho(float largura, float altura)
        {
            _largura = largura;
            _altura = altura;
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

        private Vertice2 _vSuperiorEsquerdo;            // Superior Esquerdo
        private Vertice2 _vSuperiorDireito;             // Superior Direito
        private Vertice2 _vInferiorDireito;             // Inferior Direito
        private Vertice2 _vInferiorEsquerdo;            // Inferior Esquerdo

        private Localizacao _localizacao = new Localizacao(0, 0);
        private Tamanho _tamanho = new Tamanho(300, 300);
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
        public Localizacao Localizacao
        {
            get => _localizacao; set
            {
                var val = value;
                _localizacao.X = val.X;
                _localizacao.Y = val.Y;
                AtualizarLayout();
            }
        }

        [Category("Layout")]
        [Description("O tamanho do controle em pixels")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Tamanho Tamanho
        {
            get => _tamanho; set
            {
                var val = value;
                _tamanho.Largura = val.Largura;
                _tamanho.Altura = val.Altura;
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


        private void Localizacao_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AtualizarLayout();
        }

        private void Tamanho_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            AtualizarLayout();
        }

        [Browsable(false)]
        [Category("Layout")]
        public float Top { get => Localizacao.Y; set => Localizacao.Y = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Left { get => Localizacao.X; set => Localizacao.X = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Largura { get => Tamanho.Largura; set => Tamanho.Largura = value; }
        [Browsable(false)]
        [Category("Layout")]
        public float Altura { get => Tamanho.Altura; set => Tamanho.Altura = value; }

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
            //InicializarTamanho();
            //InicializarLocalizacao();
        }

        private void InicializarLocalizacao()
        {
            // Define posição x e y do objeto 2d
            Localizacao = new Localizacao(_localizacao.X, _localizacao.Y);
            Localizacao.PropertyChanged += Localizacao_PropertyChanged;
        }

        private void InicializarTamanho()
        {
            Tamanho = new Tamanho(_tamanho.Largura, _tamanho.Altura);
            Tamanho.PropertyChanged += Tamanho_PropertyChanged;
        }

        protected virtual void GerarControle(float x, float y, float largura, float altura)
        {
            _localizacao.X = x;
            _localizacao.Y = y;
            _tamanho.Largura = largura;
            _tamanho.Altura = altura;

            InicializarLocalizacao();
            InicializarTamanho();

            AdicionarVertice(_vSuperiorEsquerdo = new Vertice2(this));
            AdicionarVertice(_vSuperiorDireito = new Vertice2(this));
            AdicionarVertice(_vInferiorDireito = new Vertice2(this));
            AdicionarVertice(_vInferiorEsquerdo = new Vertice2(this));

            Mat_render.CorSolida = new RGBA(200, 150, 80, 230);
            Mat_render.CorBorda = new RGBA(255, 255, 255, 255);
            Mat_render.LarguraBorda = 1;

            AtualizarLayout();

            // Centraliza o ponto de origem
            Origens[0].X = (_localizacao.X + _tamanho.Largura) / 2;
            Origens[0].Y = (_localizacao.Y + _tamanho.Altura) / 2;
        }

        private void AtualizarLayout()
        {
            if (_localizacao != null && _tamanho != null)
            {
                //Pos.X = _localizacao.X;
                //Pos.Y = _localizacao.Y;

                if (_vSuperiorEsquerdo != null)
                {
                    _vSuperiorEsquerdo.X = _localizacao.X;
                    _vSuperiorEsquerdo.Y = _localizacao.Y;
                }
                if (_vSuperiorDireito != null)
                {
                    _vSuperiorDireito.X = _localizacao.X + _tamanho.Largura;
                    _vSuperiorDireito.Y = _localizacao.Y;
                }
                if (_vInferiorDireito != null)
                {
                    _vInferiorDireito.X = _localizacao.X + _tamanho.Largura;
                    _vInferiorDireito.Y = _localizacao.Y + _tamanho.Altura;
                }
                if (_vInferiorEsquerdo != null)
                {
                    _vInferiorEsquerdo.X = _localizacao.X;
                    _vInferiorEsquerdo.Y = _localizacao.Y + _tamanho.Altura;
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
            if (_proxPosCtrl.X + 50 > Container.Largura) _proxPosCtrl.X = 10;
            if (_proxPosCtrl.Y + 50 > Container.Altura) _proxPosCtrl.Y = 10;

            _proxPosCtrl.X += 20;
            _proxPosCtrl.Y += 20;

            return _proxPosCtrl;
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
