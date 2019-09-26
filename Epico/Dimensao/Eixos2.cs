using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public abstract class Eixos2 : Eixos
    {
        public Eixos2() : base() { }

        public Eixos2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Eixos2(Eixos2 eixos)
        {
            Obj = eixos.Obj;
            this.X = eixos.X;
            this.Y = eixos.Y;
            Nome = eixos.Nome;
            Tag = eixos.Tag;
        }
        public Eixos2(ObjetoEpico Obj)
        {
            this.Obj = Obj;
        }
        public Eixos2(ObjetoEpico Obj, Eixos2 eixos)
        {
            base.Obj = Obj;
            this.X = eixos.X;
            this.Y = eixos.Y;
        }
        public Eixos2(ObjetoEpico Obj, float X, float Y) {
            base.Obj = Obj;

            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Posição global em relação ao espaço
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Eixos2 Global
        {
            get
            {
                Eixos2 ret = (Eixos2)NovaInstancia();
                ret.X = ((Objeto2D)Obj).Pos.X + X;
                ret.Y = ((Objeto2D)Obj).Pos.Y + Y;
                return ret;
            }
        }

        public static Eixos2 operator +(Eixos2 a, Vertice2 b)
        {
            Vertice2 ret = new Vertice2(b.Obj);
            ret.X = a.X + b.X;
            ret.Y = a.Y + a.Y;
            return ret;
        }

        public static Eixos2 operator +(Eixos2 a, Eixos2 b)
        {
            Eixos2 ret = (Eixos2)a.NovaInstancia();
            ret.X = a.X + b.X;
            ret.Y = a.Y + b.Y;
            return ret;
        }

        public static Eixos2 operator -(Eixos2 a, Eixos2 b)
        {
            Vetor2 ret = new Vetor2();
            ret.X = a.X - b.X;
            ret.Y = a.Y - b.Y;
            return ret;
        }

        public static Eixos2 operator *(Eixos2 a, float b)
        {
            Vetor2 ret = (Vetor2)a.NovaInstancia();
            ret.X = a.X * b;
            ret.Y = a.Y * b;
            return ret;
        }
    }
}
