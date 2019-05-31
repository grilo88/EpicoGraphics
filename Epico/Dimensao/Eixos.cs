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

        public float Magnitude
        {
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

        public static float Lerp(float origem, float destino, float distancia)
        {
            return Lerp(new Vetor2(origem, 0), new Vetor2(destino, 0), distancia).X;
        }

        public static float Lerp(float origem, float destino, float distancia, out bool completado)
        {
            return Lerp(new Vetor2(origem, 0), new Vetor2(destino, 0), distancia, out completado).X;
        }

        /// <summary>
        /// Interpolação Linear
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origem"></param>
        /// <param name="destino"></param>
        /// <param name="distancia"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Interpolação Linear
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origem"></param>
        /// <param name="destino"></param>
        /// <param name="distancia"></param>
        /// <param name="completado"></param>
        /// <returns></returns>
        public static T Lerp<T>(T origem, T destino, float distancia, out bool completado) where T : Eixos
        {
            completado = false;
            var ret = Lerp<T>(origem, destino, distancia);

            bool transladando = false;
            for (int i = 0; i < origem.Dim.Length; i++)
                if (origem.Dim[i] != destino.Dim[i]) transladando = true;

            if (transladando)
            {
                completado = true; // Antecipa o status completado
                for (int i = 0; i < origem.Dim.Length; i++)
                {
                    if (Arredondar(origem.Dim[i]) != Arredondar(destino.Dim[i]))
                        completado = false; // Não está próximo o suficiente do ponto de destino
                }
            }

            if (completado) // Define valores para as dimensões de origem
            {
                for (int i = 0; i < origem.Dim.Length; i++)
                    ret.Dim[i] = destino.Dim[i];
            }

            return ret;
        }

        public static float Arredondar(float valor, int casasDecimais = 3)
        {
            float mult = 1;
            for (int i = 0; i < casasDecimais; i++) mult *= 0.1F;
            float ret = (int)(valor / mult + 0.1F);
            ret *= mult;
            return ret;
        }

        public static float Grade(float valor, float tamanhoGrade = 2.5F)
        {
            valor = (int)(valor / tamanhoGrade + 0.1F);
            valor *= tamanhoGrade;
            return valor;
        }
    }
}
