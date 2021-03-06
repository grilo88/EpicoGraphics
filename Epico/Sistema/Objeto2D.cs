﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Epico.Sistema
{
    public class Objeto2D : ObjetoEpico
    {
        public object Tag { get; set; }

        public float Raio { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Layout")]
        public virtual Vetor2 Pos { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Layout")]
        public Vetor3 Angulo { get; set; }
        public Vetor2 Escala { get; set; }
        public List<Vertice2> Vertices { get; set; } = new List<Vertice2>();
        public List<Origem2> Origens { get; set; } = new List<Origem2>();
        public List<Pivo2> Pivos { get; set; } = new List<Pivo2>();
        public List<Vetor2> Arestas { get; set; } = new List<Vetor2>();
        public List<Animacao2D> Animacoes { get; set; } = new List<Animacao2D>();

        
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Extremidades")]
        public Vetor2 Min { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Extremidades")]
        public Vetor2 Max { get; set; }

        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //[Category("Extremidades")]
        //public Vetor2 GlobalMin {
        //    get
        //    {
        //        return new Vetor2(Pos.X + Min.X, Pos.Y + Min.Y);
        //    }
        //}

        //[TypeConverter(typeof(ExpandableObjectConverter))]
        //[Category("Extremidades")]
        //public Vetor2 GlobalMax {
        //    get
        //    {
        //        return new Vetor2(Pos.X + Max.X, Pos.Y + Max.Y);
        //    }
        //}

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

        public Objeto2D NovaInstancia() => new Objeto2D();

        public Objeto2D()
        {
            Pos = new Vetor2(this, 0, 0);
            Escala = new Vetor2(this, 1, 1);
            Angulo = new Vetor3(this);
            Max = new Vetor2(this);
            Min = new Vetor2(this);
            Origens.Add(new Origem2(this, 0, 0)); // Adiciona o ponto central principal
        }

        /// <summary>
        /// Posiciona o objeto na posição x
        /// </summary>
        /// <param name="x"></param>
        public void PosicionarX(float x) => Pos.X = x;

        /// <summary>
        /// Posiciona o objeto na posição y
        /// </summary>
        /// <param name="y"></param>
        public void PosicionarY(float y) => Pos.Y = y;

        public void AssociarEngine(EpicoGraphics engine)
        {
            _epico = engine;
        }

        public void DefinirAngulo(Eixos2 origem, Vetor3 novoAngulo)
        {
            Vetor3 graus = novoAngulo - Angulo;
            Angulo = novoAngulo;

            Vetor2 centro = new Vetor2();

            // Vértices
            for (int i = 0; i < Vertices.Count; i++)
            {
                Eixos2 eixo = Util2D.RotacionarPonto2D(origem, Vertices[i], graus.Z);
                Vertices[i].X = eixo.X;
                Vertices[i].Y = eixo.Y;

                Vertices[i].Raio = Util2D.Angulo2Radiano(Util2D.AnguloEntreDoisPontos(centro, Vertices[i]));
                Vertices[i].Rad = Util2D.Angulo2Radiano(Util2D.AnguloEntreDoisPontos(centro, Vertices[i]));
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

            AtualizarMinMax();
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
                Vertices[i].X = (float)Math.Cos(Vertices[i].Rad) * Vertices[i].Raio;
                Vertices[i].Y = (float)Math.Sin(Vertices[i].Rad) * Vertices[i].Raio;
            }
            Raio = raio;
            CriarArestasConvexa();
        }

        public void DefinirRaio(Vertice2 vertice, float raio)
        {
            if (vertice.Obj != this) throw new Exception("Vértice não associada a este objeto");

            vertice.X = (float)Math.Cos(vertice.Rad) * raio;
            vertice.Y = (float)Math.Sin(vertice.Rad) * raio;
            vertice.Raio = raio;
        }

        public void DefinirAngulo(Vertice2 vertice, float ang)
        {
            if (vertice.Obj != this) throw new Exception("Vértice não associada a este objeto");
            vertice.Ang = ang;
            vertice.X = (float)Math.Cos(vertice.Rad) * vertice.Raio;
            vertice.Y = (float)Math.Sin(vertice.Rad) * vertice.Raio;
        }

        public void AtualizarAngulo(Vertice2 vertice)
        {
            if (vertice.Obj != this) throw new Exception("Vértice não associada a este objeto");

            vertice.X = (float)Math.Cos(vertice.Rad) * vertice.Raio;
            vertice.Y = (float)Math.Sin(vertice.Rad) * vertice.Raio;
        }

        /// <summary>
        /// Atualiza o raio e o radiano da vértice quando sua posição é alterada
        /// </summary>
        /// <param name="vertice"></param>
        public void AtualizarRaio(Vertice2 vertice)
        {
            if (vertice.Obj != this) throw new Exception("Vértice não associada a este objeto");

            vertice.Raio = Util2D.DistanciaEntreDoisPontos(new Vetor2(), vertice);
            vertice.Rad = Util2D.Angulo2Radiano(Util2D.AnguloEntreDoisPontos(new Vetor2(), vertice));
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

        public void AtualizarMinMax()
        {
            Max.X = Vertices.Max(d => d.X);
            Min.Y = Vertices.Min(d => d.Y);
        }

        public void AdicionarVertice(Vertice2 vertice)
        {
            vertice.Raio = Util2D.DistanciaEntreDoisPontos(new Vetor2(0, 0), vertice);
            vertice.Rad = Util2D.Angulo2Radiano(Util2D.AnguloEntreDoisPontos(new Vetor2(0, 0), vertice));
            Vertices.Add(vertice);
            Raio = Vertices.Max(x => x.Raio);
            AtualizarMinMax();
        }

        /// <summary>
        /// Obtém o centro do polígono
        /// </summary>
        public Vetor2 Centro
        {
            get
            {
                Vetor2 centro = (Vetor2)Pos.NovaInstancia();
                centro.Obj = this;
                for (int v = 0; v < Vertices.Count; v++)
                {
                    centro.X += Vertices[v].X; // Soma os eixos das dimensões
                    centro.Y += Vertices[v].Y; // Soma os eixos das dimensões
                }

                // Depois divide a soma das dimensões pela quantidade de vértices
                centro.X /= Vertices.Count;
                centro.Y /= Vertices.Count;
                return centro;
            }
        }

        public void DefinirAngulo(Eixos2 origem, float angulo)
        {
            DefinirAngulo(origem, new Vetor3(0, 0, angulo));
        }

        /// <summary>
        /// Define o ângulo do objeto
        /// </summary>
        /// <param name="angulo"></param>
        public void DefinirAngulo(float angulo)
        {
            DefinirAngulo(Origens[0], new Vetor3(0, 0, angulo));
        }

        /// <summary>
        /// Cria arestas convexa para o objeto2D para fins diversos
        /// </summary>
        public void CriarArestasConvexa()
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

        public object Clone()
        {
            Objeto2D clone = (Objeto2D)MemberwiseClone();

            clone.Pos = new Vetor2(clone.Pos);
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
