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
    /// Abstração do polígono dimensional genérico
    /// </summary>
    //public abstract class ObjetoEpico_old : ICloneable
    //{
    //    public EpicoGraphics _epico;

    //    /// <summary>
    //    /// Quantidade de dimensões que esta geometria trabalha
    //    /// </summary>
    //    int _quant_dim => Pos.Dim.Length;

    //    public virtual string Nome { get; set; }
    //    public virtual float Raio { get; set; }
    //    public virtual bool Selecionado { get; set; }

    //    public virtual RGBA Cor { get; set; }

    //    [Category("Layout")]
    //    [TypeConverter(typeof(ExpandableObjectConverter))]
    //    protected T Pos { get; set; } 
    //    public virtual Escala { get; set; } 

    //    /// <summary>Vértices</summary>
    //    protected List<Eixos> Vertices { get; set; }
    //    /// <summary>Origens</summary>
    //    public virtual List<Eixos> Origens { get; set; }
    //    public virtual List<Eixos> Pivos { get; set; }
    //    /// <summary>Arestas</summary>
    //    public virtual List<Eixos> Arestas { get; set; }
    //    public virtual List<Eixos> Animacoes { get; set; }
    //    public virtual Eixos Angulo { get; set; }
    //    public virtual Eixos Min { get; set; }
    //    public virtual Eixos Max { get; set; }
    //    public virtual Eixos GlobalMin { get; }
    //    public virtual Eixos GlobalMax { get; }

    //    public virtual object NovaInstancia() => null;

    //    public virtual void AssociarEngine(EpicoGraphics epico) => _epico = epico;

    //    /// <summary>
    //    /// Obtém o centro do polígono
    //    /// </summary>
    //    public virtual Eixos Centro
    //    {
    //        get
    //        {
    //            Eixos centro = Pos.NovaInstancia();
    //            for (int v = 0; v < Vertices.Count; v++)
    //                for (int i = 0; i < _quant_dim; i++)
    //                {
    //                    centro.Dim[i] += Vertices[v].Dim[i]; // Soma os eixos das dimensões
    //                }

    //            for (int i = 0; i < _quant_dim; i++)
    //            {
    //                // Depois divide a soma das dimensões pela quantidade de vértices
    //                centro.Dim[i] /= Vertices.Count;
    //            }
    //            return centro;
    //        }
    //    }

    //    public virtual void AdicionarVertice(Eixos vertice)
    //    {
    //        Vertices.Add(vertice);
    //        AtualizarMinMax();
    //    }

    //    public virtual void AtualizarMinMax()
    //    {
    //        for (int i = 0; i < _quant_dim; i++)
    //        {
    //            Max.Dim[i] = Vertices.Max(d => d.Dim[i]);
    //            Min.Dim[i] = Vertices.Min(d => d.Dim[i]);
    //        }
    //    }

    //    public virtual void AtualizarGeometria(Eixos novoAngulo) { }

    //    public virtual void CriarArestasConvexa() { }

    //    public virtual void DefinirRaio(float raio) { }

    //    public virtual object Clone() => MemberwiseClone();
    //}
}
