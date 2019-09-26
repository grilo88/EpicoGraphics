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
        /// Coordenada X
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Coordenada Y
        /// </summary>
        public float Y { get; set; }

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
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }

        public T Somar<T>(T valor) where T : Eixos
        {
            X += valor.X;
            Y += valor.Y;
            return (T)this;
        }

        public T Subtrair<T>(T valor) where T : Eixos
        {
            X -= valor.X;
            Y -= valor.Y;
            return (T)this;
        }

        public T Multiplicar<T>(T valor) where T : Eixos
        {
            X *= valor.X;
            Y *= valor.Y;
            return (T)this;
        }

        public T Dividir<T>(T valor) where T : Eixos
        {
            X /= valor.X;
            X /= valor.Y;
            return (T)this;
        }

        public float Produto<T>(T valor) where T : Eixos
        {
            return X * valor.X + Y * valor.Y;
        }

        public T Normalizar<T>() where T : Eixos
        {
            float magnitude = Magnitude;
            X /= magnitude;
            Y /= magnitude;
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
            eixos.X = origem.X + distancia * (destino.X - origem.X);
            eixos.Y = origem.Y + distancia * (destino.Y - origem.Y);
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

            if (origem.X != destino.X ||
                origem.Y != destino.Y) transladando = true;

            if (transladando)
            {
                completado = true; // Antecipa o status completado
                if (Arredondar(origem.X) != Arredondar(destino.X) ||
                    Arredondar(origem.Y) != Arredondar(destino.Y))
                    completado = false; // Não está próximo o suficiente do ponto de destino
            }

            if (completado) // Define valores para as dimensões de origem
            {
                ret.X = destino.X;
                ret.Y = destino.Y;
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
