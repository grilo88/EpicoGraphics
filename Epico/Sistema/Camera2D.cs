using Epico.Luzes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if Editor2D || NetCore || NetStandard2
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
#elif EtoForms
using Eto.Drawing;
#endif


namespace Epico.Sistema
{
    public sealed class Camera2D : Objeto2D, IDisposable
    {
        readonly Epico2D engine;
        Bitmap render;
        Graphics g;

#region Campos
        public int ResWidth;
        public int ResHeigth;
#if Editor2D || NetStandard2 || NetCore
        public PixelFormat FormatoPixel = PixelFormat.Format32bppArgb;
        public InterpolationMode ModoInterpolacao = InterpolationMode.Default;
        public PixelOffsetMode ModoDeslocamentoPixel = PixelOffsetMode.None;
#elif EtoForms
        public PixelFormat FormatoPixel = PixelFormat.Format32bppRgba;
        public ImageInterpolation ModoInterpolacao = ImageInterpolation.Default;
        public PixelOffsetMode ModoDeslocamentoPixel = PixelOffsetMode.None;
#endif
        private int _fps;
        private int _tickFPS;
#endregion

#region Propriedades
        public int FPS { get; private set; }
        private int _maxFPS { get; set; } = 60;
        private float _tickMaxFPS { get; set; }
        /// <summary>Tempo de atraso entre uma renderização e outra.</summary>
        public long TempoDelta { get; private set; }
        public float ZoomCamera { get; set; } = 1F;
        public bool DesligarSistemaZoom { get; set; } = true;
        public bool AntiSerrilhado { get; set; } = true;
        public float PontosPorPixel { get; set; } = 1;

        public float Left => Pos.X - ResWidth / 2;
        public float Right => Pos.X + ResWidth / 2;
        public float Top => Pos.Y - ResHeigth / 2;
        public float Bottom => Pos.Y + ResHeigth / 2;
#endregion

        public Camera2D(Epico2D engine, int width, int height)
        {
            this.engine = engine;
            IniciarCamera(width, height, FormatoPixel);
        }

        public Camera2D(Epico2D engine, int width, int height, PixelFormat FormatoPixel)
        {
            this.engine = engine;
            IniciarCamera(width, height, FormatoPixel);
        }

        private void IniciarCamera(int width, int heigth, PixelFormat formatoPixel)
        {
            Nome = "Camera";
            FormatoPixel = formatoPixel;
            ResWidth = width;
            ResHeigth = heigth;
            render = new Bitmap(width, heigth, formatoPixel);
#if Editor2D || NetCore || NetStandard2
            g = Graphics.FromImage(render);
            g.SmoothingMode = AntiSerrilhado ? SmoothingMode.AntiAlias : SmoothingMode.None;
            g.InterpolationMode = ModoInterpolacao;
#elif EtoForms
            g.AntiAlias = AntiSerrilhado;
            g.ImageInterpolation = ModoInterpolacao;
#endif
            g.PixelOffsetMode = ModoDeslocamentoPixel;
            DefineFPSMaximo(_maxFPS);
        }

        public void RedefinirResolucao(int width, int height) => RedefinirResolucao(width, height, FormatoPixel);

        public void RedefinirResolucao(int width, int height, PixelFormat pixelFormat)
        {
            if (width > 0 && height > 0)
            {
                render.Dispose();
                g.Dispose();

                FormatoPixel = pixelFormat;
                ResWidth = width;
                ResHeigth = height;
                render = new Bitmap(width, height, pixelFormat);
#if Editor2D || NetCore || NetStandard2
                g = Graphics.FromImage(render);
                g.SmoothingMode = AntiSerrilhado ? SmoothingMode.AntiAlias : SmoothingMode.None;
                g.InterpolationMode = ModoInterpolacao;
#elif EtoForms
                g.AntiAlias = AntiSerrilhado;
                g.ImageInterpolation = ModoInterpolacao;
#endif
                g.PixelOffsetMode = ModoDeslocamentoPixel;
            }
        }
        public void DefineFPSMaximo(int maxFPS)
        {
            this._maxFPS = maxFPS;
            this._tickMaxFPS = 1000 / maxFPS;
        }

        /// <summary>
        /// Incrementa o Zoom
        /// </summary>
        public void Zoom(float valor)
        {
            this.ZoomCamera += valor;
        }

        /// <summary>
        /// Define o Zoom da Câmera
        /// </summary>
        public void DefinirZoom(float zoom)
        {
            ZoomCamera = zoom;
        }

        /// <summary>
        /// Centraliza a camera no objeto
        /// </summary>
        /// <param name="obj"></param>
        public void Focar(Objeto2D obj) => Pos = obj.Pos;
        public void Focar(Origem2D c) => Pos = new Vetor2D(c.GlobalX, c.GlobalY);
        public void Focar(Vertice2D v) => Pos = new Vetor2D(v.GlobalX, v.GlobalY);

#warning O ângulo da câmera não está perfeita, as vezes gira demais ou de menos.
        private float _grausCamera;
        private float _anguloRotacionado;
        public override float Angulo
        {
            get
            {
                return base.Angulo;
            }
            set
            {
                float novoAngulo = value;
                _grausCamera = novoAngulo - base.Angulo;

#warning A ideia é manter a quantidade de graus que houve rotação da camera em relação a Zero
                _anguloRotacionado = base.Angulo + _grausCamera;
                base.Angulo = novoAngulo;
            }
        }

#region Atributos de otimização do Renderizador
        PointF pontoA = new PointF();
        PointF pontoB = new PointF();
        PointF TelaPos = new PointF();
        long _tickRender;

        readonly Font font_debug = new Font("Lucida Console", 12,
#if Editor2D || NetStandard2 || NetCore
            FontStyle.Regular
#elif EtoForms
            FontStyle.None
#endif
            );
        readonly SolidBrush font_debug_color = new SolidBrush(Color.FromArgb(255, 127, 255, 212) /*Aquamarine*/);
#endregion

        public Bitmap Renderizar()
        {
            TempoDelta = DateTime.Now.Ticks - _tickRender; // Calcula o tempo delta (tempo de atraso)
            _tickRender = DateTime.Now.Ticks;

            if (ResWidth > 0 && ResHeigth > 0) // Janela não minimizada?
            {
                g.Clear(Color.FromArgb(255, 0, 0, 0) /*Preto*/);

                // Obtém a posição da tela da câmera
                TelaPos.Y = Pos.Y - ResHeigth / 2;
                TelaPos.X = Pos.X - ResWidth / 2;

                for (int i = 0; i < engine.objetos.Count; i++)
                {
                    Objeto2DRenderizar objEspaco = engine.objetos[i] as Objeto2DRenderizar;
                    if (objEspaco != null)
                    {
                        Objeto2DRenderizar obj = (Objeto2DRenderizar)objEspaco.Clone();

#region Calcula o ZOOM da câmera
                        if (!DesligarSistemaZoom)
                        {
                            Objeto2D objZoom = ZoomEscalaObjeto2D(obj, ZoomCamera);
                            Objeto2D objPosZoom = ZoomPosObjeto2D(obj, ZoomCamera);
                            objZoom.Pos = objPosZoom.Pos;
                        }
#endregion

#region Calcula o ângulo da câmera
                        for (int v = 0; v < obj.Vertices.Count(); v++)
                        {
                            XY globalPos = new XY(
                                obj.Vertices[v].GlobalX,
                                obj.Vertices[v].GlobalY);
#warning O ângulo da câmera não está perfeita, as vezes gira demais ou de menos.
                            EixoXY xy = Util.RotacionarPonto2D(Pos, globalPos, _grausCamera);
                            obj.Vertices[v].X = xy.X - obj.Pos.X;
                            obj.Vertices[v].Y = xy.Y - obj.Pos.Y;
                        }
#endregion

                        obj.AtualizarXYMinMax();

                        if (Objeto2DVisivelCamera(obj))
                        {
                            if (obj.Mat_render.CorSolida.A > 0) // Pinta objeto materialmente visível
                            {
                                GraphicsPath preenche = new GraphicsPath();
                                preenche.AddLines(obj.Vertices.ToList().Select(ponto => new PointF(
                                    -TelaPos.X + ponto.GlobalX,
                                    -TelaPos.Y + ponto.GlobalY)).ToArray());
                                g.FillPath(new SolidBrush(Color.FromArgb(obj.Mat_render.CorSolida.A, obj.Mat_render.CorSolida.R, obj.Mat_render.CorSolida.G, obj.Mat_render.CorSolida.B)), preenche);
                            }

                            // Materialização do objeto na Câmera
                            Material mat;
                            if (obj.Selecionado)
                                mat = obj.Mat_render_sel;
                            else
                                mat = obj.Mat_render;

                            if (mat.CorBorda.A > 0) // Desenha borda dos objetos materialmente visíveis
                            {
                                // Cor da borda do objeto
                                Pen pen = new Pen(new SolidBrush(Color.FromArgb(mat.CorBorda.A, mat.CorBorda.R, mat.CorBorda.G, mat.CorBorda.B)));

#if Editor2D
                                pen.Width = mat.LarguraBorda;
#elif EtoForms
                                pen.Thickness = mat.LarguraBorda;
#endif
                                for (int v = 1; v < obj.Vertices.Length + 1; v++)
                                {
                                    Vertice2D v1, v2;
                                    if (v == obj.Vertices.Length) // Conecta a última Vértice na primeira Vértice
                                    {
                                        v2 = obj.Vertices[v - 1];     // Ponto Final
                                        v1 = obj.Vertices[0];         // Ponto Inicial
                                    }
                                    else
                                    {
                                        v1 = obj.Vertices[v - 1]; // Ponto A
                                        v2 = obj.Vertices[v];     // Ponto B
                                    }

                                    // Desenha as linhas entre as vértices na câmera
                                    pontoA.X = -TelaPos.X + v1.GlobalX;
                                    pontoA.Y = -TelaPos.Y + v1.GlobalY;
                                    pontoB.X = -TelaPos.X + v2.GlobalX;
                                    pontoB.Y = -TelaPos.Y + v2.GlobalY;

                                    g.DrawLine(pen, pontoA, pontoB);
                                }
                            }

                            for (int v = 0; v < obj.Vertices.Length; v++)
                            {
                                if (obj.Vertices[v].Sel)
                                {
                                    float width = 5;
                                    float x = -TelaPos.X + obj.Vertices[v].GlobalX;
                                    float y = -TelaPos.Y + obj.Vertices[v].GlobalY;
                                    RectangleF rect = new RectangleF(x - width / 2, y - width / 2, width, width);
                                    g.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0, 0) /*Vermelho*/), rect);
                                }
                            }

                            // Exibe o(s) ponto(s) de origem do objeto
                            for (int c = 0; c < obj.Origem.Count; c++)
                            {
                                if (obj.Origem[c].Sel)
                                {
                                    float width = 5;
                                    float x = -TelaPos.X + obj.Origem[c].GlobalX;
                                    float y = -TelaPos.Y + obj.Origem[c].GlobalY;
                                    RectangleF rect = new RectangleF(x - width / 2, y - width / 2, width, width);
                                    g.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 255, 0) /*Amarelo*/), rect);
                                }
                            }
                        }
                    }
                }

                // A iluminação deve ser renderizada após pintar todos os objetos.
                for (int i = 0; i < engine.objetos.Count; i++)
                {
                    Luz2DRenderizar luz = engine.objetos[i] as Luz2DRenderizar;
                    if (luz != null)
                    {
                        if (luz is LuzPonto)
                        {
                            if (Objeto2DVisivelCamera(luz))
                            {
                                //GraphicsPath preenche = new GraphicsPath();
                                //preenche.FillMode = FillMode.Alternate;

                                //preenche.AddLines(luz.Vertices.ToList().Select(ponto => new PointF(
                                //    -TelaPos.X + ponto.GlobalX,
                                //    -TelaPos.Y + ponto.GlobalY)).ToArray());



                                ////pthGrBrush.CenterColor = Color.FromArgb(luz.Cor.A, luz.Cor.R, luz.Cor.G, luz.Cor.B);
                                ////Color[] colors = { Color.FromArgb(0, 0, 0, 0) };
                                ////pthGrBrush.SurroundColors = colors;
                                //g.FillPath(pthGrBrush, preenche);
                            }
                        }
                    }

                    // Sombras devem ser renderizadas após a renderização da iluminação
#region Sombras

#endregion
                }

#region Exibe informações de depuração
                if (engine.Debug)
                {
#if Editor2D
                    g.DrawString(Nome.ToUpper(), font_debug, font_debug_color, new PointF(10, 10));
                    g.DrawString("FPS: " + FPS, font_debug, font_debug_color, new PointF(10, 30));
#elif EtoForms
                    g.DrawText(font_debug, font_debug_color, new PointF(10, 10), Nome.ToUpper());
                    g.DrawText(font_debug, font_debug_color, new PointF(10, 30), "FPS: " + FPS);
#endif
                }
#endregion
            }
            _grausCamera = 0;

#region Calcula o FPS
            if (Environment.TickCount - _tickFPS >= 1000)
            {
                _tickFPS = Environment.TickCount;
                FPS = _fps;
                _fps = 0;
            }
            else _fps++;
#endregion

#region Limita o FPS
            // TODO: Limitar o FPS
#endregion

#if EtoForms
            g.DrawImage(render, new PointF(0, 0));
#endif
            return render;
        }

        public Objeto2D ObjetoAnguloCamera(Objeto2D obj, bool clonar = true)
        {
#warning obter objetos de acordo com o angulo da camera. Ainda há falhas nesta lógica
            Objeto2D clone = clonar ? (Objeto2D)obj.Clone() : obj;
            for (int v = 0; v < obj.Vertices.Count(); v++)
            {
                XY globalPos = new XY(
                    obj.Vertices[v].GlobalX,
                    obj.Vertices[v].GlobalY);

#warning Zero não rotaciona, 1 rotaciona
                EixoXY xy = Util.RotacionarPonto2D(Pos, globalPos, _anguloRotacionado);
                obj.Vertices[v].X = xy.X - obj.Pos.X;
                obj.Vertices[v].Y = xy.Y - obj.Pos.Y;
            }
            return clone;
        }

        public bool Objeto2DVisivelCamera(Objeto2D obj)
        {
            TelaPos.Y = Pos.Y - ResHeigth / 2;
            TelaPos.X = Pos.X - ResWidth / 2;

            float pontoXMax = -TelaPos.X + obj.GlobalXMax;
            float pontoXMin = -TelaPos.X + obj.GlobalXMin;
            float pontoYMax = -TelaPos.Y + obj.GlobalYMax;
            float pontoYMin = -TelaPos.Y + obj.GlobalYMin;

            if (pontoXMax >= 0 && pontoXMin <= ResWidth)
                if (pontoYMax >= 0 && pontoYMin <= ResHeigth)
                {
                    return true;
                }
            return false;
        }

        //public bool Objeto2DVisivel(Objeto2D obj)
        //{
        //    float xMax = -(Pos.X - ResWidth / 2) + obj.Pos.X + obj.XMax;
        //    float xMin = -(Pos.X - ResWidth / 2) + obj.Pos.X + obj.XMin;
        //    float yMax = -(Pos.Y - ResHeigth / 2) + obj.Pos.Y + obj.YMax;
        //    float yMin = -(Pos.Y - ResHeigth / 2) + obj.Pos.Y + obj.YMin;

        //    if (xMax >= 0 && xMin <= ResWidth)
        //        if (yMax >= 0 && yMin <= ResHeigth)
        //        {
        //            return true;
        //        }

        //    return false;
        //}

        /// <summary>
        /// Trabalha o Zoom orientado a escala do objeto
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private Objeto2D ZoomEscalaObjeto2D(Objeto2D obj, float zoom)
        {
            for (int i = 0; i < obj.Vertices.Length; i++)
            {
                obj.Vertices[i].X = (float)(Math.Sin(obj.Vertices[i].Rad + Util.Angulo2Radiano(obj.Angulo)) * obj.Vertices[i].Raio * zoom);
                obj.Vertices[i].Y = (float)(Math.Cos(obj.Vertices[i].Rad + Util.Angulo2Radiano(obj.Angulo)) * obj.Vertices[i].Raio * zoom);
            }
            obj.AtualizarXYMinMax();

            return obj;
        }

        /// <summary>
        /// Trabalha o Zoom orientado a posição do objeto em relação ao centro da camera
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private Objeto2D ZoomPosObjeto2D(Objeto2D obj, float zoom)
        {
            // TODO: Precisa rever este conceito. Há erro no cálculo!

            float radZoom = Util.Angulo2Radiano(Util.AnguloEntreDoisPontos(Pos, obj.Pos));
            float distZoom = (Util.DistanciaEntreDoisPontos(Pos, obj.Pos) * zoom);
            obj.Pos.X += (float)(Math.Cos(radZoom) * distZoom);
            obj.Pos.Y += (float)(Math.Sin(radZoom) * distZoom);
            
            return obj;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    g.Dispose();
                    render.Dispose();
                    // TODO: descartar estado gerenciado (objetos gerenciados).
                }

                // TODO: liberar recursos não gerenciados (objetos não gerenciados) e substituir um finalizador abaixo.
                // TODO: definir campos grandes como nulos.

                disposedValue = true;
            }
        }

        // TODO: substituir um finalizador somente se Dispose(bool disposing) acima tiver o código para liberar recursos não gerenciados.
        // ~Camera2D()
        // {
        //   // Não altere este código. Coloque o código de limpeza em Dispose(bool disposing) acima.
        //   Dispose(false);
        // }

        // Código adicionado para implementar corretamente o padrão descartável.
        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza em Dispose(bool disposing) acima.
            Dispose(true);
            // TODO: remover marca de comentário da linha a seguir se o finalizador for substituído acima.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
