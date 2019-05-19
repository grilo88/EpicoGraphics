using Epico.Sistema;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if Editor2D || NetStandard2 || NetCore
using System.Drawing;
#elif EtoForms
using Eto.Drawing;
#endif

namespace Epico
{
    public static class Util
    {

        public static float DistanciaEntreDoisPontos(this EixoXY pontoA, EixoXY pontoB) => DistanciaEntreDoisPontos(pontoA.X, pontoA.Y, pontoB.X, pontoB.Y);
        public static float AnguloEntreDoisPontos(this EixoXY pontoA, EixoXY pontoB) => AnguloEntreDoisPontos(pontoA.X, pontoA.Y, pontoB.X, pontoB.Y);

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

        /// <summary>
        /// Rotaciona um ponto 2D a partir de um ponto de origem
        /// </summary>
        /// <param name="origem">Ponto de origem do objeto</param>
        /// <param name="p">Ponto a ser rotacionado</param>
        /// <param name="graus">Diferença de ângulos no caso de rotação ou soma dos ângulo no caso de acoplamento de objetos.</param>
        /// <returns></returns>
        public static EixoXY RotacionarPonto2D(EixoXY origem, EixoXY ponto, float graus) => RotacionarPonto2D(origem.X, origem.Y, ponto.X, ponto.Y, graus);

        /// <summary>
        /// Rotaciona um ponto 2D a partir de um ponto de origem
        /// </summary>
        /// <param name="origem">Ponto de origem do objeto</param>
        /// <param name="p">Ponto a ser rotacionado</param>
        /// <param name="angulo">Diferença de ângulos no caso de rotação ou soma dos ângulo no caso de acoplamento de objetos.</param>
        /// <returns></returns>
        public static EixoXY RotacionarPonto2D(float origemX, float origemY, float x, float y, float angulo)
        {
            float rad = Angulo2Radiano(angulo);
            float rotX = (float)(Math.Cos(rad) * (x - origemX) - Math.Sin(rad) * (y - origemY) + origemX);
            float rotY = (float)(Math.Sin(rad) * (x - origemX) + Math.Cos(rad) * (y - origemY) + origemY);
            return new XY(rotX, rotY);
        }

        /// <summary>
        /// Obtém objeto 2d através do espaço global.
        /// </summary>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public static Objeto2D ObterObjeto2DPeloEspaco(this Epico2D engine, Vetor2D ponto)
        {
            for (int i = 0; i < engine.objetos.Count; i++)
            {
                Objeto2D obj = engine.objetos[i];

                float xMax = obj.Pos.X + obj.XMax;
                float xMin = obj.Pos.X + obj.XMin;
                float yMax = obj.Pos.Y + obj.YMax;
                float yMin = obj.Pos.Y + obj.YMin;

                if (ponto.X >= xMin && ponto.X <= xMax)
                    if (ponto.Y >= yMin && ponto.Y <= yMax)
                    {
                        return engine.objetos[i];
                    }
            }
            return null;
        }

        /// <summary>
        /// Obtém objeto 2d através da tela. X = 0 a Largura da camera, Y = 0 ao tamanho da camera.
        /// </summary>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public static Objeto2D ObterObjeto2DPelaTela(this Epico2D engine, Camera2D camera, PointF ponto)
        {
            for (int i = 0; i < engine.objetos.Count; i++)
            {
                Objeto2D obj = engine.objetos[i];

                float xMaxTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.XMax;
                float xMinTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.XMin;
                float yMaxTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + obj.YMax;
                float yMinTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + obj.YMin;

                if (ponto.X >= xMinTela && ponto.X <= xMaxTela)
                    if (ponto.Y >= yMinTela && ponto.Y <= yMaxTela)
                    {
                        return engine.objetos[i];
                    }
            }
            return null;
        }

        public static IEnumerable<Objeto2D> ObterObjetos2DPelaTela(this Epico2D engine, Camera2D camera, PointF ponto)
        {
            return ObterObjetos2DPelaTela(engine, camera, new XY(ponto.X, ponto.Y));
        }

        /// <summary>
        /// Obtém objetos 2d através da tela. X = 0 a Largura da camera, Y = 0 ao tamanho da camera.
        /// </summary>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public static IEnumerable<Objeto2D> ObterObjetos2DPelaTela(this Epico2D engine, Camera2D camera, EixoXY ponto)
        {
            for (int i = 0; i < engine.objetos.Count; i++)
            {
                Objeto2D obj = engine.objetos[i];
                var comp = obj;
                //Objeto2D comp = camera.ObjetoAnguloCamera(obj, true);

                float xMaxTela = -(camera.Pos.X - camera.ResWidth / 2) + comp.Pos.X + comp.XMax;
                float xMinTela = -(camera.Pos.X - camera.ResWidth / 2) + comp.Pos.X + comp.XMin;
                float yMaxTela = -(camera.Pos.Y - camera.ResHeigth / 2) + comp.Pos.Y + comp.YMax;
                float yMinTela = -(camera.Pos.Y - camera.ResHeigth / 2) + comp.Pos.Y + comp.YMin;

                if (ponto.X >= xMinTela && ponto.X <= xMaxTela)
                    if (ponto.Y >= yMinTela && ponto.Y <= yMaxTela)
                    {
                        yield return engine.objetos[i];
                    }
            }
        }

        public static Objeto2D ObterUnicoObjeto2DPelaTela(this Epico2D engine, Camera2D camera, EixoXY ponto)
        {
            return ObterObjetos2DPelaTela(engine, camera, ponto).LastOrDefault();
        }

        /// <summary>
        /// Obtém objetos 2d através da tela. X = 0 a Largura da camera, Y = 0 ao tamanho da camera.
        /// </summary>
        /// <param name="ponto"></param>
        /// <returns></returns>
        public static IEnumerable<Objeto2D> ObterObjetos2DPelaTela(this Epico2D engine, Camera2D camera, RectangleF rect)
        {
            for (int i = 0; i < engine.objetos.Count; i++)
            {
                Objeto2D obj = engine.objetos[i];

                float xMaxTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.XMax;
                float xMinTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + obj.XMin;
                float yMaxTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + obj.YMax;
                float yMinTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + obj.YMin;

                // Testa se o objeto está colidindo com a região do retângulo
                if ((xMinTela >= rect.X || xMaxTela >= rect.X) && (xMinTela <= rect.X + rect.Width || xMaxTela <= rect.X + rect.Width))
                    if ((yMinTela >= rect.Y || yMaxTela >= rect.Y) && (yMinTela <= rect.Y + rect.Height || yMaxTela <= rect.Y + rect.Height))
                    {
                        yield return engine.objetos[i];
                    }
            }
        }

        public static IEnumerable<Vertice2D> ObterVerticesObjeto2DPelaTela(Camera2D camera, List<Objeto2D> objs, RectangleF rect)
        {
            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];

                for (int i = 0; i < obj.Vertices.Length; i++)
                {
                    Vertice2D v = obj.Vertices[i];

                    float xTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + v.X;
                    float yTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + v.Y;

                    // Testa se as vértices estão colidindo com a região do retângulo
                    if (xTela >= rect.X && xTela <= rect.X + rect.Width)
                        if (yTela >= rect.Y && yTela <= rect.Y + rect.Height)
                        {
                            yield return v;
                        }
                }
            }
        }

        public static IEnumerable<Origem2D> ObterOrigensObjeto2DPelaTela(Camera2D camera, List<Objeto2D> objs, RectangleF rect)
        {
            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];

                for (int i = 0; i < obj.Origem.Count; i++)
                {
                    Origem2D c = obj.Origem[i];

                    float xTela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + c.X;
                    float yTela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + c.Y;

                    // Testa se as vértices estão colidindo com a região do retângulo
                    if (xTela >= rect.X && xTela <= rect.X + rect.Width)
                        if (yTela >= rect.Y && yTela <= rect.Y + rect.Height)
                        {
                            yield return c;
                        }
                }
            }
        }

        public static IEnumerable<Vertice2D> ObterVetoresObjeto2DPelaTela(Camera2D camera, List<Objeto2D> objs, RectangleF rect)
        {
            for (int o = 0; o < objs.Count; o++)
            {
                Objeto2D obj = objs[o];

                for (int i = 1; i < obj.Vertices.Length + 1; i++)
                {
                    Vertice2D v1, v2;
                    if (i == obj.Vertices.Length)
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
                    float y1Tela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + v1.Y;

                    float x2Tela = -(camera.Pos.X - camera.ResWidth / 2) + obj.Pos.X + v2.X;
                    float y2Tela = -(camera.Pos.Y - camera.ResHeigth / 2) + obj.Pos.Y + v2.Y;

                    // Testa se o vetor está colidindo com a região do retângulo
                    if (Util.IntersecaoRetaRetangulo(new Vetor2D(x1Tela, y1Tela), new Vetor2D(x2Tela, y2Tela), rect))
                    {
                        yield return v1;
                        yield return v2;
                    }

                    i++; // Próximo vetor
                }
            }
        }

        public static bool IntersecaoRetaRetangulo(Vetor2D a, Vetor2D b, RectangleF rect)
        {
            return IntersecaoRetaRetangulo(a, b, rect.X, rect.Y, rect.Right, rect.Bottom);
        }

        /// <summary>
        /// Interseção entre uma reta e um retângulo
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static bool IntersecaoRetaRetangulo(Vetor2D a, Vetor2D b, float minX, float minY, float maxX, float maxY)
        {
            // Completamente fora
            if ((a.X <= minX && b.X <= minX) || (a.Y <= minY && b.Y <= minY) ||
                (a.X >= maxX && b.X >= maxX) || (a.Y >= maxY && b.Y >= maxY))
                return false;

            float m = (b.Y - a.Y) / (b.X - a.X);

            float y = m * (minX - a.X) + a.Y;
            if (y >= minY && y <= maxY) return true;

            y = m * (maxX - a.X) + a.Y;
            if (y >= minY && y <= maxY) return true;

            float x = (minY - a.Y) / m + a.X;
            if (x >= minX && x <= maxX) return true;

            x = (maxY - a.Y) / m + a.X;
            if (x >= minX && x <= maxX) return true;

            return false;
        }

        /// <summary>
        /// Detecta interseção entre dois segmentos de retas
        /// </summary>
        /// <param name="a">Ponto A do segmento 1</param>
        /// <param name="b">Ponto B do segmento 1</param>
        /// <param name="c">Ponto A do segmento 2</param>
        /// <param name="d">Ponto B do segmento 2</param>
        /// <param name="intersecao">Ponto de interseção entre as retas</param>
        /// <param name="linhas_intersecao">Linha de interseção</param>
        /// <param name="aa">Ponto A da linha de interseç+ão</param>
        /// <param name="bb">Ponto B da linha de interseção</param>
        /// <returns></returns>
        public static bool IntersecaoEntreDuasRetas(Vertice2D a, Vertice2D b, Vertice2D c, Vertice2D d,
            out Vertice2D intersecao, out bool linhas_intersecao, out Vertice2D aa, out Vertice2D bb)
        {
            // Obtém os parâmetros dos segmentos
            float dxAB = b.X - a.X;
            float dyAB = b.Y - a.Y;
            float dxCD = d.X - c.X;
            float dyCD = d.Y - c.Y;

            // Resolve para t1 e t2
            float denominador = (dyAB * dxCD - dxAB * dyCD);

            float t1 =
                ((a.X - c.X) * dyCD + (c.Y - a.Y) * dxCD) / denominador;
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

            float t2 = ((c.X - a.X) * dyAB + (a.Y - c.Y) * dxAB) / -denominador;

            // Ponto de interseção
            intersecao = new Vertice2D(a.X + dxAB * t1, a.Y + dyAB * t1);

            // Os segmentos se cruzam se t1 e t2 estiverem entre 0 e 1
            bool colisao =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Encontre os pontos mais próximos nos segmentos
            if (t1 < 0) t1 = 0; else if (t1 > 1) t1 = 1;
            if (t2 < 0) t2 = 0; else if (t2 > 1) t2 = 1;

            // Linha de interseção
            aa = new Vertice2D(a.X + dxAB * t1, a.Y + dyAB * t1);
            bb = new Vertice2D(c.X + dxCD * t2, c.Y + dyCD * t2);

            return colisao;
        }
    }
}
