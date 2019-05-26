using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Epico.Sistema
{
    public class Objeto2D :
        Poligono<Objeto2D, Vetor2, Vertice2, Origem2, Pivo2, Vetor3, Animacao2D>
    {
        public EpicoGraphics _epico;

        public override string Nome { get; set; } = "SemNome";
        public override float Raio { get; set; }
        public override bool Selecionado { get; set; }

        public override Vetor2 Pos { get; set; }
        public override Vetor2 Escala { get; set; }
        public override List<Vertice2> Vertices { get; set; } = new List<Vertice2>();
        public override List<Origem2> Origens { get; set; } = new List<Origem2>();
        public override List<Pivo2> Pivos { get; set; } = new List<Pivo2>();
        public override List<Vetor2> Arestas { get; set; } = new List<Vetor2>();
        public override List<Animacao2D> Animacoes { get; set; } = new List<Animacao2D>();

        public override Vetor3 Angulo { get; set; }
        public override Vetor2 Min { get; set; }
        public override Vetor2 Max { get; set; }
        public override Vetor2 GlobalMin {
            get
            {
                return new Vetor2(Pos.X + Min.X, Pos.Y + Min.Y);
            }
        }

        public override Vetor2 GlobalMax {
            get
            {
                return new Vetor2(Pos.X + Max.X, Pos.Y + Max.Y);
            }
        }

        [Category("Ordenação")]
        [Description("Ordenação de objetos no espaço 2D. Objetos com Ordem Z maiores são renderizados por último deixando-os na frente dos objetos na Ordem Z menores que ele.")]
        public int GlobalOrdemZ
        {
            get => _epico.objetos2D.FindIndex(x => x == this);
            set
            {
                // Reposiciona o objeto2D na nova ordemZ global
                int novoIndice = value;
                int indiceAtual = _epico.objetos2D.FindIndex(x => x == this);
                _epico.objetos2D.RemoveAt(indiceAtual);
                _epico.objetos2D.Insert(novoIndice, this);
            }
        }

        [Category("Ordenação")]
        [Description("Último índice de Ordem Z no espaço 2D.")]
        public int GlobalOrdemZMax { get => _epico.objetos2D.Count - 1; }
        

        public override Objeto2D NovaInstancia() => new Objeto2D();

        public Objeto2D()
        {
            Pos = new Vetor2(this, 0, 0);
            Escala = new Vetor2(this, 1, 1);
            Origens.Add(new Origem2(this, 0, 0)); // Adiciona o ponto central principal
        }

        /// <summary>
        /// Posiciona o objeto na posição x
        /// </summary>
        /// <param name="x"></param>
        public virtual void PosicionarX(float x) => Pos.X = x;

        /// <summary>
        /// Posiciona o objeto na posição y
        /// </summary>
        /// <param name="y"></param>
        public virtual void PosicionarY(float y) => Pos.Y = y;

        public override void AssociarEngine(EpicoGraphics engine)
        {
            _epico = engine;
        }

        public override void AtualizarGeometria(Vetor3 novoAngulo)
        {
            Vetor3 graus = novoAngulo - Angulo;
            Angulo = novoAngulo;

            // Vértices
            for (int i = 0; i < Vertices.Count; i++)
            {
                Eixos2 eixo = Util2D.RotacionarPonto2D(Origens[0], Vertices[i], graus.Z);
                Vertices[i].X = eixo.X;
                Vertices[i].Y = eixo.Y;
            }

            // Arestas
            if (Arestas.Count > 0)
            {
                for (int i = 0; i < Arestas.Count; i++)
                {
                    Eixos2 eixo = Util2D.RotacionarPonto2D(Centro, Arestas[i], graus.Z);
                    Arestas[i].X = eixo.X;
                    Arestas[i].Y = eixo.Y;
                }
            }

            base.AtualizarMinMax();
        }

        /// <summary>
        /// Define o raio do objeto, isto é, a distância do centro ao ponto máximo da circunferência.
        /// </summary>
        /// <param name="raio"></param>
        public void DefinirRaio(float raio)
        {
            int idx = IndiceRaioMax();

            float raioMax = Vertices[idx].Raio;
            float diff = raio - raioMax;

            // Define raios internos de cada vértice proporcionalmente ao novo raio do ponto máximo da circunferência
            float percentual = diff / raioMax * 100; // Percentual da proporção
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i].Raio += Vertices[i].Raio * percentual / 100;

            }
            Raio = raio;
            AtualizarGeometria(Angulo);
        }

        private int IndiceRaioMax()
        {
            int idxMax = 0;
            float v = float.MinValue;

            for (int i = 0; i < Vertices.Count(); i++)
            {
                if (Vertices[i].Raio > v)
                {
                    v = Vertices[i].Raio;
                    idxMax = i;
                }
            }
            return idxMax;
        }

        /// <summary>
        /// Define o ângulo do objeto
        /// </summary>
        /// <param name="angulo"></param>
        public virtual void DefinirAngulo(float angulo)
        {
            AtualizarGeometria(new Vetor3(0, 0, angulo));
        }

        /// <summary>
        /// Cria arestas convexa para o objeto2D para fins diversos
        /// </summary>
        public override void CriarArestasConvexa()
        {
            Vetor2 p1, p2;
            Arestas.Clear();
            for (int i = 0; i < Vertices.Count(); i++)
            {
                p1 = new Vetor2(this, Vertices[i].X, Vertices[i].Y);
                if (i + 1 >= Vertices.Count())
                {
                    p2 = new Vetor2(this, Vertices[0].X, Vertices[0].Y);
                }
                else
                {
                    p2 = new Vetor2(this, Vertices[i + 1].X, Vertices[i + 1].Y);
                }
                Arestas.Add(p2 - p1);
            }
        }

        public override object Clone()
        {
            Objeto2D clone = (Objeto2D)base.Clone();

            // Cria novas instâncias para os elementos
            clone.Vertices = Vertices
                .Select(v => new Vertice2(this, v.X, v.Y)
                {
                    Ang = v.Ang,
                    Nome = v.Nome,
                    Rad = v.Rad,
                    Raio = v.Raio,
                    Sel = v.Sel
                }).ToList();
            // Cria novas instâncias para os elementos
            clone.Origens = Origens
                .Select(x => new Origem2(this, x.X, x.Y)
                {
                    Sel = x.Sel
                }).ToList();

            return clone;
        }
    }
}
