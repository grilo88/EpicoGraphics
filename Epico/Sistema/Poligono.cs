using Epico;
using Epico.Sistema2D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Epico.Sistema
{
    /// <summary>
    /// Abstração de geometria genérica
    /// </summary>
    /// <typeparam name="T">ObjetoEspacial</typeparam>
    /// <typeparam name="T1">Vetor</typeparam>
    /// <typeparam name="T2">Vértice</typeparam>
    /// <typeparam name="T3">Origem</typeparam>
    /// <typeparam name="T4">Pivô</typeparam>
    /// <typeparam name="T5">Vetor do Ângulo</typeparam>
    /// <typeparam name="T5">Animação</typeparam>
    public abstract class Poligono<T, T1, T2, T3, T4, T5, T6> : ICloneable
        where T1 : Eixos
        where T2 : Eixos
        where T3 : Eixos
        where T4 : Eixos
        where T5 : Eixos
        where T6 : Animacao2D
    {
        /// <summary>
        /// Quantidade de dimensões que esta geometria trabalha
        /// </summary>
        int _quant_dim => Pos.Dim.Length;

        public abstract string Nome { get; set; }
        public abstract float Raio { get; set; }
        public abstract bool Selecionado { get; set; }

        public RGBA Cor { get; set; }

        [Category("Layout")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual T1 Pos { get; set; }
        public abstract T1 Escala { get; set; }

        /// <summary>Vértices</summary>
        public abstract List<T2> Vertices { get; set; }
        /// <summary>Origens</summary>
        public abstract List<T3> Origens { get; set; }
        public abstract List<T4> Pivos { get; set; }
        /// <summary>Arestas</summary>
        public abstract List<T1> Arestas { get; set; }
        public abstract List<T6> Animacoes { get; set; }
        public abstract T5 Angulo { get; set; }
        public abstract T1 Min { get; set; }
        public abstract T1 Max { get; set; }
        public abstract T1 GlobalMin { get; }
        public abstract T1 GlobalMax { get; }

        public abstract T NovaInstancia();
        public abstract void AssociarEngine(EpicoGraphics engine);

        /// <summary>
        /// Obtém o centro do polígono
        /// </summary>
        public virtual T1 Centro
        {
            get
            {
                T1 centro = (T1)Pos.NovaInstancia();
                for (int v = 0; v < Vertices.Count; v++)
                    for (int i = 0; i < _quant_dim; i++)
                    {
                        centro.Dim[i] += Vertices[v].Dim[i]; // Soma os eixos
                    }

                for (int i = 0; i < _quant_dim; i++)
                {
                    // Depois divide a soma pela quantidade de vértices
                    centro.Dim[i] /= Vertices.Count;
                }
                return centro;
            }
        }

        public virtual void AdicionarVertice(T2 vertice)
        {
            Vertices.Add(vertice);
            AtualizarMinMax();
        }

        public virtual void AtualizarMinMax()
        {
            for (int i = 0; i < _quant_dim; i++)
            {
                Max.Dim[i] = Vertices.Max(x => x.Dim[i]);
                Min.Dim[i] = Vertices.Min(x => x.Dim[i]);
            }
        }

        public abstract void AtualizarGeometria(T5 novoAngulo);

        public abstract void CriarArestasConvexa();

        public virtual object Clone() => MemberwiseClone();
    }
}
