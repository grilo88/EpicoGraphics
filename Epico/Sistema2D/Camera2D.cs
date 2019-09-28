using Epico.Luzes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Epico.Sistema;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Epico.Sistema2D
{
    public sealed class Camera2D : Objeto2D, IDisposable
    {
        readonly EpicoGraphics epico;
        Bitmap render;
        public Graphics g;

        //Stopwatch sw = new Stopwatch();

        #region Campos
        public int ResWidth;
        public int ResHeight;
        public PixelFormat FormatoPixel = PixelFormat.Format32bppArgb;
        public InterpolationMode ModoInterpolacao = InterpolationMode.Default;
        public PixelOffsetMode ModoDeslocamentoPixel = PixelOffsetMode.None;
        private int _fps;
        private int _tickFPS;
        #endregion

        #region Propriedades
        public int FPS { get; private set; }
        public int ObjetosVisiveis { get; private set; }
        private int _maxFPS { get; set; } = 60;
        private float _tickMaxFPS { get; set; }
        /// <summary>Tempo de atraso entre uma renderização e outra.</summary>
        public long TempoDelta { get; private set; }


        float _zoomCamera = 100F;

        public float ZoomCamera {
            get => _zoomCamera;
            set => _zoomCamera = value; }

        public bool DesligarSistemaZoom { get; set; } = true;
        public bool AntiSerrilhado { get; set; } = true;
        public float PontosPorPixel { get; set; } = 1;

        public float Left => Pos.X - ResWidth / 2;
        public float Right => Pos.X + ResWidth / 2;
        public float Top => Pos.Y - ResHeight / 2;
        public float Bottom => Pos.Y + ResHeight / 2;

        public bool EfeitoMotionBlur { get; set; }
        public bool Efeito3D { get; set; } = true;

        public float EixoZ => _zoomCamera * 1.2F;
        public float PosZ { get; set; } = 100F;
        #endregion

        public Camera2D(EpicoGraphics epico, int width, int height)
        {
            this.epico = epico;
            IniciarCamera(width, height, FormatoPixel);
        }

        public Camera2D(EpicoGraphics engine, int width, int height, PixelFormat FormatoPixel)
        {
            this.epico = engine;
            IniciarCamera(width, height, FormatoPixel);
        }

        private void IniciarCamera(int width, int heigth, PixelFormat formatoPixel)
        {
            Nome = "Camera";
            FormatoPixel = formatoPixel;
            ResWidth = width;
            ResHeight = heigth;
            render = new Bitmap(width, heigth, formatoPixel);
            g = Graphics.FromImage(render);
            g.SmoothingMode = AntiSerrilhado ? SmoothingMode.AntiAlias : SmoothingMode.None;
            g.InterpolationMode = ModoInterpolacao;
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
                ResHeight = height;
                render = new Bitmap(width, height, pixelFormat);

                g = Graphics.FromImage(render);
                g.SmoothingMode = AntiSerrilhado ? SmoothingMode.AntiAlias : SmoothingMode.None;
                g.InterpolationMode = ModoInterpolacao;
                g.PixelOffsetMode = ModoDeslocamentoPixel;
            }
        }
        public void DefineFPSMaximo(int maxFPS)
        {
            this._maxFPS = maxFPS;
            this._tickMaxFPS = 1000 / maxFPS;
        }

        /// <summary>
        /// Centraliza a camera no objeto
        /// </summary>
        /// <param name="obj"></param>
        public void Focar(Objeto2D obj)
        {
            Pos = PosFoco(obj);
        }

        public Vetor2 PosFoco(Objeto2D obj)
        {
            Vetor2 novaPos = (Vetor2)obj.Pos.NovaInstancia();
            novaPos.Obj = obj;

            if (obj is Controle2D)
            {     
                novaPos.X = obj.Pos.X + ((Controle2D)obj).Width / 2;
                novaPos.Y = obj.Pos.Y + ((Controle2D)obj).Height / 2;
            }
            else
            {
                novaPos.X = obj.Centro.Global.X;
                novaPos.Y = obj.Centro.Global.Y;
            }
            return novaPos;
        }

        public Vetor2 PosFoco(Eixos2 pos)
        {
            Vetor2 novaPos = new Vetor2(pos.Obj);

            if (pos is Origem2 || pos is Vertice2)
            {
                novaPos.X = pos.Global.X;
                novaPos.Y = pos.Global.Y;
            }
            else
            {
                novaPos.X = pos.X;
                novaPos.Y = pos.Y;
            }
            return novaPos;
        }

#region Atributos de otimização do Renderizador
        PointF pontoA = new PointF();
        PointF pontoB = new PointF();
        long _tickRender;

        readonly Font font_debug = new Font("Lucida Console", 12,FontStyle.Regular);
        readonly SolidBrush font_debug_color = new SolidBrush(Color.FromArgb(255, 127, 255, 212) /*Aquamarine*/);
        #endregion

        List<Objeto2D> efeito3DProjecao = new List<Objeto2D>();

        public Bitmap Renderizar()
        {
            //sw.Stop();
            //TempoDelta = sw.ElapsedMilliseconds;
            //sw.Start();

            TempoDelta = DateTime.Now.Ticks - _tickRender; // Calcula o tempo delta (tempo de atraso)
            _tickRender = DateTime.Now.Ticks;

            ObjetosVisiveis = 0;

            if (ResWidth > 0 && ResHeight > 0) // Janela não minimizada?
            {
                if (EfeitoMotionBlur)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), new Rectangle(0, 0, ResWidth, ResHeight));
                else
                    g.Clear(Color.FromArgb(255, 0, 0, 0) /*Preto*/);

                // Obtém a posição da tela da câmera

                for (int i = 0; i < epico.objetos2D.Count; i++)
                {
                    Objeto2DRenderizar obj = epico.objetos2D[i] as Objeto2DRenderizar;
                    if (obj != null)
                    {
                        Objeto2DRenderizar objProjecao = (Objeto2DRenderizar)obj.Clone();

                        if (Efeito3D)
                        {
                            objProjecao.Tag = objProjecao.Clone();
                        }

                        #region Aplica zoom e ângulo na câmera
                        Vetor2 PosCam = new Vetor2(Pos);
                        Vetor2 PosCamZoomDiff = new Vetor2();
                        PosCamZoomDiff = (Pos / PosZ * _zoomCamera - Pos);

                        Vetor2 PosCamZoomDiff_Efeito3D = new Vetor2();
                        PosCamZoomDiff_Efeito3D = Pos / PosZ * EixoZ - Pos;

                        //float cameraZ = -(((pontoAncora.X - Centro().X) * zoom) / Centro().X) + pontoAncora.Z;

                        for (int v = 0; v < objProjecao.Vertices.Count(); v++)
                        {
                            //var pontoAncora = objProjecao.Vertices[v];
                            //var cameraZ = -(((pontoAncora.X - objProjecao.Centro.X) * ZoomCamera) / objProjecao.Centro.X) + 100;

                            Vetor2 globalPos = new Vetor2(
                                objProjecao.Vertices[v].Global.X / PosZ * _zoomCamera,
                                objProjecao.Vertices[v].Global.Y / PosZ * _zoomCamera);

                            Eixos2 rot = Util2D.RotacionarPonto2D(PosCam / PosZ * _zoomCamera, globalPos, -Angulo.Z);
                            objProjecao.Vertices[v].X = rot.X - objProjecao.Pos.X;
                            objProjecao.Vertices[v].Y = rot.Y - objProjecao.Pos.Y;

                            if (Efeito3D)
                            {
                                globalPos = new Vetor2(
                                ((Objeto2D)objProjecao.Tag).Vertices[v].Global.X / PosZ * EixoZ,
                                ((Objeto2D)objProjecao.Tag).Vertices[v].Global.Y / PosZ * EixoZ);

                                rot = Util2D.RotacionarPonto2D(PosCam / PosZ * EixoZ, globalPos, -Angulo.Z);
                                ((Objeto2D)objProjecao.Tag).Vertices[v].X = rot.X - ((Objeto2D)objProjecao.Tag).Pos.X;
                                ((Objeto2D)objProjecao.Tag).Vertices[v].Y = rot.Y - ((Objeto2D)objProjecao.Tag).Pos.Y;
                            }
                        }
                        for (int c = 0; c < objProjecao.Origens.Count(); c++)
                        {
                            Vetor2 globalPos = new Vetor2(
                                objProjecao.Origens[c].Global.X / PosZ * _zoomCamera,
                                objProjecao.Origens[c].Global.Y / PosZ * _zoomCamera);
                            Eixos2 rot = Util2D.RotacionarPonto2D(PosCam / PosZ * _zoomCamera, globalPos, -Angulo.Z);
                            objProjecao.Origens[c].X = rot.X - objProjecao.Pos.X;
                            objProjecao.Origens[c].Y = rot.Y - objProjecao.Pos.Y;
                        }
                        objProjecao.AtualizarMinMax();
                        #endregion

                        if (Objeto2DVisivelCamera(obj))
                        {
                            ObjetosVisiveis++;

                            if (objProjecao.Mat_render.CorSolida.A > 0) // Pinta objeto materialmente visível
                            {
                                GraphicsPath preenche = new GraphicsPath();
                                preenche.AddLines(objProjecao.Vertices.AsEnumerable()
                                    .Select(v => new PointF(
                                    -Left + v.Global.X - PosCamZoomDiff.X,
                                    -Top + v.Global.Y - PosCamZoomDiff.Y
                                    )).ToArray());
                                g.FillPath(new SolidBrush(Color.FromArgb(objProjecao.Mat_render.CorSolida.A, objProjecao.Mat_render.CorSolida.R, objProjecao.Mat_render.CorSolida.G, objProjecao.Mat_render.CorSolida.B)), preenche);

                                if (Efeito3D)
                                {
                                    preenche = new GraphicsPath();
                                    preenche.AddLines(((Objeto2D)objProjecao.Tag).Vertices.AsEnumerable()
                                        .Select(v => new PointF(
                                        -Left + v.Global.X - PosCamZoomDiff_Efeito3D.X,
                                        -Top + v.Global.Y - PosCamZoomDiff_Efeito3D.Y
                                        )).ToArray());
                                    g.FillPath(new SolidBrush(Color.FromArgb(objProjecao.Mat_render.CorSolida.A, objProjecao.Mat_render.CorSolida.R, objProjecao.Mat_render.CorSolida.G, objProjecao.Mat_render.CorSolida.B)), preenche);
                                }
                            }

                            // Materialização do objeto na Câmera
                            Material mat;
                            if (objProjecao.Selecionado)
                                mat = objProjecao.Mat_render_sel;
                            else
                                mat = objProjecao.Mat_render;

                            if (mat.CorBorda.A > 0) // Desenha borda dos objetos materialmente visíveis
                            {
                                // Cor da borda do objeto
                                Pen pen = new Pen(new SolidBrush(Color.FromArgb(mat.CorBorda.A, mat.CorBorda.R, mat.CorBorda.G, mat.CorBorda.B)));

                                pen.Width = -Math.Abs(mat.LarguraBorda / PosZ * _zoomCamera);
                                for (int v = 1; v < objProjecao.Vertices.Count() + 1; v++)
                                {
                                    Vertice2 v1, v2;
                                    if (v == objProjecao.Vertices.Count()) // Conecta a última Vértice na primeira Vértice
                                    {
                                        v2 = objProjecao.Vertices[v - 1];     // Ponto Final
                                        v1 = objProjecao.Vertices[0];         // Ponto Inicial
                                    }
                                    else
                                    {
                                        v1 = objProjecao.Vertices[v - 1]; // Ponto A
                                        v2 = objProjecao.Vertices[v];     // Ponto B
                                    }

                                    // Desenha as linhas entre as vértices na câmera
                                    pontoA.X = -Left - PosCamZoomDiff.X + v1.Global.X;
                                    pontoA.Y = -Top - PosCamZoomDiff.Y + v1.Global.Y;
                                    pontoB.X = -Left - PosCamZoomDiff.X + v2.Global.X;
                                    pontoB.Y = -Top - PosCamZoomDiff.Y + v2.Global.Y;

                                    g.DrawLine(pen, pontoA, pontoB);
                                }

                                if (Efeito3D)
                                {
                                    for (int v = 1; v < ((Objeto2D)objProjecao.Tag).Vertices.Count() + 1; v++)
                                    {
                                        Vertice2 v1, v2;
                                        Vertice2 v1b, v2b;

                                        if (v == ((Objeto2D)objProjecao.Tag).Vertices.Count()) // Conecta a última Vértice na primeira Vértice
                                        {
                                            v2 = ((Objeto2D)objProjecao.Tag).Vertices[v - 1];     // Ponto Final
                                            v1 = ((Objeto2D)objProjecao.Tag).Vertices[0];         // Ponto Inicial

                                            v2b = objProjecao.Vertices[v - 1];     // Ponto Final
                                            v1b = objProjecao.Vertices[0];         // Ponto Inicial
                                        }
                                        else
                                        {
                                            v1 = ((Objeto2D)objProjecao.Tag).Vertices[v - 1]; // Ponto A
                                            v2 = ((Objeto2D)objProjecao.Tag).Vertices[v];     // Ponto B

                                            v1b = objProjecao.Vertices[v - 1]; // Ponto A
                                            v2b = objProjecao.Vertices[v];     // Ponto B
                                        }

                                        // Desenha as linhas entre as vértices na câmera
                                        pontoA.X = -Left - PosCamZoomDiff_Efeito3D.X + v1.Global.X;
                                        pontoA.Y = -Top - PosCamZoomDiff_Efeito3D.Y + v1.Global.Y;
                                        pontoB.X = -Left - PosCamZoomDiff_Efeito3D.X + v2.Global.X;
                                        pontoB.Y = -Top - PosCamZoomDiff_Efeito3D.Y + v2.Global.Y;

                                        g.DrawLine(pen, pontoA, pontoB);

                                        //if (i % 2 == 0)
                                        {
                                            // Conecta os pontos na coordenada Z

                                            PointF pontoZa = new PointF(), pontoZb = new PointF();

                                            pontoZa.X = -Left - PosCamZoomDiff.X + v1b.Global.X;
                                            pontoZa.Y = -Top - PosCamZoomDiff.Y + v1b.Global.Y;
                                            pontoZb.X = -Left - PosCamZoomDiff.X + v2b.Global.X;
                                            pontoZb.Y = -Top - PosCamZoomDiff.Y + v2b.Global.Y;

                                            g.DrawLine(pen, pontoA, pontoZa);
                                            g.DrawLine(pen, pontoB, pontoZb);
                                        }
                                    }
                                }
                            }

                            // Exibe as vértices do objeto
                            for (int v = 0; v < objProjecao.Vertices.Count; v++)
                            {
                                if (objProjecao.Vertices[v].Sel)
                                {
                                    float width = 5;
                                    float x = -Left - PosCamZoomDiff.X + objProjecao.Vertices[v].Global.X;
                                    float y = -Top - PosCamZoomDiff.Y + objProjecao.Vertices[v].Global.Y;
                                    RectangleF rect = new RectangleF(x - width / 2, y - width / 2, width, width);
                                    g.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 0, 0) /*Vermelho*/), rect);
                                }
                            }

                            // Exibe os pontos de origem do objeto
                            for (int c = 0; c < objProjecao.Origens.Count; c++)
                            {
                                if (objProjecao.Origens[c].Sel)
                                {
                                    float width = 5;
                                    float x = -Left - PosCamZoomDiff.X + objProjecao.Origens[c].Global.X;
                                    float y = -Top - PosCamZoomDiff.Y + objProjecao.Origens[c].Global.Y;
                                    RectangleF rect = new RectangleF(x - width / 2, y - width / 2, width, width);
                                    g.FillEllipse(new SolidBrush(Color.FromArgb(255, 255, 255, 0) /*Amarelo*/), rect);
                                }
                            }
                        }
                    }
                }

                // A iluminação deve ser renderizada após pintar todos os objetos.
                for (int i = 0; i < epico.objetos2D.Count; i++)
                {
                    Luz2DRenderizar luz = epico.objetos2D[i] as Luz2DRenderizar;
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
                if (epico.Debug)
                {
                    g.DrawString(Nome.ToUpper(), font_debug, font_debug_color, new PointF(10, 10));
                    g.DrawString("FPS: " + FPS, font_debug, font_debug_color, new PointF(10, 30));

                    string legenda = "Objetos visíveis: " + ObjetosVisiveis;
                    SizeF tam = g.MeasureString(legenda, font_debug);
                    g.DrawString(legenda, font_debug, font_debug_color, new PointF(ResWidth - tam.Width - 10, 10));
                }
                #endregion
            }

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

            return render;
        }

        /// <summary>
        /// Checa se o objeto 2D está visível na câmera
        /// </summary>
        /// <param name="obj">Objeto 2D antes da fase de projeção de tela</param>
        /// <returns></returns>
        public bool Objeto2DVisivelCamera(Objeto2D obj)
        {
            Vertice2[] rectCam = new Vertice2[4];

            // Inverte escala no tamanho de projeção da tela (Menos zoom = maior tamanho de projeção)
            float ProjLeft = Pos.X - (ResWidth / ZoomCamera * PosZ) / 2; 
            float ProjRight = Pos.X + (ResWidth / ZoomCamera * PosZ) / 2;
            float ProjTop = Pos.Y - (ResHeight / ZoomCamera * PosZ) / 2;
            float ProjBottom = Pos.Y + (ResHeight / ZoomCamera * PosZ) / 2;

            rectCam[0] = new Vertice2(Util2D.RotacionarPonto2D(Pos, new Vetor2(ProjLeft, ProjTop), Angulo.Z));          // Superior Esquerda
            rectCam[1] = new Vertice2(Util2D.RotacionarPonto2D(Pos, new Vetor2(ProjRight, ProjTop), Angulo.Z));         // Superior Direita
            rectCam[2] = new Vertice2(Util2D.RotacionarPonto2D(Pos, new Vetor2(ProjRight, ProjBottom), Angulo.Z));      // Inferior Direita
            rectCam[3] = new Vertice2(Util2D.RotacionarPonto2D(Pos, new Vetor2(ProjLeft, ProjBottom), Angulo.Z));

            return Util2D.IntersecaoEntrePoligonos(rectCam,
                obj.Vertices.Select(x => new Vertice2(x.Global.X, x.Global.Y)).ToArray());
        }

        public void DispararMouseClick(int X, int Y)
        {
            Objeto2D obj = Util2D.ObterUnicoObjeto2DMouseXY(epico, this, new Vetor2(X, Y));

            if (obj is Controle2D)
            {
                ((Controle2D)obj).OnMouseClick(new EventArgs());
            }
        }

        public void DispararMouseUp(int X, int Y)
        {
            Objeto2D obj = Util2D.ObterUnicoObjeto2DMouseXY(epico, this, new Vetor2(X, Y));

            if (obj is Controle2D)
            {
                ((Controle2D)obj).OnMouseUp(new EventArgs());
            }
        }

        public void DispararMouseDown(int X, int Y)
        {
            Objeto2D obj = Util2D.ObterUnicoObjeto2DMouseXY(epico, this, new Vetor2(X, Y));

            if (obj is Controle2D)
            {
                ((Controle2D)obj).OnMouseDown(new EventArgs());
            }
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

