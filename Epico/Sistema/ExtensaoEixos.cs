using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public static class ExtensaoEixos
    {
        public static T Somar<T, T1>(this T a, T1 b) where T : Eixos where T1 : Eixos
        {
            for (int i = 0; i < a.Dim.Length; i++)
                a.Dim[i] += b.Dim[i];
            return a;
        }

        public static T Subtrair<T, T1>(this T a, T1 b) where T : Eixos where T1 : Eixos
        {
            for (int i = 0; i < a.Dim.Length; i++)
                a.Dim[i] -= b.Dim[i];
            return a;
        }

        public static T Multiplicar<T, T1>(this T a, T1 b) where T : Eixos where T1 : Eixos
        {
            for (int i = 0; i < a.Dim.Length; i++)
                a.Dim[i] *= b.Dim[i];
            return a;
        }

        public static T Dividir<T, T1>(this T a, T1 b) where T : Eixos where T1 : Eixos
        {
            for (int i = 0; i < a.Dim.Length; i++)
                a.Dim[i] /= b.Dim[i];
            return a;
        }

        public static float Produto<T, T1>(this T a, T1 b) where T : Eixos where T1 : Eixos
        {
            float prod = 0;
            for (int i = 0; i < a.Dim.Length; i++)
                prod += a.Dim[i] * b.Dim[i];
            return prod;
        }

        public static T Normalizar<T>(this T a) where T : Eixos
        {
            float magnitude = a.Magnitude;
            for (int i = 0; i < a.Dim.Length; i++)
                a.Dim[i] /= magnitude;
            return a;
        }
    }
}
