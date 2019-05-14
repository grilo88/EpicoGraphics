using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico.Sistema
{
    public class Centro2D
    {
        public float x;
        public float y;
        public bool sel;    // Selecionado

        public Centro2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Vetor2D
    {
        public float x;
        public float y;
        public bool sel;    // Selecionado

        public Vetor2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vetor2D operator +(Vetor2D v, Vetor2D vetor)
        {
            return new Vetor2D(v.x + vetor.x, v.y + vetor.y);
        }
    }

    public class Vertice2D
    {
        public float x;     
        public float y;     
        public float rad;   // Radiano
        public float raio;  // Raio
        public float ang;   // Ângulo
        public bool sel;    // Selecionado
    }
}
