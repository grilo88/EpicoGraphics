using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public class Eixos4 : Eixos
    {
        public Eixos4() => Dim = new float[4];
        public Eixos4(float W, float X, float Y, float Z) => Dim = new float[4] { X, Y, Z, W };

        public float X { get => Dim[0]; set => Dim[0] = value; }
        public float Y { get => Dim[1]; set => Dim[1] = value; }
        public float Z { get => Dim[2]; set => Dim[2] = value; }
        public float W { get => Dim[3]; set => Dim[3] = value; }

        public override Eixos NovaInstancia() => new Eixos4();

        public override Eixos NovaInstancia(ObjetoEpico epico)
        {
            throw new NotImplementedException();
        }
    }
}
