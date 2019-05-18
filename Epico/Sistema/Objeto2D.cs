﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    /// <summary>
    /// Tipo abstrato que envolve tanto objetos visíveis e invisíveis do simulador
    /// </summary>
    public abstract class Objeto2D : ICloneable
    {
        #region Propriedades
        [Category("Design")]
        /// <summary>Id do objeto</summary>
        public int Id { get; set; }
        [Category("Design")]
        /// <summary>Nome do objeto</summary>
        public string Nome { get; set; }

        [Category("Geometria")]
        /// <summary>Distância do centro ao ponto máximo da circunferência.</summary>
        public float Raio { get; set; }
        [Category("Geometria")]
        /// <summary>Ângulo Z do objeto</summary>
        public virtual float Angulo { get; set; }
        [Category("Geometria")]
        /// <summary>Ângulo X do objeto</summary>
        public float AnguloX { get; set; }
        [Category("Geometria")]
        /// <summary>Ângulo Y do objeto</summary>
        public float AnguloY { get; set; }
        [Category("Extremidade")]
        /// <summary>Ponto X máximo do objeto</summary>
        public float XMax { get; set; }
        [Category("Extremidade")]
        /// <summary>Ponto X mínimo do objeto</summary>
        public float XMin { get; set; }
        [Category("Extremidade")]
        /// <summary>Ponto Y máximo do objeto</summary>
        public float YMax { get; set; }
        [Category("Extremidade")]
        /// <summary>Ponto Y mínimo do objeto</summary>
        public float YMin { get; set; }

        [Category("Extremidade")]
        public float GlobalXMax => Pos.X + XMax;
        [Category("Extremidade")]
        public float GlobalXMin => Pos.X + XMin;
        [Category("Extremidade")]
        public float GlobalYMax => Pos.Y + YMax;
        [Category("Extremidade")]
        public float GlobalYMin => Pos.Y + YMin;

        /// <summary>Cor de representação abstrata do objeto</summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Padrão")]
        public RGBA Cor { get; set; }

        [Category("Design")]
        /// <summary>Define se o objeto está selecionado em modo Editor</summary>
        public bool Selecionado { get; set; }

        
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Transformacao Transformação { get; set; }
        #endregion

        #region Campos
        /// <summary>Posição do objeto</summary>
        public Vetor2D Pos;
        /// <summary>Escala do objeto</summary>
        public Vetor2D Escala;

        private bool _otimizaAtualizaGeometria;
        private int _quantVertices;
        #endregion

        #region Arrays
        /// <summary>Ponto(s) de origem do objeto</summary>
        public List<Origem2D> Origem { get; set; } = new List<Origem2D>();
        /// <summary>Vértices do objeto</summary>
        public Vertice2D[] Vertices = new Vertice2D[0];
        [TypeConverter(typeof(ExpandableObjectConverter))]
        /// <summary>Animações do objeto</summary>
        public List<Animacao2D> Animacoes { get; set; } = new List<Animacao2D>();
        [TypeConverter(typeof(ExpandableObjectConverter))]
        /// <summary>Pivôs do objeto</summary>
        public List<Pivo2D> Pivos { get; set; } = new List<Pivo2D>();
        #endregion

        public Objeto2D()
        {
            Pos = new Vetor2D(this, 0, 0);
            Escala = new Vetor2D(this, 1, 1);
            Origem.Add(new Origem2D(this, 0, 0)); // Adiciona o ponto central principal
            Transformação = new Transformacao(this);
        }

        ///// <summary>
        ///// Atualiza fatores importantes e otimiza os cálculos geométricos durante uma atualização geométrica.
        ///// </summary>
        //public void BeginAtualizarGeometria()
        //{
        //    _otimizaAtualizaGeometria = true;
        //}

        public void EndAtualizarGeometria()
        {
            if (_quantVertices != Vertices.Length)
            {
                Array.Resize(ref Vertices, _quantVertices);
            }

            AtualizarGeometria(Angulo);
            AtualizarXYMinMax();
            //AtualizarRaio();

            _otimizaAtualizaGeometria = false;
        }

        /// <summary>
        /// Adiciona vértice ao objeto
        /// </summary>
        /// <param name="v"></param>
        public void AdicionarVertice(Vertice2D v)
        {
            //if (_otimizaAtualizaGeometria)
            //{
            //    if (Vertices.Length == _quantVertices)
            //    {
            //        Array.Resize(ref Vertices, Vertices.Length + 10); // Otimiza o redimensionamento
            //    }

            //    Vertices[Vertices.Length - 1] = v;
            //}
            //else
            //{
                Array.Resize(ref Vertices, Vertices.Length + 1);
                Vertices[Vertices.Length - 1] = v;
                _quantVertices = Vertices.Length;
                AtualizarXYMinMax();
            //}

            //_quantVertices = Vertices.Length;
        }

        public virtual void Animar(string nome)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move o objeto incrementando as posições x e y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void Mover(float x, float y)
        {
            Pos.X += x;
            Pos.Y += y;
        }

        /// <summary>
        /// Move o objeto incrementando as posições x e y
        /// </summary>
        /// <param name="pos"></param>
        public virtual void Mover(Vetor2D pos)
        {
            Pos.X += pos.X;
            Pos.Y += pos.Y;
        }

        /// <summary>
        /// Move o objeto incrementando a posição x
        /// </summary>
        /// <param name="x"></param>
        public virtual void MoverX(float x)
        {
            Pos.X = x;
        }

        /// <summary>
        /// Move o objeto incrementando a posição y
        /// </summary>
        /// <param name="y"></param>
        public virtual void MoverY(float y)
        {
            Pos.Y = y;
        }

        /// <summary>
        /// Posiciona o objeto na posição x e y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void Posicionar(float x, float y)
        {
            Pos.X = x;
            Pos.Y = y;
        }

        /// <summary>
        /// Posiciona o objeto na posição x e y
        /// </summary>
        /// <param name="pos"></param>
        public virtual void Posicionar(Vetor2D pos)
        {
            Pos.X = pos.X;
            Pos.Y = pos.Y;
        }

        /// <summary>
        /// Posiciona o objeto na posição x
        /// </summary>
        /// <param name="x"></param>
        public virtual void PosicionarX(float x)
        {
            Pos.X = x;
        }

        /// <summary>
        /// Posiciona o objeto na posição y
        /// </summary>
        /// <param name="y"></param>
        public virtual void PosicionarY(float y)
        {
            Pos.Y = y;
        }

        ///// <summary>
        ///// Gira o objeto
        ///// </summary>
        ///// <param name="graus"></param>
        //public virtual void Rotacionar(float graus)
        //{
        //    Angulo += graus;
        //    AtualizarGeometria();
        //}

        ///// <summary>
        ///// Gira o objeto no eixo x
        ///// </summary>
        ///// <param name="graus"></param>
        //public virtual void RotacionarX(float graus)
        //{
        //    AnguloX += graus;
        //    AtualizarGeometria();
        //}

        ///// <summary>
        ///// Gira o objeto no eixo y
        ///// </summary>
        ///// <param name="graus"></param>
        //public virtual void RotacionarY(float graus)
        //{
        //    AnguloY += graus;
        //    AtualizarGeometria();
        //}

        //public virtual void RotacionarVertice(Vertice2D v, int graus)
        //{
        //    AtualizarGeometria();
        //}

        /// <summary>
        /// Define o ângulo do objeto
        /// </summary>
        /// <param name="angulo"></param>
        public virtual void DefinirAngulo(float angulo)
        {
            AtualizarGeometria(angulo);
        }

        ///// <summary>
        ///// Define o ângulo do eixo x do objeto
        ///// </summary>
        ///// <param name="angulo"></param>
        //public virtual void DefinirAnguloX(float angulo)
        //{
        //    // TODO: DefinirAnguloY não testado 
        //    AnguloX = angulo;
        //    AtualizarGeometria();
        //}

        ///// <summary>
        ///// Define o ângulo do eixo y do objeto
        ///// </summary>
        ///// <param name="angulo"></param>
        //public virtual void DefinirAnguloY(float angulo)
        //{
        //    // TODO: DefinirAnguloY não testado 
        //    AnguloY = angulo;
        //    AtualizarGeometria();
        //}

        /// <summary>
        /// Define o raio do objeto, isto é, a distância do centro ao ponto máximo da circunferência.
        /// </summary>
        /// <param name="raio"></param>
        public virtual void DefinirRaio(float raio)
        {
            int idx = IndiceRaioMax();

            float raioMax = Vertices[idx].Raio;
            float diff = raio - raioMax;

            // Define novos raios internos de cada vértice proporcionalmente ao novo raio de ponto máximo da circunferência
            float percentual = diff / raioMax * 100; // Percentual da diferença
            for (int i = 0; i < Vertices.Length; i++)
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

            for (int i = 0; i < Vertices.Length; i++)
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
        /// Incrementa escala x e y do objeto
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void Escalar(float x, float y)
        {
            Escala.X += x;
            Escala.Y += y;
            AtualizarGeometria(Angulo);
        }

        /// <summary>
        /// Incrementa escala x do objeto
        /// </summary>
        /// <param name="x"></param>
        public virtual void EscalarX(float x)
        {
            Escala.X += x;
            AtualizarGeometria(Angulo);
        }

        /// <summary>
        /// Incrementa escala y do objeto
        /// </summary>
        /// <param name="y"></param>
        public virtual void EscalarY(float y)
        {
            Escala.Y += y;
            AtualizarGeometria(Angulo);
        }

        /// <summary>
        /// Define escala x e y do objeto
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void DefinirEscala(float x, float y)
        {
            Escala.X = x;
            Escala.Y = y;
            AtualizarGeometria(Angulo);
        }

        /// <summary>
        /// Define escala x do objeto
        /// </summary>
        /// <param name="x"></param>
        public virtual void DefinirEscalaX(float x)
        {
            Escala.X = x;
            AtualizarGeometria(Angulo);
        }

        /// <summary>
        /// Define escala y do objeto
        /// </summary>
        /// <param name="y"></param>
        public virtual void DefinirEscalaY(float y)
        {
            Escala.Y = y;
            AtualizarGeometria(Angulo);
        }

        /// <summary>Inverte o eixo x do objeto</summary>
        public virtual void InverterEixoX()
        {
            throw new NotImplementedException();
        }

        /// <summary>Inverte o eixo y do objeto</summary>
        public virtual void InverterEixoY()
        {
            throw new NotImplementedException();
        }

        /// <summary>Atualiza os pontos máximos e mínimos da geometria. Deve ser chamado após atualização das vértices.</summary>
        public void AtualizarXYMinMax()
        {
            XMax = Vertices.Max(x => x.X);
            XMin = Vertices.Min(x => x.X);
            YMax = Vertices.Max(x => x.Y);
            YMin = Vertices.Min(x => x.Y);
        }

        ///// <summary>Atualiza o raio obtendo o ponto máximo da circunferência. Quando há mudanças na variação de raios entre uma vértice e outra essa atualização é necessária. Deve ser chamado após atualização das vértices.</summary>
        //public float AtualizarRaio()
        //{
        //    Raio = Vertices.Max(x => x.Raio);
        //    return Raio;
        //}

        /// <summary>Atualiza todas as vértices considerando fatores como: radiano, raio, ângulo, escala, etc.</summary>
        public virtual void AtualizarGeometria(float novoAngulo)
        {
            float graus = novoAngulo - Angulo;
            Angulo = novoAngulo;
            for (int i = 0; i < Vertices.Length; i++)
            {
                EixoXY eixo = Util.RotacionarPonto2D(Origem[0], Vertices[i], graus);
                Vertices[i].X = eixo.X;
                Vertices[i].Y = eixo.Y;
            }

            AtualizarXYMinMax();
        }

        public void DefinirPosXCentro(float x)
        {
            Origem.First().X = x;
        }
        public void DefinirPosYCentro(float y)
        {
            Origem.First().Y = y;
        }

        public virtual object Clone()
        {
            object clone = MemberwiseClone();


            return clone;
        }
    }
}
