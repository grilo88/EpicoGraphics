using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public class Eixos3 : Eixos
    {
        public Eixos3() => Dim = new float[3];
        public Eixos3(float X, float Y, float Z) => Dim = new float[3] { X, Y, Z };

        public Eixos3(Eixos3 eixos)
        {
            Obj = eixos.Obj;
            Dim = eixos.Dim;
            Nome = eixos.Nome;
            Tag = eixos.Tag;
        }
        public Eixos3(Geometria Obj)
        {
            this.Obj = Obj;
            Dim = new float[3];
        }
        public Eixos3(Geometria Obj, Eixos3 eixos)
        {
            base.Obj = Obj;
            Dim = new float[3] { eixos.Dim[0], eixos.Dim[1], eixos.Dim[2] };
        }
        public Eixos3(Geometria Obj, float X, float Y, float Z)
        {
            base.Obj = Obj;
            Dim = new float[3] { X, Y, Z };
        }

        public float X { get => Dim[0]; set => Dim[0] = value; }
        public float Y { get => Dim[1]; set => Dim[1] = value; }
        public float Z { get => Dim[2]; set => Dim[2] = value; }

        public override Eixos NovaInstancia() => new Eixos3();

        public static Eixos3 operator -(Eixos3 a, Eixos3 b)
        {
            Eixos3 ret = (Eixos3)a.NovaInstancia();
            ret.X = a.X - b.X;
            ret.Y = a.Y - b.Y;
            ret.Z = a.Z - b.Z;
            return ret;
        }
    }
}
