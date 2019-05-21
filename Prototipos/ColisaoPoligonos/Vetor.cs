using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ColisaoPoligonos
{

    public struct Vetor
    {

        public float X;
        public float Y;

        static public Vetor DoPonto(Point p)
        {
            return Vetor.DoPonto(p.X, p.Y);
        }

        static public Vetor DoPonto(int x, int y)
        {
            return new Vetor((float)x, (float)y);
        }

        public Vetor(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalizar()
        {
            float magnitude = Magnitude;
            X = X / magnitude;
            Y = Y / magnitude;
        }

        public Vetor GetNormalized()
        {
            float magnitude = Magnitude;

            return new Vetor(X / magnitude, Y / magnitude);
        }

        public float ProdutoPontual(Vetor vetor)
        {
            return this.X * vetor.X + this.Y * vetor.Y;
        }

        public float DistanciaAte(Vetor vetor)
        {
            return (float)Math.Sqrt(Math.Pow(vetor.X - this.X, 2) + Math.Pow(vetor.Y - this.Y, 2));
        }

        public static implicit operator Point(Vetor p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static implicit operator PointF(Vetor p)
        {
            return new PointF(p.X, p.Y);
        }

        public static Vetor operator +(Vetor a, Vetor b)
        {
            return new Vetor(a.X + b.X, a.Y + b.Y);
        }

        public static Vetor operator -(Vetor a)
        {
            return new Vetor(-a.X, -a.Y);
        }

        public static Vetor operator -(Vetor a, Vetor b)
        {
            return new Vetor(a.X - b.X, a.Y - b.Y);
        }

        public static Vetor operator *(Vetor a, float b)
        {
            return new Vetor(a.X * b, a.Y * b);
        }

        public static Vetor operator *(Vetor a, int b)
        {
            return new Vetor(a.X * b, a.Y * b);
        }

        public static Vetor operator *(Vetor a, double b)
        {
            return new Vetor((float)(a.X * b), (float)(a.Y * b));
        }

        public override bool Equals(object obj)
        {
            Vetor v = (Vetor)obj;

            return X == v.X && Y == v.Y;
        }

        public bool Equals(Vetor v)
        {
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vetor a, Vetor b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vetor a, Vetor b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        public string ToString(bool arredondado)
        {
            if (arredondado)
            {
                return (int)Math.Round(X) + ", " + (int)Math.Round(Y);
            }
            else
            {
                return ToString();
            }
        }
    }
}
