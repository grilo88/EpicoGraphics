using Epico.Sistema2D;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epico.Sistema;

#if Editor2D || NetStandard2 || NetCore
using System.Drawing;
#elif EtoForms
using Eto.Drawing;
#endif

namespace Epico
{
    public static class Util2D
    {
        public static float DistanciaEntreDoisPontos(this Eixos2 pontoA, Eixos2 pontoB) => 
            DistanciaEntreDoisPontos(pontoA.X, pontoA.Y, pontoB.X, pontoB.Y);
        public static float AnguloEntreDoisPontos(this Eixos2 pontoA, Eixos2 pontoB) => 
            AnguloEntreDoisPontos(pontoA.X, pontoA.Y, pontoB.X, pontoB.Y);

        public static float DistanciaEntreDoisPontos(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
        public static float AnguloEntreDoisPontos(float x1, float y1, float x2, float y2)
        {
            return (float)(Math.Atan2(y2 - y1, x2 - x1) * 180 / Math.PI);
        }
        public static float Angulo2Radiano(this float angulo)
        {
            return angulo * (float)Math.PI / 180;
        }

        public static float Radiano2Angulo(this float radiano)
        {
            return radiano * (180 / (float)Math.PI);
        }

        /// <summary>
        /// Rotaciona um ponto 2D a partir de um ponto de origem
        /// </summary>
        /// <param name="origem">Ponto de origem do objeto</param>
        /// <param name="p">Ponto a ser rotacionado</param>
        /// <param name="graus">Diferença de ângulos no caso de rotação ou soma dos ângulo no caso de acoplamento de objetos.</param>
        /// <returns></returns>
        public static Eixos2 RotacionarPonto2D(Eixos2 origem, Eixos2 ponto, float graus) => 
            RotacionarPonto2D(origem.X, origem.Y, ponto.X, ponto.Y, graus);

        /// <summary>
        /// Rotaciona um ponto 2D a partir de um ponto de origem
        /// </summary>
        /// <param name="origem">Ponto de origem do objeto</param>
        /// <param name="p">Ponto a ser rotacionado</param>
        /// <param name="angulo">Diferença de ângulos no caso de rotação ou soma dos ângulo no caso de acoplamento de objetos.</param>
        /// <returns></returns>
        public static Eixos2 RotacionarPonto2D(float origemX, float origemY, float x, float y, float angulo)
        {
            float rad = Angulo2Radiano(angulo);
            float rotX = (float)(Math.Cos(rad) * (x - origemX) - Math.Sin(rad) * (y - origemY) + origemX);
            float rotY = (float)(Math.Sin(rad) * (x - origemX) + Math.Cos(rad) * (y - origemY) + origemY);
            return new Vetor2(rotX, rotY);
        }

        /// <summary>
        /// Obtém a posição no espaço 2D através da coordenada x e y da tela
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="mouseXY"></param>
        /// <returns>Retorna ponto 2d pela coordenada X e Y da tela</returns>
        public static Eixos2 ObterPosEspaco2DMouseXY(
            this Camera2D cam, Eixos2 mouseXY)
        {
            Vetor2 PosCamZoomDiff = new Vetor2();
            PosCamZoomDiff = cam.Pos * cam.ZoomCamera - cam.Pos;

            float x = cam.Left + mouseXY.X + PosCamZoomDiff.X;
            float y = cam.Top + mouseXY.Y + PosCamZoomDiff.Y;

            Vetor2 ponto = new Vetor2(x / cam.ZoomCamera, y / cam.ZoomCamera); // Reduz escala para tamanho real em 2D
            Eixos2 ponto2D = Util2D.RotacionarPonto2D(cam.Pos, ponto, cam.Angulo.Z); // Rotaciona ponto no tamanho real
            return ponto2D;
        }

        public static PointF ObterPontoTelaPeloEspaco2D(this Camera2D cam, Eixos2 pos2D)
        {
            Vetor2 PosCam = new Vetor2(cam.Pos);
            Vetor2 PosCamZoomDiff = new Vetor2();
            PosCamZoomDiff = cam.Pos * cam.ZoomCamera - cam.Pos;

            Vetor2 globalPos = (Vetor2)pos2D * cam.ZoomCamera;
            Eixos2 rot = Util2D.RotacionarPonto2D(PosCam * cam.ZoomCamera, globalPos, -cam.Angulo.Z);

            PointF pontoTela = new PointF();
            pontoTela.X = -cam.Left - PosCamZoomDiff.X + rot.X;
            pontoTela.Y = -cam.Top - PosCamZoomDiff.Y + rot.Y;

            return pontoTela;
        }

        ///// <summary>
        ///// Obtém objeto 2d através do espaço global.
        ///// </summary>
        ///// <param name="ponto"></param>
        ///// <returns></returns>
        //public static Objeto2D ObterObjeto2DPeloEspaco(this EpicoGraphics engine, Vetor2 ponto)
        //{
        //    for (int i = 0; i < engine.objetos2D.Count; i++)
        //    {
        //        Objeto2D obj = engine.objetos2D[i];

        //        float xMax = obj.Pos.X + obj.Max.X;
        //        float xMin = obj.Pos.X + obj.Min.X;
        //        float yMax = obj.Pos.Y + obj.Max.Y;
        //        float yMin = obj.Pos.Y + obj.Min.Y;

        //        if (ponto.X >= xMin && ponto.X <= xMax)
        //            if (ponto.Y >= yMin && ponto.Y <= yMax)
        //            {
        //                return engine.objetos2D[i];
        //            }
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Obtém objeto 2d através da tela. X = 0 a Largura da camera, Y = 0 ao tamanho da camera.
        ///// </summary>
        ///// <param name="ponto"></param>
        ///// <returns></returns>
        //public static Objeto2D ObterObjeto2DPelaTela(this EpicoGraphics engine, Camera2D camera, Eixos2 ponto)
        //{
        //    for (int i = 0; i < engine.objetos2D.Count; i++)
        //    {
        //        Objeto2D obj = engine.objetos2D[i];

        //        float xMaxTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.Max.X;
        //        float xMinTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.Min.X;
        //        float yMaxTela = -(camera.Pos.Y - camera.ResHeight / 2) + obj.Pos.Y + obj.Max.Y;
        //        float yMinTela = -(camera.Pos.Y - camera.ResHeight / 2) + obj.Pos.Y + obj.Min.Y;

        //        if (ponto.X >= xMinTela && ponto.X <= xMaxTela)
        //            if (ponto.Y >= yMinTela && ponto.Y <= yMaxTela)
        //            {
        //                return engine.objetos2D[i];
        //            }
        //    }
        //    return null;
        //}

        public static IEnumerable<Objeto2D> ObterObjetos2DMouseXY(
            this EpicoGraphics engine, Camera2D camera, Eixos2 ponto)
        {

            return ObterObjetos2DMouseXY(engine, camera, new Vertice2(ponto.X, ponto.Y));
        }

        public static Objeto2D ObterUnicoObjeto2DMouseXY(this EpicoGraphics engine, Camera2D camera, Eixos2 ponto)
        {
            return ObterObjetos2DMouseXY(engine, camera, ponto).LastOrDefault();
        }

        /// <summary>
        /// Obtém objetos no espaço 2D conforme a seleção pela tela da câmera
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="cam"></param>
        /// <param name="verticesTela"></param>
        /// <returns></returns>
        public static IEnumerable<Objeto2D> ObterObjetos2DMouseXY(this EpicoGraphics engine, Camera2D cam, params Vertice2[] verticesTela)
        {
            for (int i = 0; i < verticesTela.Length; i++)
            {
                // Converte X e Y da tela para as coordenadas X e Y no mundo 2D
                Eixos2 xy = ObterPosEspaco2DMouseXY(cam, verticesTela[i]);
                verticesTela[i].X = xy.X;
                verticesTela[i].Y = xy.Y;
            }

            for (int i = 0; i < engine.objetos2D.Count; i++)
            {
                if (IntersecaoEntrePoligonos(verticesTela,
                    engine.objetos2D[i].Vertices.Select(x => new Vertice2(x.Global.X, x.Global.Y)).ToArray()))
                {
                    yield return engine.objetos2D[i];
                }
            }
        }

        public static IEnumerable<Vertice2> ObterVerticesObjeto2DPelaTela(this Camera2D cam, List<Objeto2D> objs, params Vertice2[] verticesTela)
        {
            for (int i = 0; i < verticesTela.Length; i++)
            {
                // Converte X e Y da tela para as coordenadas X e Y no mundo 2D
                Eixos2 xy = ObterPosEspaco2DMouseXY(cam, verticesTela[i]);
                verticesTela[i].X = xy.X;
                verticesTela[i].Y = xy.Y;
            }

            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];
                for (int i = 0; i < obj.Vertices.Count(); i++)
                {
                    Vertice2 vertice = obj.Vertices[i];
                    if (IntersecaoEntrePoligonos(verticesTela,
                        new Vertice2(vertice.Global.X, vertice.Global.Y)))
                    {
                        yield return vertice;
                    }
                }
            }
        }

        public static IEnumerable<Origem2> ObterOrigensObjeto2DPelaTela(this Camera2D cam, List<Objeto2D> objs, params Vertice2[] verticesTela)
        {
            for (int i = 0; i < verticesTela.Length; i++)
            {
                // Converte X e Y da tela para as coordenadas X e Y no mundo 2D
                Eixos2 xy = ObterPosEspaco2DMouseXY(cam, verticesTela[i]);
                verticesTela[i].X = xy.X;
                verticesTela[i].Y = xy.Y;
            }

            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];
                for (int i = 0; i < obj.Origens.Count; i++)
                {
                    Origem2 origem = obj.Origens[i];
                    if (IntersecaoEntrePoligonos(verticesTela, 
                        new Vertice2(origem.Global.X, origem.Global.Y)))
                    {
                        yield return origem;
                    }
                }
            }
        }

        public static IEnumerable<Vertice2> ObterVetoresObjeto2DPelaTela(this Camera2D camera, List<Objeto2D> objs, RectangleF rect)
        {
            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];

                for (int i = 1; i < obj.Vertices.Count() + 1; i++)
                {
                    Vertice2 v1, v2;
                    if (i == obj.Vertices.Count())
                    {
                        v1 = obj.Vertices[i - 1];
                        v2 = obj.Vertices[0];
                    }
                    else
                    {
                        v1 = obj.Vertices[i - 1];
                        v2 = obj.Vertices[i];
                    }

                    float x1Tela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + v1.X;
                    float y1Tela = -(camera.Pos.Y - camera.ResHeight / 2) + obj.Pos.Y + v1.Y;

                    float x2Tela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + v2.X;
                    float y2Tela = -(camera.Pos.Y - camera.ResHeight / 2) + obj.Pos.Y + v2.Y;

                    // Testa se o vetor está colidindo com a região do retângulo
                    if (Util2D.IntersecaoRetaRetangulo(new Vetor2(x1Tela, y1Tela), new Vetor2(x2Tela, y2Tela), rect))
                    {
                        yield return v1;
                        yield return v2;
                    }

                    i++; // Próximo vetor
                }
            }
        }

        public static bool IntersecaoRetaRetangulo(Eixos2 pontoA, Eixos2 pontoB, RectangleF rect)
        {
            return IntersecaoRetaRetangulo(pontoA, pontoB, rect.X, rect.Y, rect.Right, rect.Bottom);
        }

        public static bool IntersecaoEntreDoisPoligonos(Objeto2D a, Objeto2D b)
        {
            return IntersecaoEntrePoligonos(
                a.Vertices.Select(x => new Vertice2(a.Pos.Global.X, a.Pos.Global.Y)).ToArray(),
                b.Vertices.Select(x => new Vertice2(b.Pos.Global.X, b.Pos.Global.Y)).ToArray());
        }

        /// <summary>
        /// Checa se ocorre a interseção entre dois polígonos
        /// </summary>
        /// <param name="a">Vértices de posições X e Y globais</param>
        /// <param name="b">Vértices de posições X e Y globais</param>
        /// <returns></returns>
        public static bool IntersecaoEntrePoligonos(Vertice2[] a, params Vertice2[] b)
        {
            foreach (Vertice2[] vPoligono in new[] { a, b })
            {
                for (int i1 = 0; i1 < vPoligono.Count(); i1++)
                {
                    int i2 = (i1 + 1) % vPoligono.Count();
                    Vertice2 p1 = vPoligono[i1];
                    Vertice2 p2 = vPoligono[i2];

                    Vetor2 normal = new Vetor2(p2.Y - p1.Y, p1.X - p2.X);

                    float? minA = null, maxA = null;
                    foreach (var p in a)
                    {
                        var projetado = normal.X * p.X + normal.Y * p.Y;
                        if (minA == null || projetado < minA) minA = projetado;
                        if (maxA == null || projetado > maxA) maxA = projetado;
                    }

                    float? minB = null, maxB = null;
                    foreach (var p in b)
                    {
                        float projetado = normal.X * p.X + normal.Y * p.Y;
                        if (minB == null || projetado < minB) minB = projetado;
                        if (maxB == null || projetado > maxB) maxB = projetado;
                    }

                    if (maxA < minB || maxB < minA) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Interseção entre uma reta e um retângulo
        /// </summary>
        /// <param name="pontoA"></param>
        /// <param name="pontoB"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static bool IntersecaoRetaRetangulo(Eixos2 pontoA, Eixos2 pontoB, float minX, float minY, float maxX, float maxY)
        {
            // Completamente fora!
            if ((pontoA.X <= minX && pontoB.X <= minX) || (pontoA.Y <= minY && pontoB.Y <= minY) ||
                (pontoA.X >= maxX && pontoB.X >= maxX) || (pontoA.Y >= maxY && pontoB.Y >= maxY))
                return false;

            float m = (pontoB.Y - pontoA.Y) / (pontoB.X - pontoA.X);

            float y = m * (minX - pontoA.X) + pontoA.Y;
            if (y >= minY && y <= maxY) return true;

            y = m * (maxX - pontoA.X) + pontoA.Y;
            if (y >= minY && y <= maxY) return true;

            float x = (minY - pontoA.Y) / m + pontoA.X;
            if (x >= minX && x <= maxX) return true;

            x = (maxY - pontoA.Y) / m + pontoA.X;
            if (x >= minX && x <= maxX) return true;

            return false;
        }

        /// <summary>
        /// Detecta interseção entre dois segmentos de retas
        /// </summary>
        /// <param name="pontoA1">Ponto A do segmento 1</param>
        /// <param name="pontoB1">Ponto B do segmento 1</param>
        /// <param name="pontoA2">Ponto A do segmento 2</param>
        /// <param name="pontoB2">Ponto B do segmento 2</param>
        /// <param name="intersecao">Ponto de interseção entre as retas</param>
        /// <param name="linhas_intersecao">Linha de interseção?</param>
        /// <param name="aa">Ponto A da linha de interseç+ão</param>
        /// <param name="bb">Ponto B da linha de interseção</param>
        /// <returns></returns>
        public static bool IntersecaoEntreDuasRetas(Vertice2 pontoA1, Vertice2 pontoB1, Vertice2 pontoA2, Vertice2 pontoB2,
            out Vertice2 intersecao, out bool linhas_intersecao, out Vertice2 aa, out Vertice2 bb)
        {
            // Obtém os parâmetros dos segmentos
            float dxAB = pontoB1.X - pontoA1.X;
            float dyAB = pontoB1.Y - pontoA1.Y;
            float dxCD = pontoB2.X - pontoA2.X;
            float dyCD = pontoB2.Y - pontoA2.Y;

            // Resolve para t1 e t2
            float denominador = (dyAB * dxCD - dxAB * dyCD);

            float t1 =
                ((pontoA1.X - pontoA2.X) * dyCD + (pontoA2.Y - pontoA1.Y) * dxCD) / denominador;
            if (float.IsInfinity(t1))
            {
                // As linhas são paralelas (ou próximas o suficiente)
                linhas_intersecao = false;
                intersecao = null;
                aa = null;
                bb = null;
                return false;
            }
            linhas_intersecao = true;

            float t2 = ((pontoA2.X - pontoA1.X) * dyAB + (pontoA1.Y - pontoA2.Y) * dxAB) / -denominador;

            // Ponto de interseção
            intersecao = new Vertice2(pontoA1.X + dxAB * t1, pontoA1.Y + dyAB * t1);

            // Os segmentos se cruzam se t1 e t2 estiverem entre 0 e 1
            bool colisao =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Encontre os pontos mais próximos nos segmentos
            if (t1 < 0) t1 = 0; else if (t1 > 1) t1 = 1;
            if (t2 < 0) t2 = 0; else if (t2 > 1) t2 = 1;

            // Linha de interseção
            aa = new Vertice2(pontoA1.X + dxAB * t1, pontoA1.Y + dyAB * t1);
            bb = new Vertice2(pontoA2.X + dxCD * t2, pontoA2.Y + dyCD * t2);

            return colisao;
        }
    }
}
