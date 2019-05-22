using Epico.Sistema3D;
using System;
using System.Collections.Generic;
using System.Text;

namespace Epico
{
    public static class Util3D
    {
        public static float Angulo2Radiano(this float angulo)
        {
            return angulo * (float)Math.PI / 180;
        }

        public static EixoXYZ RotacionarPonto3D(EixoXYZ origem, EixoXYZ ponto, float graus) => RotacionarPonto3D(origem.X, origem.Y, origem.Z, ponto.X, ponto.Y, ponto.Z, graus);

        public static EixoXYZ RotacionarPonto3D(float origemX, float origemY, float origemZ, float x, float y, float z, float angulo)
        {
            float rad = Angulo2Radiano(angulo);
            float rotX = (float)(Math.Cos(rad) * (x - origemX) + Math.Sin(rad) * (y - origemY) + origemX);
            float rotY = (float)(Math.Cos(rad) * (x - origemX) + Math.Sin(rad) * (y - origemY) + origemY);
            float rotZ = (float)(Math.Cos(rad) * (z - origemZ) + Math.Sin(rad) * (z - origemZ) + origemZ);
            return new XYZ(rotX, rotY, rotZ);
        }

        public static T EulerRotacionarX<T>(this T vetor, EixoXYZ pivo, float graus) where T : EixoXYZ
        {
            return EulerRotacionarX((T)vetor.Subtrair(pivo), graus);
        }

        public static T EulerRotacionarY<T>(this T vetor, EixoXYZ pivo, float graus) where T : EixoXYZ
        {
            return EulerRotacionarY((T)vetor.Subtrair(pivo), graus);
        }

        public static T EulerRotacionarZ<T>(this T vetor, EixoXYZ pivo, float graus) where T : EixoXYZ
        {
            return EulerRotacionarZ((T)vetor.Subtrair(pivo), graus);
        }

        public static T EulerRotacionarX<T>(this T vetor, float graus) where T : EixoXYZ
        {
            // https://pt.wikipedia.org/wiki/%C3%82ngulos_de_Euler
            float rad = Angulo2Radiano(graus);
            float rotY = vetor.Y * (float)Math.Cos(rad) + vetor.Z * (float)Math.Sin(rad);
            float rotZ = vetor.Y * -(float)Math.Sin(rad) + vetor.Z * (float)Math.Cos(rad);
            vetor.Y = rotY;
            vetor.Z = rotZ;
            return vetor;
        }

        public static T EulerRotacionarY<T>(this T vetor, float graus) where T : EixoXYZ
        {
            // https://pt.wikipedia.org/wiki/%C3%82ngulos_de_Euler
            float rad = Angulo2Radiano(graus);
            float rotX = vetor.X * (float)Math.Cos(rad) + vetor.Z * (float)Math.Sin(rad);
            float rotZ = vetor.X * -(float)Math.Sin(rad) + vetor.Z * (float)Math.Cos(rad);
            vetor.X = rotX;
            vetor.Z = rotZ;
            return vetor;
        }

        public static T EulerRotacionarZ<T>(this T vetor, float graus) where T : EixoXYZ
        {
            // https://pt.wikipedia.org/wiki/%C3%82ngulos_de_Euler
            float rad = Angulo2Radiano(graus);
            float rotX = vetor.X * (float)Math.Cos(rad) + vetor.Y * (float)Math.Sin(rad);
            float rotY = vetor.X * -(float)Math.Sin(rad) + vetor.Y * (float)Math.Cos(rad);
            vetor.X = rotX;
            vetor.Y = rotY;
            return vetor;
        }
    }
}
