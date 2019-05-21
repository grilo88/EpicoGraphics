using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColisaoPoligonos
{
    public partial class frmPrincipal : Form
    {
        List<Poligono> poligonos = new List<Poligono>();
        Poligono jogador;

        public frmPrincipal()
        {
            InitializeComponent();

            Paint += new PaintEventHandler(Form1_Paint);
            KeyDown += new KeyEventHandler(Form1_KeyDown);

            KeyPreview = true;
            DoubleBuffered = true;

            Random rnd = new Random(Environment.TickCount);

            Poligono p = new Poligono();
            p.Pontos.Add(new Vetor(0, 0));
            p.Pontos.Add(new Vetor(50, -25));
            p.Pontos.Add(new Vetor(75, 0));
            p.Pontos.Add(new Vetor(75, 75));
            p.Pontos.Add(new Vetor(50, 100));
            p.Pontos.Add(new Vetor(0, 75));
            p.cor = Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            poligonos.Add(p);

            p = new Poligono();
            p.Pontos.Add(new Vetor(150, 150));
            p.Pontos.Add(new Vetor(50, 0));
            p.Pontos.Add(new Vetor(150, 0));
            p.Posicao(80, 80);
            p.cor = Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            poligonos.Add(p);

            p = new Poligono();
            p.Pontos.Add(new Vetor(0, 50));
            p.Pontos.Add(new Vetor(50, 0));
            p.Pontos.Add(new Vetor(150, 80));
            p.Pontos.Add(new Vetor(160, 200));
            p.Pontos.Add(new Vetor(-10, 190));
            p.Posicao(300, 300);
            p.cor = Color.FromArgb(255, rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            poligonos.Add(p);

            foreach (Poligono polygon in poligonos) polygon.CriarArestas();

            jogador = poligonos[0];
        }

        void Form1_Paint(object sender, PaintEventArgs e)
        {

            foreach (Poligono p in poligonos)
            {
                e.Graphics.FillPolygon(new SolidBrush(p.cor), p.Pontos.Select(x => new PointF(x.X, x.Y)).ToArray());
            }

            Invalidate();
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int i = 14;
            Vetor velocidade = new Vetor();

            switch (e.KeyValue)
            {

                case 32: //Espaço

                    break;

                case 38: // Cima

                    velocidade = new Vetor(0, -i);
                    break;

                case 40: // Baixo

                    velocidade = new Vetor(0, i);
                    break;

                case 39: // Direita

                    velocidade = new Vetor(i, 0);
                    break;

                case 37: // Esquerda

                    velocidade = new Vetor(-i, 0);
                    break;

            }

            Vetor jogadorTransladacao = velocidade;

            foreach (Poligono poligono in poligonos)
            {
                // Ignora o jogador no tratamento
                if (poligono == jogador) continue;

                // Testa a colisão com os polígonos
                ColisaoPoligonoResultado r = ColisaoPoligono(jogador, poligono, velocidade);

                if (r.Interceptar)
                {
                    jogadorTransladacao = velocidade + r.TranslacaoMinimaVetor;
                    break;
                }
            }

            jogador.Posicao(jogadorTransladacao);

        }

        // Estrutura que armazena os resultados da função PolygonCollision
        public struct ColisaoPoligonoResultado
        {
            public bool Interceptar; // Os polígonos vão se cruzar no tempo?
            public bool Intersecao; // Os polígonos estão se cruzando atualmente?
            public Vetor TranslacaoMinimaVetor; // A translação a aplicar ao polígono A para empurrar os polígonos.
        }

        // Verifique se o polígono A vai colidir com o polígono B na velocidade dada
        public ColisaoPoligonoResultado ColisaoPoligono(Poligono poligonoA, Poligono poligonoB, Vetor velocidade)
        {
            ColisaoPoligonoResultado resultado = new ColisaoPoligonoResultado();
            resultado.Intersecao = true;
            resultado.Interceptar = true;

            int arestaCountA = poligonoA.Arestas.Count;
            int arestaCountB = poligonoB.Arestas.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vetor EixoTranslacao = new Vetor();
            Vetor aresta;

            // Loop através de todas as bordas de ambos os polígonos
            for (int indiceAresta = 0; indiceAresta < arestaCountA + arestaCountB; indiceAresta++)
            {
                if (indiceAresta < arestaCountA)
                {
                    aresta = poligonoA.Arestas[indiceAresta];
                }
                else
                {
                    aresta = poligonoB.Arestas[indiceAresta - arestaCountA];
                }

                // ===== 1. Descobrir se os polígonos estão se cruzando atualmente =====

                // Encontre o eixo perpendicular à borda atual
                Vetor eixo = new Vetor(-aresta.Y, aresta.X);
                eixo.Normalizar();

                // Encontre a projeção do polígono no eixo atual
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjecaoPoligono(eixo, poligonoA, ref minA, ref maxA);
                ProjecaoPoligono(eixo, poligonoB, ref minB, ref maxB);

                // Verifique se as projeções de polígono estão se cruzando atualmente
                if (DistanciaDoIntervalo(minA, maxA, minB, maxB) > 0) resultado.Intersecao = false;

                // ===== 2. Agora, encontre os polígonos que irão se *cruzar* =====

                // Projetar a velocidade no eixo atual
                float velocidadeProjecao = eixo.ProdutoPontual(velocidade);

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

                    Vetor d = poligonoA.Centro - poligonoB.Centro;
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
        public void ProjecaoPoligono(Vetor eixo, Poligono poligono, ref float min, ref float max)
        {
            // Para projetar um ponto em um eixo, use o produto escalar
            float d = eixo.ProdutoPontual(poligono.Pontos[0]);
            min = d;
            max = d;
            for (int i = 0; i < poligono.Pontos.Count; i++)
            {
                d = poligono.Pontos[i].ProdutoPontual(eixo);
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
