using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public abstract class Eixos
    {
        /// <summary>
        /// Objeto geométrico de dimensões genéricas do qual estes eixos estão associados
        /// </summary>
        public ObjetoEpico Obj { get; set; }
        public object Tag { get; set; }

        
        /// <summary>
        /// Nome do {Eixos}
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Selecionado
        /// </summary>
        public bool Sel { get; set; }

        /// <summary>
        /// Dimensões dos eixos
        /// </summary>
        internal float[] Dim { get; set; }

        /// <summary>
        /// Cria uma nova instância do Eixos
        /// </summary>
        /// <returns></returns>
        public abstract Eixos NovaInstancia();
        public abstract Eixos NovaInstancia(ObjetoEpico epico);

        public float Magnitude {
            get
            {
                float ret = 0;
                for (int i = 0; i < Dim.Length; i++) ret += Dim[i] * Dim[i];
                return (float)Math.Sqrt(ret);
            }
        }

        public T Somar<T>(T valor) where T : Eixos
        {
            for (int i = 0; i < Dim.Length; i++) Dim[i] += valor.Dim[i];
            return (T)this;
        }

        public T Subtrair<T>(T valor) where T : Eixos
        {
            for (int i = 0; i < Dim.Length; i++) Dim[i] -= valor.Dim[i];
            return (T)this;
        }

        public T Multiplicar<T>(T valor) where T : Eixos
        {
            for (int i = 0; i < Dim.Length; i++) Dim[i] *= valor.Dim[i];
            return (T)this;
        }

        public T Dividir<T>(T valor) where T : Eixos
        {
            for (int i = 0; i < Dim.Length; i++) Dim[i] /= valor.Dim[i];
            return (T)this;
        }

        public float Produto<T>(T valor) where T : Eixos
        {
            float prod = 0;
            for (int i = 0; i < Dim.Length; i++) prod += Dim[i] * valor.Dim[i];
            return prod;
        }

        public T Normalizar<T>() where T : Eixos
        {
            float magnitude = Magnitude;
            for (int i = 0; i < Dim.Length; i++) Dim[i] /= magnitude;
            return (T)this;
        }

        public static T Lerp<T>(T origem, T destino, float distancia) where T : Eixos
        {
            // a + f * (b - a);
            Eixos eixos = origem.NovaInstancia();
            for (int i = 0; i < origem.Dim.Length; i++)
            {
                eixos.Dim[i] = origem.Dim[i] + distancia * (destino.Dim[i] - origem.Dim[i]);
            }

            return (T)eixos;
        }
    }
}
