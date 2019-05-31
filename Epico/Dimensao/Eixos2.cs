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
        public Eixos2() => Dim = new float[2];
        public Eixos2(float X, float Y) => Dim = new float[2] { X, Y };
        public Eixos2(Eixos2 eixos)
        {
            Dim = new float[2];
            Obj = eixos.Obj;
            X = eixos.X;
            Y = eixos.Y;
            Nome = eixos.Nome;
            Tag = eixos.Tag;
        }
        public Eixos2(ObjetoEpico Obj)
        {
            this.Obj = Obj;
            Dim = new float[2];
        }
        public Eixos2(ObjetoEpico Obj, Eixos2 eixos)
        {
            base.Obj = Obj;
            Dim = new float[2] { eixos.Dim[0], eixos.Dim[1] };
        }
        public Eixos2(ObjetoEpico Obj, float X, float Y) {
            base.Obj = Obj;
            Dim = new float[2] { X, Y };
        }

        /// <summary>Coordenada X</summary>
        public float X { get => Dim[0]; set => Dim[0] = value; }
        /// <summary>Coordenada Y</summary>
        public float Y { get => Dim[1]; set => Dim[1] = value; }

        /// <summary>
        /// Posição global em relação ao espaço dimensional
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Eixos2 Global
        {
            get
            {
                Eixos2 ret = (Eixos2)NovaInstancia();
                ret.Dim[0] = ((Objeto2D)Obj).Pos.Dim[0] + Dim[0];
                ret.Dim[1] = ((Objeto2D)Obj).Pos.Dim[1] + Dim[1];
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
