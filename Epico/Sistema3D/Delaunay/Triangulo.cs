///////////////////////////////////////////////////////////////////////////////
//
//  Triangle.cs
//
//  By Philip R. Braica (HoshiKata@aol.com, VeryMadSci@gmail.com)
//
//  Distributed under the The Code Project Open License (CPOL)
//  http://www.codeproject.com/info/cpol10.aspx
///////////////////////////////////////////////////////////////////////////////

    using Epico.Sistema3D;
    using System;
    using System.Collections.Generic;

#if EtoForms
using Eto.Drawing;
#else
using System.Drawing;
#endif

using System.Linq;
    using System.Text;

/// <summary>
/// Triângulo 3D
/// </summary>
public class Triangulo3D
{
    /// <summary>
    /// Reseta o índice, a indexação dá a cada triângulo um ID único desde a construção da malha.
    /// </summary>
    public static void ResetIndice() { m_indice = 0; }

#region Construcors: (), (Triangle src), (Vertice3D a, Vertice3D b, Vertice3D c)
    /// <summary>
    /// Construtor.
    /// </summary>
    public Triangulo3D()
    {
        Indice = m_indice;
        m_indice++;
    }

    /// <summary>
    /// Construtor de cópia.
    /// </summary>
    /// <param name="src"></param>
    public Triangulo3D(Triangulo3D src)
    {
        A = src.A;
        B = src.B;
        C = src.C;
        AB = src.AB;
        BC = src.BC;
        CA = src.CA;
        Indice = m_indice;
        m_indice++;
    }

    /// <summary>
    /// Construtor de Vertices 3D.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Triangulo3D(Vertice3D a, Vertice3D b, Vertice3D c)
    {
        A = a;
        B = b;
        C = c;
        Indice = m_indice;
        m_indice++;
    }
#endregion

#region Protected data.

    // Vertice3Des.
    protected Vertice3D m_a = null;
    protected Vertice3D m_b = null;
    protected Vertice3D m_c = null;

    // Comprimentos.
    protected float m_abLen = 0;
    protected float m_bcLen = 0;
    protected float m_caLen = 0;
    protected bool m_abLenCalcd = false;
    protected bool m_bcLenCalcd = false;
    protected bool m_caLenCalcd = false;

    // Determinações laterais.
    protected bool m_abDet = false;
    protected bool m_bcDet = false;
    protected bool m_caDet = false;
    protected bool m_abDetCalcd = false;
    protected bool m_bcDetCalcd = false;
    protected bool m_caDetCalcd = false;

    /// <summary>
    /// Índice deste triângulo para depuração.
    /// </summary>
    protected static int m_indice = 0;

    // Lados
    protected Triangulo3D m_ab = null;
    protected Triangulo3D m_bc = null;
    protected Triangulo3D m_ca = null;

    // Centro
    protected bool m_centroComputado = false;
    protected Vertice3D m_centro = null;

#endregion

    /// <summary>
    /// Índice.
    /// </summary>
    public int Indice { get; protected set; }

    /// <summary>
    /// Em qual região de pesquisa está.
    /// </summary>
    public int CodigoRegiao { get; set; }

    /// <summary>
    /// Para string.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Indice + ": " + A.ToString() + " => " + B.ToString() + " => " + C.ToString();
    }

    /// <summary>
    /// Computa o centro
    /// </summary>
    public Vertice3D Center
    {
        get
        {
            if (m_centroComputado) return m_centro;
            m_centro = new Vertice3D(
                (A.X + B.X + C.X) / 3f,
                (A.Y + B.Y + C.Y) / 3f,
                (A.Z + B.Z + C.Z) / 3f);

            float delta = m_centro.QuadradoDeltaXY(A);
            float tmp = m_centro.QuadradoDeltaXY(B);
            delta = delta > tmp ? delta : tmp;
            tmp = m_centro.QuadradoDeltaXY(C);
            delta = delta > tmp ? delta : tmp;
            MaisDitanteDoCentro = delta;
            m_centroComputado = true;

            return m_centro;
        }
    }

    /// <summary>
    /// A maior distância que um ponto está do centro é a distância ao quadrado.
    /// </summary>
    public float MaisDitanteDoCentro { get; protected set; }

    /// <summary>
    /// Vertice3D A
    /// </summary>
    public Vertice3D A
    {
        get { return m_a; }
        set
        {
            if (m_a == value) return;
            m_abDetCalcd = false;
            m_caDetCalcd = false;
            m_abLenCalcd = false;
            m_caLenCalcd = false;
            m_centroComputado = false;
            m_a = value;
        }
    }

    /// <summary>
    /// Vertice3D B
    /// </summary>
    public Vertice3D B
    {
        get { return m_b; }
        set
        {
            if (m_b == value) return;
            m_abDetCalcd = false;
            m_bcDetCalcd = false;
            m_abLenCalcd = false;
            m_bcLenCalcd = false;
            m_centroComputado = false;
            m_b = value;
        }
    }

    /// <summary>
    /// Vertice3D C
    /// </summary>
    public Vertice3D C
    {
        get { return m_c; }
        set
        {
            if (m_c == value) return;
            m_caDetCalcd = false;
            m_bcDetCalcd = false;
            m_caLenCalcd = false;
            m_bcLenCalcd = false;
            m_centroComputado = false;
            m_c = value;
        }
    }

    /// <summary>
    /// Triângulo AB ação ao lado de AB.
    /// </summary>
    public Triangulo3D AB { get { return m_ab; } set { m_ab = value; } }

    /// <summary>
    /// Triângulo BC ação ao lado de BC.
    /// </summary>
    public Triangulo3D BC { get { return m_bc; } set { m_bc = value; } }

    /// <summary>
    /// Triângulo CA ação ao lado de CA.
    /// </summary>
    public Triangulo3D CA { get { return m_ca; } set { m_ca = value; } }


    /// <summary>
    /// AB det.
    /// </summary>
    protected bool abDet
    {
        get
        {
            if (!m_abDetCalcd)
            {
                m_abDet = Vertice3DTeste(A, B, C);
            }
            return m_abDet;
        }
    }

    /// <summary>
    /// BC det.
    /// </summary>
    protected bool bcDet
    {
        get
        {
            if (!m_bcDetCalcd)
            {
                m_bcDet = Vertice3DTeste(B, C, A);
            }
            return m_bcDet;
        }
    }

    /// <summary>
    /// CA det.
    /// </summary>
    protected bool caDet
    {
        get
        {
            if (!m_caDetCalcd)
            {
                m_caDet = Vertice3DTeste(C, A, B);
            }
            return m_caDet;
        }
    }

    /// <summary>
    /// Teste de lateralidade Vertice3D.
    /// </summary>
    /// <param name="la"></param>
    /// <param name="lb"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    protected bool Vertice3DTeste(Vertice3D la, Vertice3D lb, Vertice3D t)
    {
        // y = mx + b
        if (la.X == lb.X)
        {
            // Vertical at X.
            return t.X > la.X;
        }
        if (la.Y == lb.Y)
        {
            return t.Y > la.Y;
        }
        float m = (la.Y - lb.Y) / (la.X - lb.X);
        float b = la.Y - (m * la.X);
        return (m * t.X + b - t.Y) > 0;
    }

    /// <summary>
    /// Isso contém em t?
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool Contains(Vertice3D t)
    {
        float delta = t.QuadradoDeltaXY(Center);
        if (delta > MaisDitanteDoCentro) return false;
        if (abDet != Vertice3DTeste(A, B, t)) return false;
        if (bcDet != Vertice3DTeste(B, C, t)) return false;
        if (caDet != Vertice3DTeste(C, A, t)) return false;
        return true;
    }

    /// <summary>
    /// Comprimento de AB, em cache e preguiçoso calculado.
    /// </summary>
    public float AB_Comp
    {
        get
        {
            if (m_abLenCalcd == true)
            {
                return m_abLen;
            }
            if ((A == null) || (B == null)) return -1;
            m_abLen = A.QuadradoDeltaXY(B);
            m_abLenCalcd = true;
            return m_abLen;
        }
    }

    /// <summary>
    /// Comprimento de BC, em cache e preguiçoso calculado.
    /// </summary>
    public float BC_Comp
    {
        get
        {
            if (m_bcLenCalcd == true)
            {
                return m_bcLen;
            }
            if ((B == null) || (C == null)) return -1;
            m_bcLen = B.QuadradoDeltaXY(C);
            m_bcLenCalcd = true;
            return m_bcLen;
        }
    }

    /// <summary>
    /// Comprimento de CA, em cache e preguiçoso calculado.
    /// </summary>
    public float CA_Comp
    {
        get
        {
            if (m_caLenCalcd == true)
            {
                return m_caLen;
            }
            if ((C == null) || (A == null)) return -1;
            m_caLen = C.QuadradoDeltaXY(A);
            m_caLenCalcd = true;
            return m_caLen;
        }
    }

    /// <summary>
    /// Áres do Triângulo
    /// </summary>
    public float Area
    {
        get
        {
            float a = AB_Comp;
            float b = BC_Comp;
            float c = CA_Comp;
            a = (float)System.Math.Sqrt(a);
            b = (float)System.Math.Sqrt(b);
            c = (float)System.Math.Sqrt(c);

            // Fórmula de garces
            float s = 0.5f * (a + b + c);
            return (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

    /// <summary>
    /// Retorna o índice do comprimento da aresta;
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public float Aresta_Comprimento(int i)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        return i == 0 ? AB_Comp : i == 1 ? BC_Comp : CA_Comp;
    }

    /// <summary>
    /// Retorna o oposto da aresta.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Vertice3D OpostoDaAresta(int i)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        return i == 0 ? C : i == 1 ? A : B;
    }

    /// <summary>
    /// Defina o Vertice3D por índice.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="v"></param>
    public void DefinaVertice3D(int i, Vertice3D v)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        if (i == 0) A = v;
        if (i == 1) B = v;
        if (i == 2) C = v;
    }

    /// <summary>
    /// Obtenha o ângulo de coseno associado a um Vertice3D.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public float Vertice3DCoseno2Angulo(int i)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        float dx1 = 0;
        float dx2 = 0;
        float dy1 = 0;
        float dy2 = 0;
        if (i == 0)
        {
            dx1 = B.X - A.X;
            dy1 = B.Y - A.Y;
            dx2 = C.X - A.X;
            dy2 = C.Y - A.Y;
        }
        else
        {
            if (i == 1)
            {
                dx1 = C.X - B.X;
                dy1 = C.Y - B.Y;
                dx2 = A.X - B.X;
                dy2 = A.Y - B.Y;
            }
            else
            {
                dx1 = A.X - C.X;
                dy1 = A.Y - C.Y;
                dx2 = B.X - C.X;
                dy2 = B.Y - C.Y;
            }
        }
        float mag1 = (dx1 * dx1) + (dy1 * dy1);
        float mag2 = (dx2 * dx2) + (dy2 * dy2);
        float mag = (float)System.Math.Sqrt(mag1 * mag2);
        float dot = (float)((dx1 * dx2) + (dy1 * dy2)) / mag;

        // dot is 0 to 1 result of the cosine.
        return dot;
    }


    /// <summary>
    /// Obtenha o ângulo de um Vertice3D em radianos.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public float Vertice3DAngulo2Radiano(int i)
    {
        return (float)Math.Acos(Vertice3DCoseno2Angulo(i));
    }

    /// <summary>
    /// É este retângulo dentro da região.
    /// </summary>
    /// <param name="regiao"></param>
    /// <returns></returns>
    public bool Dentro(RectangleF regiao)
    {
        if (!A.DentroXY(regiao)) return false;
        if (!B.DentroXY(regiao)) return false;
        if (!C.DentroXY(regiao)) return false;
        return true;
    }

    /// <summary>
    /// Repare quaisquer ligações de aresta, nos dois sentidos.
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public void RepararArestas(Triangulo3D a)
    {
        // Check if a.AB is in this.
        if (this.Indice == a.Indice) return;
        if (AmbosEm(a, a.A, a.B)) { a.AB = this; return; }
        if (AmbosEm(a, a.B, a.C)) { a.BC = this; return; }
        if (AmbosEm(a, a.C, a.A)) { a.CA = this; return; }
    }

    /// <summary>
    /// Ambos estão em Vertice3Des?
    /// </summary>
    /// <param name="t"></param>
    /// <param name="vt"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected bool AmbosEm(Triangulo3D t, Vertice3D a, Vertice3D b)
    {
        if (a == A)
        {
            if (b == B) { AB = t; return true; }
            if (b == C) { CA = t; return true; }
        }
        if (a == B)
        {
            if (b == A) { AB = t; return true; }
            if (b == C) { BC = t; return true; }
        }
        if (a == C)
        {
            if (b == A) { CA = t; return true; }
            if (b == B) { BC = t; return true; }
        }
        return false;
    }

    /// <summary>
    /// Vertice3D.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Vertice3D GetVertice3D(int i)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        if (i == 0) return A;
        if (i == 1) return B;
        return C;
    }

    /// <summary>
    /// Defina a aresta pelo índice.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="t"></param>
    public void DefinirAresta(int i, Triangulo3D t)
    {
        i = i < 0 ? i + 3 : i > 2 ? i - 3 : i;
        if (i == 0) AB = t;
        if (i == 1) BC = t;
        if (i == 2) CA = t;
    }

    /// <summary>
    /// Obtém o índice da aresta.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Triangulo3D Aresta(int i)
    {
        return i == 0 ? AB : i == 1 ? BC : CA;
    }
}
