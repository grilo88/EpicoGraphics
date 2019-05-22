using EpicoGraphics.Sistema3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace EpicoGraphics
{
    public static class Util3D
    {
        public static float Angulo2Radiano(this float angulo)
        {
            return angulo * (float)Math.PI / 180;
        }

        public static EixoXYZ RotacionarPonto3D(float origemX, float origemY, float origemZ, float x, float y, float z, float angulo)
        {
            float rad = Angulo2Radiano(angulo);
            float rotX = (float)(Math.Cos(rad) * (x - origemX) + Math.Sin(rad) * (y - origemY) + origemX);
            float rotY = (float)(Math.Cos(rad) * (x - origemX) + Math.Sin(rad) * (y - origemY) + origemY);
            float rotZ = (float)(Math.Cos(rad) * (z - origemZ) + Math.Sin(rad) * (z - origemZ) + origemZ);
            return new XYZ(rotX, rotY, rotZ);
        }
    }
}
