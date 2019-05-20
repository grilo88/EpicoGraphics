using Epico;
using Epico.Controles;
using Epico.Controles;
using Epico.Luzes;
using Epico.Objetos2D.Avancados;
using Epico.Objetos2D.Primitivos;
using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Editor2D
{
    public partial class frmEditor : Form
    {
        bool _sair = false;
        const int _raio_padrao = 50;

        Epico2D  _engine2D = new Epico2D();
        List<Objeto2D> _obj_sel = new List<Objeto2D>();
        List<Origem2D> _origem_sel = new List<Origem2D>();
        List<Vertice2D> _vetor_sel = new List<Vertice2D>();
        List<Vertice2D> _vertice_sel = new List<Vertice2D>();

        List<ToolStripButton> _ferramentasTransformacao = new List<ToolStripButton>();
        List<ToolStripButton> _ferramentasSelecao = new List<ToolStripButton>();

        bool moveCamera = false;

        #region Retângulo da Ferramenta Multi-Seleção
        private PointF selStart;
        private const byte selAlpha = 70;
        private RectangleF selRect = new RectangleF();
        private Brush selBrush = new SolidBrush(Color.FromArgb(selAlpha, 72, 145, 220));
        #endregion

        public frmEditor()
        {
            InitializeComponent();

            _ferramentasSelecao.Add(toolStripSelecao);
            _ferramentasSelecao.Add(toolStripOrigem);
            _ferramentasSelecao.Add(toolStripVetor);
            _ferramentasSelecao.Add(toolStripVertice);

            _ferramentasTransformacao.Add(toolStripMove);
            _ferramentasTransformacao.Add(toolStripAngulo);
            _ferramentasTransformacao.Add(toolStripRaio);
            _ferramentasTransformacao.Add(toolStripEscala);

            KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Cria a Câmera
            _engine2D.CriarCamera(picScreen.ClientRectangle.Width, picScreen.ClientRectangle.Height);
            #endregion

            #region Define os atributos dos controles

            HabilitarFerramentasTransformacao(false);

            DefineMaxMinValues(
                txtCamPosX, txtCamPosY, txtCamAngulo, txtCamZoom,
                txtPosX, txtPosY, txtRaio, txtAngulo, txtEscalaX, txtEscalaY,
                txtOrigemPosX, txtOrigemPosY,
                txtVerticePosX, txtVerticePosY, txtVerticeRaio, txtVerticeAngulo);

            BtnForm2D_Click(sender, e);
            AtualizarControlesObjeto2D(_obj_sel);
            AtualizarComboObjetos2D();

            debugToolStripMenuItem.Checked = _engine2D.Debug = true;
            desligarZoomToolStripMenuItem.Checked = _engine2D.Camera.DesligarSistemaZoom = true;

            cboCamera.DisplayMember = "Nome";
            cboCamera.ValueMember = "Cam";
            cboCamera.DataSource = _engine2D.Cameras.Select(
                Cam => new
                {
                    Cam.Id,
                    Cam.Nome,
                    Cam
                }).ToList();
            #endregion

            

            Show();

            #region  Loop principal de rotinas do simulador 2D
            while (!_sair)
            {
                // Use o tempo delta em todos os cálculos que alteram o comportamento dos objetos 2d
                // para que rode em processadores de baixo e alto desempenho sem alterar a qualidade do simulador

                // TODO: Insira toda sua rotina aqui

                if (moveCamera)
                {
                    EixoXY xyCamDrag = new XY(cameraDrag.X, cameraDrag.Y);
                    EixoXY xyCursor = new XY(Cursor.Position.X, Cursor.Position.Y);
                    float distCursor = Util.DistanciaEntreDoisPontos(xyCamDrag, xyCursor);
                    float angCursor = Util.AnguloEntreDoisPontos(xyCamDrag, xyCursor);

                    _engine2D.Camera.Pos.X += (float)(Math.Cos(Util.Angulo2Radiano(angCursor + _engine2D.Camera.Angulo)) * distCursor * _engine2D.Camera.TempoDelta * 0.000001);
                    _engine2D.Camera.Pos.Y += (float)(Math.Sin(Util.Angulo2Radiano(angCursor + _engine2D.Camera.Angulo)) * distCursor * _engine2D.Camera.TempoDelta * 0.000001);
                }

                if (_engine2D.Camera.ResWidth != picScreen.ClientRectangle.Width ||
                    _engine2D.Camera.ResHeight != picScreen.ClientRectangle.Height)
                {
                    _engine2D.Camera.RedefinirResolucao(picScreen.ClientRectangle.Width, picScreen.ClientRectangle.Height);
                }

                picScreen.Image = _engine2D.Camera.Renderizar();
                Application.DoEvents();
            }
            #endregion
        }

       
        private void DefineMaxMinValues(params NumericUpDown[] numericUpDown)
        {
            numericUpDown.ToList().ForEach(x => 
            {
                x.Maximum = decimal.MaxValue;
                x.Minimum = decimal.MinValue;
            });
        }

        private void AtualizarControlesObjeto2D(List<Objeto2D> selecionados)
        {
            // Nenhum objeto selecionado?
            if (selecionados.Count == 0)
            {
                // Desabilita todas as ferramentas de transformação
                HabilitarFerramentasTransformacao(false);

                txtVisivel.Text = string.Empty;
                txtPosX.Text = string.Empty;
                txtPosY.Text = string.Empty;
                txtAngulo.Text = string.Empty;
                txtRaio.Text = string.Empty;
                txtEscalaX.Text = string.Empty;
                txtEscalaY.Text = string.Empty;

                cboVertices.DataSource = null;
                txtVerticeAngulo.Text = string.Empty;
                txtVerticePosX.Text = string.Empty;
                txtVerticePosY.Text = string.Empty;
                txtVerticeRaio.Text = string.Empty;

                propGrid.SelectedObject = null;
            }
            else
            {
                // Reabilita todas as ferramentas de transformação
                HabilitarFerramentasTransformacao(true);

                if (selecionados.Count == 1) // Único objeto selecionado?
                {
                    txtPosX.Text = selecionados.First().Pos.X.ToString();
                    txtPosY.Text = selecionados.First().Pos.Y.ToString();
                    txtAngulo.Text = selecionados.First().Angulo.ToString();
                    txtRaio.Text = selecionados.First().Raio.ToString();
                    txtEscalaX.Text = selecionados.First().Escala.X.ToString();
                    txtEscalaY.Text = selecionados.First().Escala.Y.ToString();
                    propGrid.SelectedObject = selecionados.First();
                }
                else // Muitos objetos selecionados?
                {
                    txtVisivel.Text = string.Empty;
                    txtPosX.Text = string.Empty;
                    txtPosY.Text = string.Empty;
                    txtAngulo.Text = string.Empty;
                    txtRaio.Text = string.Empty;
                    txtEscalaX.Text = string.Empty;
                    txtEscalaY.Text = string.Empty;

                    cboVertices.DataSource = null;
                    txtVerticeAngulo.Text = string.Empty;
                    txtVerticePosX.Text = string.Empty;
                    txtVerticePosY.Text = string.Empty;
                    txtVerticeRaio.Text = string.Empty;

                    propGrid.SelectedObject = null;
                }
            }
        }

        PointF cameraDrag;
        private void PicDesign_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                moveCamera = false;
                cameraDrag = Cursor.Position;
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (toolStripSelecao.Checked || toolStripVertice.Checked ||
                    toolStripOrigem.Checked || toolStripVetor.Checked)
                {
                    selStart = e.Location;
                }
            }
        }

        private Vetor2D PosAleatorio(Objeto2D obj)
        {
            int x = new Random(Environment.TickCount).Next(0, picScreen.ClientRectangle.Width);
            int y = new Random(Environment.TickCount + x).Next(0, picScreen.ClientRectangle.Height);

            _engine2D.Camera.Pos.X = x;
            _engine2D.Camera.Pos.Y = y;

            return new Vetor2D(obj, x, y);
        }

        private void AtualizarComboObjetos2D()
        {
            cboObjeto2D.BeginUpdate();
            cboObjeto2D.DisplayMember = "Nome";
            cboObjeto2D.ValueMember = "o";
            cboObjeto2D.DataSource = _engine2D.objetos
                .Select(o => new
                {
                    o.Id,
                    o.Nome,
                    o
                }).ToList();
            cboObjeto2D.EndUpdate();
        }

        private void BtnQuadrado_Click(object sender, EventArgs e)
        {
            Quadrado obj = new Quadrado(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }


        private void BtnCirculo_Click(object sender, EventArgs e)
        {
            Circulo obj = new Circulo(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnTriangulo_Click(object sender, EventArgs e)
        {
            Triangulo obj = new Triangulo(_raio_padrao);
            obj.Pos = PosAleatorio(obj);

            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnPentagono_Click(object sender, EventArgs e)
        {
            Pentagono obj = new Pentagono(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnHexagono_Click(object sender, EventArgs e)
        {
            Hexagono obj = new Hexagono(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;

        }

        private void BtnLosango_Click(object sender, EventArgs e)
        {
            Losango obj = new Losango(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));

            
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnTrianguloRetangulo_Click(object sender, EventArgs e)
        {
            TrianguloRetangulo obj = new TrianguloRetangulo(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnRetangulo_Click(object sender, EventArgs e)
        {
            Retangulo obj = new Retangulo(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void TxtNome_TextChanged(object sender, EventArgs e)
        {
            if (_obj_sel.Count == 1 && cboObjeto2D.Focused)
            {
                _obj_sel.First().Nome = cboObjeto2D.Text;
                propGrid.Refresh();
            }
        }

        private void TxtAngulo_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtAngulo.Focused)
            {
                if (float.TryParse(txtAngulo.Text, out float angulo))
                {
                    _obj_sel.First().DefinirAngulo(angulo);
                    propGrid.Refresh();
                }
            }
        }

        private void TxtRaio_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtRaio.Focused)
            {
                if (float.TryParse(txtRaio.Text, out float raio))
                {
                    _obj_sel.First().DefinirRaio(raio);
                    propGrid.Refresh();
                }
            }
        }

        private void TxtPosY_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtPosY.Focused)
            {
                if (float.TryParse(txtPosY.Text, out float posY))
                {
                    _obj_sel.First().PosicionarY(posY);
                    propGrid.Refresh();
                }
            }
        }

        private void TxtPosX_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtPosX.Focused)
            {
                if (float.TryParse(txtPosX.Text, out float posX))
                {
                    _obj_sel.First().PosicionarX(posX);
                    propGrid.Refresh();
                }
            }
        }

        private void PicDesign_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                if (toolStripSelecao.Checked || toolStripVertice.Checked ||
                    toolStripOrigem.Checked || toolStripVetor.Checked)
                {
                    // Retângulo Multi-Seleção
                    PointF tempEndPoint = e.Location;
                    selRect.Location = new PointF(
                        Math.Min(selStart.X, tempEndPoint.X),
                        Math.Min(selStart.Y, tempEndPoint.Y));
                    selRect.Size = new SizeF(
                        Math.Abs(selStart.X - tempEndPoint.X),
                        Math.Abs(selStart.Y - tempEndPoint.Y));
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                moveCamera = true;
            }
        }

        private void TxtCamPosX_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamPosX.Focused)
            {
                if (float.TryParse(txtCamPosX.Text, out float camPosX))
                {
                    _engine2D.Camera.Pos.X = camPosX;
                    propGrid.Refresh();
                }
            }
        }

        private void TxtCamPosY_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamPosY.Focused)
            {
                if (float.TryParse(txtCamPosY.Text, out float camPosY))
                {
                    _engine2D.Camera.Pos.Y = camPosY;
                    propGrid.Refresh();
                }
            }
        }


        private void TxtEscalaY_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtEscalaY.Focused)
            {
                if (float.TryParse(txtEscalaY.Text, out float escalaY))
                {
                    _obj_sel.First().DefinirEscalaY(escalaY);
                    propGrid.Refresh();
                }
            }
        }
        private void TxtEscalaX_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtEscalaX.Focused)
            {
                if (float.TryParse(txtEscalaX.Text, out float escalaX))
                {
                    _obj_sel.First().DefinirEscalaX(escalaX);
                    propGrid.Refresh();
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void TmrObjeto2D_Tick(object sender, EventArgs e)
        {
            if (_sair) return;
            
            if (_obj_sel.Count == 1)
            {
                if (_engine2D.Camera.Objeto2DVisivelCamera(_obj_sel.First()))
                    txtVisivel.Text = "Sim";
                else
                    txtVisivel.Text = "Não";
            }

            if (!txtCamPosX.Focused) txtCamPosX.Value = (decimal)_engine2D.Camera.Pos.X;
            if (!txtCamPosY.Focused) txtCamPosY.Value = (decimal)_engine2D.Camera.Pos.Y;
            if (!txtCamAngulo.Focused) txtCamAngulo.Value = (decimal)_engine2D.Camera.Angulo;
            if (!txtCamZoom.Focused) txtCamZoom.Value = (decimal)_engine2D.Camera.ZoomCamera;
        }

        private void BtnVarios_Click(object sender, EventArgs e)
        {
            int quant = 50 / 4;

            for (int i = 0; i < quant; i++)
            {
                BtnCirculo_Click(sender, e);
                Thread.Sleep(20);
                BtnTriangulo_Click(sender, e);
                Thread.Sleep(20);
                BtnQuadrado_Click(sender, e);
                Thread.Sleep(20);
                BtnPentagono_Click(sender, e);
                Thread.Sleep(20);
            }
        }

        private void BtnFocarObjeto_Click(object sender, EventArgs e)
        {
            if (cboObjeto2D.SelectedValue != null)
            {
                _engine2D.Camera.Focar((Objeto2D)cboObjeto2D.SelectedValue);
            }
        }

        private void CboObjeto2D_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboObjeto2D.SelectedValue != null)
            {
                _obj_sel.ForEach(x => x.Selecionado = false);
                _obj_sel.Clear();
                _obj_sel.Add((Objeto2D)cboObjeto2D.SelectedValue);
                _obj_sel.First().Selecionado = true;
                AtualizarControlesObjeto2D(_obj_sel);
                
                //_vertices_sel.ForEach(x => x.sel = false);
                //_vertices_sel.Clear();

                LimparSelecoesGeometricas();

                #region Vértices
                cboVertices.BeginUpdate();
                cboVertices.DisplayMember = "i";
                cboVertices.ValueMember = "v";
                cboVertices.DataSource = _obj_sel.First().Vertices.Select(
                (v, i) => new
                {
                    i = "Vértice " + i,
                    v
                }).ToList();
                cboVertices.EndUpdate();
                #endregion

                #region Pontos Centrais
                cboOrigem.BeginUpdate();
                cboOrigem.DisplayMember = "i";
                cboOrigem.ValueMember = "c";
                cboOrigem.DataSource = _obj_sel.First().Origem.Select(
                (c, i) => new
                {
                    i = "Ponto " + i,
                    c
                }).ToList();
                cboOrigem.EndUpdate();
                #endregion
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        [DllImport("user32")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        // you also need ReleaseDC
        [DllImport("user32")]
        private static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);
        private void TelaCheiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //IntPtr hdc = GetWindowDC(this.Handle);
            //Graphics g = Graphics.FromHdc(hdc);

            //engine2D.Camera.g = g;
        }

        private void FPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fPSToolStripMenuItem.Checked = !fPSToolStripMenuItem.Checked;
            _engine2D.Debug = fPSToolStripMenuItem.Checked;
        }

        private void BtnNovaCamera_Click(object sender, EventArgs e)
        {
            #region Cria a Câmera 2D
            Camera2D camera = _engine2D.CriarCamera(picScreen.Width, picScreen.Height);
            camera.Pos = new Vetor2D(_obj_sel.First().Pos.X, _obj_sel.First().Pos.Y);
            #endregion

            cboCamera.DataSource = _engine2D.Cameras
                .Select(cam => new
                {
                    cam.Id,
                    cam.Nome,
                    cam
                }).ToList();

            cboCamera.SelectedValue = camera;
        }

        private void CboCamera_SelectedValueChanged(object sender, EventArgs e)
        {
            _engine2D.Camera = (Camera2D)cboCamera.SelectedValue;
        }

        private void BtnQuadrilatero_Click(object sender, EventArgs e)
        {
            Quadrilatero obj = new Quadrilatero();
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.GerarGeometria(rnd.Next(0, 359), _raio_padrao, (int)(_raio_padrao * 1.5F));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnEstrela_Click(object sender, EventArgs e)
        {
            Estrela obj = new Estrela(_raio_padrao);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;

        }


        private void BtnEstrelaQuaPon_Click(object sender, EventArgs e)
        {
            Estrela obj = new Estrela(null, _raio_padrao, 8);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnEstrelaSeisPontas_Click(object sender, EventArgs e)
        {
            Estrela obj = new Estrela(null, _raio_padrao, 12);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void PicScreen_Paint(object sender, PaintEventArgs e)
        {
            if (picScreen.Image != null)
            {
                if (selRect.Width > 0 && selRect.Height > 0) // Retângulo
                {
                    // Desenha o retângulo multi-seleção na tela
                    e.Graphics.FillRectangle(selBrush, selRect);

                    #region Retângulo de colisão para o mundo 2D
                    Vertice2D[] rect = new Vertice2D[4];
                    rect[0] = new Vertice2D(selRect.X, selRect.Y);                                      // Superior Esquerdo
                    rect[1] = new Vertice2D(selRect.X + selRect.Width, selRect.Y);                      // Superior Direito
                    rect[2] = new Vertice2D(selRect.X + selRect.Width, selRect.Y + selRect.Height);     // Inferior Direito
                    rect[3] = new Vertice2D(selRect.X, selRect.Y + selRect.Height);                     // Inferior Esquerdo
                    #endregion

                    if (toolStripSelecao.Checked)
                    {
                        _obj_sel.ForEach(x => x.Selecionado = false);
                        _obj_sel.Clear();
                        _obj_sel = _engine2D.ObterObjetos2DPelaTela(_engine2D.Camera, rect).ToList();
                        _obj_sel.ForEach(x => x.Selecionado = true);

                        // Informa a quantidade de objetos presentes na área do retângulo
                        var tmp = Util.ObterObjetos2DPelaTela(_engine2D, _engine2D.Camera, rect);
                        e.Graphics.DrawString(
                            $"{tmp.Count()} objetos", new Font("Lucida Console", 10),
                            new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                            new RectangleF(selRect.Location, selRect.Size),
                            new StringFormat(StringFormatFlags.NoWrap));
                    }
                    else if (toolStripOrigem.Checked)
                    {
                        // Informa a quantidade de objetos presentes na área do retângulo
                        _origem_sel.ForEach(x => x.Sel = false);
                        _origem_sel.Clear();
                        _origem_sel = Util.ObterOrigensObjeto2DPelaTela(_engine2D.Camera, _obj_sel, rect).ToList();
                        _origem_sel.ForEach(x => x.Sel = true);

                        e.Graphics.DrawString(
                            $"{_origem_sel.Count()} origens", new Font("Lucida Console", 10),
                            new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                            new RectangleF(selRect.Location, selRect.Size),
                            new StringFormat(StringFormatFlags.NoWrap));
                    }
                    else if (toolStripVetor.Checked)
                    {
                        // Informa a quantidade de objetos presentes na área do retângulo
                        _vetor_sel.ForEach(x => x.Sel = false);
                        _vetor_sel.Clear();
                        _vetor_sel = Util.ObterVetoresObjeto2DPelaTela(_engine2D.Camera, _obj_sel, selRect).ToList();
                        _vetor_sel.ForEach(x => x.Sel = true);

                        e.Graphics.DrawString(
                            $"{_vetor_sel.Count()} vetores", new Font("Lucida Console", 10),
                            new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                            new RectangleF(selRect.Location, selRect.Size),
                            new StringFormat(StringFormatFlags.NoWrap));
                    }
                    else if (toolStripVertice.Checked)
                    {
                        // Informa a quantidade de objetos presentes na área do retângulo
                        _vertice_sel.ForEach(x => x.Sel = false);
                        _vertice_sel.Clear();
                        _vertice_sel = Util.ObterVerticesObjeto2DPelaTela(_engine2D.Camera, _obj_sel, rect).ToList();
                        _vertice_sel.ForEach(x => x.Sel = true);

                        e.Graphics.DrawString(
                            $"{_vertice_sel.Count()} vértices", new Font("Lucida Console", 10),
                            new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                            new RectangleF(selRect.Location, selRect.Size),
                            new StringFormat(StringFormatFlags.NoWrap));
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
        }

        private void TxtCamZoom_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamZoom.Focused)
            {
                if (float.TryParse(txtCamZoom.Text, out float camZoom))
                {
                    _engine2D.Camera.DefinirZoom(camZoom);
                    propGrid.Refresh();
                }
            }
        }

        private void DesligarZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            desligarZoomToolStripMenuItem.Checked = !desligarZoomToolStripMenuItem.Checked;
            _engine2D.Camera.DesligarSistemaZoom = desligarZoomToolStripMenuItem.Checked;
        }

        private void BtnDeformado_Click(object sender, EventArgs e)
        {
            Deformado obj = new Deformado();
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));

            obj.GerarGeometria(0, 5, 50);
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnLuzPonto_Click(object sender, EventArgs e)
        {
            LuzPonto obj = new LuzPonto(150, 150);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnLuzDirecional_Click(object sender, EventArgs e)
        {
            // TODO: Luz Ambiente
            throw new NotImplementedException();
        }

        private void BtnLuzDestaque_Click(object sender, EventArgs e)
        {
            // TODO: Luz Lanterna
            throw new NotImplementedException();
        }

        private void CboVertices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboVertices.SelectedValue != null)
            {
                _vertice_sel.ForEach(x => x.Sel = false);
                _vertice_sel.Clear();
                _vertice_sel.Add((Vertice2D)cboVertices.SelectedValue);
                if (toolStripVertice.Checked) _vertice_sel.First().Sel = true;
                AtualizarControlesVertice(_vertice_sel);
            }
        }

        private void AtualizarControlesVertice(List<Vertice2D> selecionados)
        {
            if (selecionados.Count == 1)
            {
                txtVerticePosX.Text = selecionados.First().X.ToString();
                txtVerticePosY.Text = selecionados.First().Y.ToString();
                txtVerticeAngulo.Text = selecionados.First().Ang.ToString();
                txtVerticeRaio.Text = selecionados.First().Raio.ToString();
                propGrid.SelectedObject = selecionados.First();
            }
            else
            {
                txtVerticePosX.Text = string.Empty;
                txtVerticePosY.Text = string.Empty;
                txtVerticeAngulo.Text = string.Empty;
                txtVerticeRaio.Text = string.Empty;
                propGrid.SelectedObject = null;
            }
        }

        private void PicScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (toolStripSelecao.Checked) // Ferramenta Seleção de Objetos
                {
                    if (selRect.Width == 0 && selRect.Height == 0) // Não formou retângulo multi-seleção?
                    {
                        _vertice_sel.ForEach(x => x.Sel = false);
                        _vertice_sel.Clear();
                        _obj_sel.ForEach(x => x.Selecionado = false);
                        Objeto2D objSel = Util.ObterUnicoObjeto2DPelaTela(_engine2D, _engine2D.Camera, new XY(selStart.X, selStart.Y));
                        _obj_sel = new List<Objeto2D>();
                        if (objSel != null) _obj_sel.Add(objSel);
                    }

                    if (_obj_sel.Count() == 0) // Nenhum objeto selecionado?
                    {
                        cboObjeto2D.SelectedIndex = -1;
                    }
                    else if (_obj_sel.Count == 1) // Um objeto selecionado?
                    {
                        cboObjeto2D.SelectedValue = _obj_sel.First();
                        _obj_sel.First().Selecionado = true;
                    }

                    AtualizarControlesObjeto2D(_obj_sel);

                    selRect = new RectangleF();
                }
                else if (toolStripVertice.Checked) // Ferramenta Seleção de Vértice
                {
                    selRect = new RectangleF();
                }
                else if (toolStripOrigem.Checked) // Ferramenta Seleção de Vértice
                {
                    selRect = new RectangleF();
                }
                else if (toolStripVetor.Checked) // Ferramenta Seleção de Vértice
                {
                    selRect = new RectangleF();
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                moveCamera = false;
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (selRect.Contains(e.Location))
                    {
                        Debug.WriteLine("Clique Direito");
                    }
                }
            }
        }

        private void ToolStripSelecao_Click(object sender, EventArgs e)
        {
            ResetaFerramentasSelecao((ToolStripButton)sender);
            HabilitarFerramentasTransformacao();
            toolStripSelecao.Checked = true;
            LimparSelecoesGeometricas();
            tabControlObjeto.SelectedTab = tabObjeto;
        }

        private void ToolStripVertice_Click(object sender, EventArgs e)
        {
            ResetaFerramentasSelecao((ToolStripButton)sender);
            HabilitarFerramentasTransformacao(toolStripEscala);
            toolStripEscala.Enabled = false;
            toolStripVertice.Checked = true;
            LimparSelecoesGeometricas();
            tabControlObjeto.SelectedTab = tabVertice;

            CboVertices_SelectedIndexChanged(sender, new EventArgs()); // Exibe a vértice selecionada
        }

        private void ToolStripMove_Click(object sender, EventArgs e)
        {
            ResetaFerramentasTransformacao((ToolStripButton)sender);
            toolStripMove.Checked = true;
        }

        private void ToolStripEscala_Click(object sender, EventArgs e)
        {
            ResetaFerramentasTransformacao((ToolStripButton)sender);
            toolStripEscala.Checked = true;
        }

        private void ToolStripAngulo_Click(object sender, EventArgs e)
        {
            ResetaFerramentasTransformacao((ToolStripButton)sender);
            toolStripAngulo.Checked = true;
        }

        private void ToolStripRaio_Click(object sender, EventArgs e)
        {
            ResetaFerramentasTransformacao((ToolStripButton)sender);
            toolStripRaio.Checked = true;
        }

        /// <summary>
        /// Habilita todas as ferramentas de transformação e desabilita as ferramentas informadas no parâmetro.
        /// </summary>
        /// <param name="desabilitar"></param>
        private void HabilitarFerramentasTransformacao(params ToolStripButton[] desabilitar)
        {
            _ferramentasTransformacao.ForEach(x => x.Enabled = true);
            desabilitar.ToList().ForEach(x => x.Enabled = false);
        }

        private void HabilitarFerramentasTransformacao(bool habilitar)
        {
            _ferramentasTransformacao.ForEach(x => x.Enabled = habilitar);
        }

        private void ResetaFerramentasTransformacao(ToolStripButton exceto)
        {
            _ferramentasTransformacao.Except(new List<ToolStripButton>() { exceto })
                .ToList().ForEach(x => x.Checked = false);
        }

        private void ResetaFerramentasSelecao(ToolStripButton exceto)
        {
            _ferramentasSelecao.Except(new List<ToolStripButton>() { exceto })
                .ToList().ForEach(x => x.Checked = false);
        }

        private void ToolStripVetor_Click(object sender, EventArgs e)
        {
            ResetaFerramentasSelecao((ToolStripButton)sender);
            HabilitarFerramentasTransformacao();
            toolStripVetor.Checked = true;
            LimparSelecoesGeometricas();
            tabControlObjeto.SelectedTab = tabVetor;
        }

        private void ToolStripOrigem_Click(object sender, EventArgs e)
        {
            ResetaFerramentasSelecao((ToolStripButton)sender);
            HabilitarFerramentasTransformacao(toolStripRaio, toolStripEscala);
            toolStripOrigem.Checked = true;
            LimparSelecoesGeometricas();
            tabControlObjeto.SelectedTab = tabOrigem;

            CboOrigem_SelectedIndexChanged(sender, new EventArgs()); // Exibe o ponto de origem
        }

        private void LimparSelecoesGeometricas()
        {
            _origem_sel.ForEach(x => x.Sel = false);
            _origem_sel.Clear();

            _vetor_sel.ForEach(x => x.Sel = false);
            _vetor_sel.Clear();

            _vertice_sel.ForEach(x => x.Sel = false);
            _vertice_sel.Clear();
        }

        private void TabControlObjeto_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabObjeto)
            {
                ToolStripSelecao_Click(toolStripSelecao, new EventArgs());
            }
            else if (e.TabPage == tabOrigem)
            {
                ToolStripOrigem_Click(toolStripOrigem, new EventArgs());
            }
            else if (e.TabPage == tabVetor)
            {
                ToolStripVetor_Click(toolStripVetor, new EventArgs());
            }
            else if (e.TabPage == tabVertice)
            {
                ToolStripVertice_Click(toolStripVertice, new EventArgs());
            }
        }

        private void CboOrigem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboOrigem.SelectedValue != null)
            {
                _origem_sel.ForEach(x => x.Sel = false);
                _origem_sel.Clear();
                _origem_sel.Add((Origem2D)cboOrigem.SelectedValue);
                if (toolStripOrigem.Checked) _origem_sel.First().Sel = true;
                AtualizarControlesPontoCentro(_origem_sel);
            }
        }

        private void AtualizarControlesPontoCentro(List<Origem2D> selecionados)
        {
            // Nenhum objeto selecionado?
            if (selecionados.Count == 0)
            {
                txtOrigemPosX.Text = string.Empty;
                txtOrigemPosY.Text = string.Empty;
                propGrid.SelectedObject = null;
            }
            else
            {
                if (selecionados.Count == 1) // Único objeto selecionado?
                {
                    txtOrigemPosX.Text = selecionados.First().X.ToString();
                    txtOrigemPosY.Text = selecionados.First().Y.ToString();
                    propGrid.SelectedObject = selecionados.First();
                }
                else // Muitos objetos selecionados?
                {
                    txtOrigemPosX.Text = string.Empty;
                    txtOrigemPosY.Text = string.Empty;
                    propGrid.SelectedObject = null;
                }
            }
        }

        private void TxtOrigemPosX_ValueChanged(object sender, EventArgs e)
        {
            if (_origem_sel != null && txtOrigemPosX.Focused)
            {
                if (float.TryParse(txtOrigemPosX.Text, out float posX))
                {
                    _origem_sel.First().X = posX;
                    propGrid.Refresh();
                }
            }
        }

        private void TxtOrigemPosY_ValueChanged(object sender, EventArgs e)
        {
            if (_origem_sel != null && txtOrigemPosY.Focused)
            {
                if (float.TryParse(txtOrigemPosY.Text, out float posY))
                {
                    _origem_sel.First().Y = posY;
                    propGrid.Refresh();
                }
            }
        }

        private void BtnFocarOrigem_Click(object sender, EventArgs e)
        {
            if (cboOrigem.SelectedValue != null)
            {
                _engine2D.Camera.Focar((Origem2D)cboOrigem.SelectedValue);
            }
        }

        private void TxtVerticePosX_ValueChanged(object sender, EventArgs e)
        {
            if (_vertice_sel != null && txtVerticePosX.Focused)
            {
                if (float.TryParse(txtVerticePosX.Text, out float posX))
                {
                    // TODO: Ao alterar posY deve recalcular o angulo e o radiano com base
                    // nas novas coordenadas de PosX e PosY
                    _vertice_sel.First().X = posX;
                    propGrid.Refresh();
                }
            }
        }

        private void TxtVerticePosY_ValueChanged(object sender, EventArgs e)
        {
            if (_vertice_sel != null && txtVerticePosY.Focused)
            {
                if (float.TryParse(txtVerticePosY.Text, out float posY))
                {
                    // TODO: Ao alterar posY deve recalcular o angulo e o radiano com base
                    // nas novas coordenadas de PosX e PosY
                    _vertice_sel.First().Y = posY;
                    propGrid.Refresh();
                }
            }
        }

        private void TxtVerticeRaio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void TxtVerticeAngulo_ValueChanged(object sender, EventArgs e)
        {

        }

        private void TxtCamAngulo_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamAngulo.Focused)
            {
                if (float.TryParse(txtCamAngulo.Text, out float ang))
                {
                    _engine2D.Camera.Angulo = ang;
                    propGrid.Refresh();
                }
            }
        }

        private void BtnForm2D_Click(object sender, EventArgs e)
        {
            Form2D form = new Form2D();
            form.Pos = new Vetor2D(form, 200, 200);
            _engine2D.AddObjeto(form);
            _engine2D.Camera.Focar(form);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = form;
        }

        private void FrmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            _sair = true;
        }

        private void BtnPanel_Click(object sender, EventArgs e)
        {
            if (!(cboObjeto2D.SelectedValue is Controle2D))
            {
                MessageBox.Show(this, "Primeiro selecione um controle 2d antes de adicionar este controle.", "Controle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Panel2D panel = new Panel2D(_engine2D, (Controle2D)cboObjeto2D.SelectedValue);
            panel.MouseDown += Panel_MouseDown;
            panel.Pos = new Vetor2D(panel, 200, 200);
            _engine2D.AddObjeto(panel);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = panel;
        }

        private void Panel_MouseDown(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void test()
        {
            
        }

        private void GroupCamera_Enter(object sender, EventArgs e)
        {
            propGrid.SelectedObject = cboCamera.SelectedValue;
            propGrid.Refresh();
        }

        private void TabObjeto_Enter(object sender, EventArgs e)
        {
            propGrid.SelectedObject = cboObjeto2D.SelectedValue;
            propGrid.Refresh();
        }

        private void TabOrigem_Enter(object sender, EventArgs e)
        {
            propGrid.SelectedObject = cboOrigem.SelectedValue;
            propGrid.Refresh();
        }

        private void TabVertice_Enter(object sender, EventArgs e)
        {
            propGrid.SelectedObject = cboVertices.SelectedValue;
            propGrid.Refresh();
        }

        private void TabVetor_Enter(object sender, EventArgs e)
        {
            // TODO: Implementar cboVetor
            propGrid.SelectedObject = null;
            propGrid.Refresh();
        }

        private void PropGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            propGrid.Refresh();
        }

        private void PropGrid_SelectedObjectsChanged(object sender, EventArgs e)
        {
            if (propGrid.SelectedObject != null)
            {
            }
        }

        private void PicScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void FrmEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (picScreen.Focused)
                {
                    _engine2D.objetos.Remove((Objeto2D)cboObjeto2D.SelectedItem);
                    //_obj_sel.Clear();
                    //AtualizarControlesObjeto2D(_obj_sel);

                }
            }
        }

        private void PicScreen_Click(object sender, EventArgs e)
        {

        }

        private void BtnButton_Click(object sender, EventArgs e)
        {
            if (!(cboObjeto2D.SelectedValue is Controle2D))
            {
                MessageBox.Show(this, "Primeiro selecione um controle 2d antes de adicionar este controle.", "Controle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Button2D obj = new Button2D(_engine2D, (Controle2D)cboObjeto2D.SelectedValue);
            obj.MouseDown += Panel_MouseDown;
            obj.Pos = new Vetor2D(obj, 200, 200);
            _engine2D.AddObjeto(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void MultiplicarQuadrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multiplicarQuadrosToolStripMenuItem.Checked = !multiplicarQuadrosToolStripMenuItem.Checked;
            _engine2D.Camera.EfeitoQuadroDuplicado = multiplicarQuadrosToolStripMenuItem.Checked;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
