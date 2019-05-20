using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace ColisaoPoligonos
{

    public class Poligono
    {
        public Color cor { get; set; }
        private List<Vetor> pontos = new List<Vetor>();
        private List<Vetor> arestas = new List<Vetor>();

        public void CriarArestas()
        {
            Vetor p1;
            Vetor p2;
            arestas.Clear();
            for (int i = 0; i < pontos.Count; i++)
            {
                p1 = pontos[i];
                if (i + 1 >= pontos.Count)
                {
                    p2 = pontos[0];
                }
                else
                {
                    p2 = pontos[i + 1];
                }
                arestas.Add(p2 - p1);
            }
        }

        public List<Vetor> Arestas
        {
            get { return arestas; }
        }

        public List<Vetor> Pontos
        {
            get { return pontos; }
        }

        public Vetor Centro
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < pontos.Count; i++)
                {
                    totalX += pontos[i].X;
                    totalY += pontos[i].Y;
                }

                return new Vetor(totalX / (float)pontos.Count, totalY / (float)pontos.Count);
            }
        }

        public void Posicao(Vetor v)
        {
            Posicao(v.X, v.Y);
        }

        public void Posicao(float x, float y)
        {
            for (int i = 0; i < pontos.Count; i++)
            {
                Vetor p = pontos[i];
                pontos[i] = new Vetor(p.X + x, p.Y + y);
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < pontos.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + pontos[i].ToString(true) + "}";
            }

            return result;
        }

    }
}

