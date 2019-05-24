using Epico;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Epico.Sistema
{
    public abstract class Geometria
    {
        public Eixos Posicao { get; set; }
        /// <summary>Vértices</summary>
        public abstract List<Eixos> Vertices { get; set; }
        /// <summary>Ponto(s) de origem</summary>
        public abstract List<Eixos> Origem { get; set; }
        /// <summary>Arestas</summary>
        public abstract List<Eixos> Arestas { get; set; }
    }
}
