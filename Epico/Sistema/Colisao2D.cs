using System;
using System.Collections.Generic;
using System.Text;

namespace Epico.Sistema
{
    // Estrutura que armazena os resultados da função PolygonCollision
    public struct ColisaoPoligonoConvexoResultado
    {
        public bool Interceptar; // Os polígonos vão se cruzar no tempo?
        public bool Intersecao; // Os polígonos estão se cruzando atualmente?
        public Vetor2D TranslacaoMinimaVetor; // A translação a aplicar ao polígono A para empurrar os polígonos.
    }

    public class Colisao2D
    {
        public ColisaoPoligonoConvexoResultado PoligonoConvexo(
            Objeto2D objetoA, Objeto2D objetoB, Vetor2D movimento)
        {
            ColisaoPoligonoConvexoResultado resultado = new ColisaoPoligonoConvexoResultado();
            resultado.Intersecao = true;
            resultado.Interceptar = true;

            int arestaQuantA = objetoA.Arestas.Count;
            int arestaQuantB = objetoB.Arestas.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vetor2D EixoTranslacao = new Vetor2D();
            Vetor2D aresta;

            // Loop através de todas as bordas de ambos os polígonos
            for (int indiceAresta = 0; indiceAresta < arestaQuantA + arestaQuantB; indiceAresta++)
            {
                if (indiceAresta < arestaQuantA)
                    aresta = objetoA.Arestas[indiceAresta];
                else
                    aresta = objetoB.Arestas[indiceAresta - arestaQuantA];

                // ===== 1. Descobrir se os polígonos estão se cruzando atualmente =====

                // Encontre o eixo perpendicular à borda atual
                Vetor2D eixo = new Vetor2D(objetoA, -aresta.Y, aresta.X);
                eixo.Normalizar();

                // Encontre a projeção do polígono no eixo atual
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjecaoPoligono(eixo, objetoA, ref minA, ref maxA);
                ProjecaoPoligono(eixo, objetoB, ref minB, ref maxB);

                // Verifique se as projeções de polígono estão se cruzando atualmente
                if (DistanciaDoIntervalo(minA, maxA, minB, maxB) > 0) resultado.Intersecao = false;

                // ===== 2. Agora, encontre os polígonos que irão se *cruzar* =====

                // Projetar a velocidade no eixo atual
                float velocidadeProjecao = eixo.ProdutoPontual(movimento);

                // Obter a projeção do polígono A durante o movimento
                if (velocidadeProjecao < 0)
                {
                    minA += velocidadeProjecao;
                }
                else
                {
                    maxA += velocidadeProjecao;
                }

                // Faça o mesmo teste acima para a nova projeção
                float distanciaDoIntervalo = DistanciaDoIntervalo(minA, maxA, minB, maxB);
                if (distanciaDoIntervalo > 0) resultado.Interceptar = false;

                // Se os polígonos não estiverem se cruzando e não se cruzarem, saia do loop
                if (!resultado.Intersecao && !resultado.Interceptar) break;

                // Verifique se a distância atual do intervalo é a mínima.
                // Em caso afirmativo, armazene a distância do intervalo e a distância atual. 
                // Isso será usado para calcular o vetor de transladação mínima
                distanciaDoIntervalo = Math.Abs(distanciaDoIntervalo);
                if (distanciaDoIntervalo < minIntervalDistance)
                {
                    minIntervalDistance = distanciaDoIntervalo;
                    EixoTranslacao = eixo;

                    Vetor2D d = new Vetor2D(objetoA, objetoA.Centro.Global - objetoB.Centro.Global);
                    if (d.ProdutoPontual(EixoTranslacao) < 0) EixoTranslacao = -EixoTranslacao;
                }
            }

            // O vetor de transladação mínimo pode ser usado para pressionar os polígonos. 
            // Primeiro move os polígonos pela sua velocidade e, em seguida, move PoligonoA por TransladacaoMinimaVetor.
            if (resultado.Interceptar) resultado.TranslacaoMinimaVetor = EixoTranslacao * minIntervalDistance;

            return resultado;
        }

        // Calcular a distância entre[minA, maxA] e[minB, maxB] a distância será negativa se os intervalos se sobrepuserem
        public float DistanciaDoIntervalo(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        // Calcule a projeção de um polígono em um eixo e retorne-o como um intervalo [min, max]
        public void ProjecaoPoligono(EixoXY global_eixo, Objeto2D poligono, ref float min, ref float max)
        {
            // Para projetar um ponto em um eixo, use o produto escalar
            float d = global_eixo.ProdutoPontual(poligono.Vertices[0].Global);
            min = d;
            max = d;
            for (int i = 0; i < poligono.Vertices.Length; i++)
            {
                d = poligono.Vertices[i].Global.ProdutoPontual(global_eixo);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }
    }
}
