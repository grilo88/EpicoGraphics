using Epico.Objetos2D.Avancados;
using Epico.Objetos2D.Primitivos;
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
using System.Windows.Input;
using Epico;
using Epico.Sistema2D;
using Epico.Luzes;
using Epico.Controles;
using Epico.Sistema;

namespace Editor2D
{
    enum SentidoEixos
    {
        ESPACIAL, OBJETO, CAMERA
    }

    public partial class frmEditor : Form
    {
        SentidoEixos SentidoManipulaObjeto = SentidoEixos.ESPACIAL;

        bool _sair = false;
        const int _raio_padrao = 50;

        Vetor3 _novaPosCamera = new Vetor3();

        Vetor3 _novoAnguloCamera = new Vetor3();
        float _novoZoomCamera = 1;
        float _anguloZ_SentidoManipulaObjeto = 0F;

        MouseEventArgs _mouseDown = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        MouseEventArgs _mouseUp = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        MouseEventArgs _mouseMove = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        KeyEventArgs _keyDown = new KeyEventArgs(Keys.None);
        KeyEventArgs _keyUp = new KeyEventArgs(Keys.None);

        EpicoGraphics  _epico = new EpicoGraphics();
        List<Objeto2D> _obj_sel = new List<Objeto2D>();
        List<Origem2> _origem_sel = new List<Origem2>();
        List<Vertice2> _vetor_sel = new List<Vertice2>();
        List<Vertice2> _vertice_sel = new List<Vertice2>();
        
        List<ToolStripButton> _ferramentasSelecao = new List<ToolStripButton>();
        List<ToolStripButton> _ferramentasTransformacao = new List<ToolStripButton>();
        List<ToolStripButton> _ferramentasOrientacao = new List<ToolStripButton>();

        bool _moveCamera = false;

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

            _ferramentasOrientacao.Add(toolStripOrientacaoEspacial);
            _ferramentasOrientacao.Add(toolStripOrientacaoObjeto);
            _ferramentasOrientacao.Add(toolStripOrientacaoCamera);

            KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _epico.CriarCamera(picScreen.ClientRectangle.Width, picScreen.ClientRectangle.Height);

            #region Define os atributos dos controles

            HabilitarFerramentasTransformacao(false);

            DefineMaxMinValues(
                txtCamPosX, txtCamPosY, txtCamPosZ, txtCamAngulo, txtCamZoom,
                txtPosX, txtPosY, txtRaio, txtAngulo, txtEscalaX, txtEscalaY,
                txtOrigemPosX, txtOrigemPosY,
                txtVerticePosX, txtVerticePosY, txtVerticeRaio, txtVerticeAngulo);

            //BtnForm2D_Click(sender, e);
            AtualizarControlesObjeto2D(_obj_sel);
            AtualizarComboObjetos2D();

            debugToolStripMenuItem.Checked = _epico.Debug = true;
            desligarZoomToolStripMenuItem.Checked = _epico.Camera.DesligarSistemaZoom = true;

            cboCamera.DisplayMember = "Nome";
            cboCamera.ValueMember = "Cam";
            cboCamera.DataSource = _epico.Cameras.Select(
                Cam => new
                {
                    Cam.Nome,
                    Cam
                }).ToList();
            #endregion

            _epico.Camera.Pos = new Vetor2(_novaPosCamera.X, _novaPosCamera.Y);
            _novaPosCamera.Z = _epico.Camera.PosZ;

            Show();

            #region  Loop principal de rotinas do simulador 2D
            while (!_sair)
            {
                // Use o tempo delta em todos os cálculos que alteram o comportamento dos objetos 2d
                // para que rode em processadores de baixo e alto desempenho sem alterar a qualidade do simulador

                // TODO: Insira toda sua rotina aqui

                if (_moveCamera)
                {
                    Vetor2 xyCamDrag = new Vetor2(cameraDrag.X, cameraDrag.Y);
                    Vetor2 xyCursor = new Vetor2(Cursor.Position.X, Cursor.Position.Y);
                    float distCursor = Util2D.DistanciaEntreDoisPontos(xyCamDrag, xyCursor);
                    float angCursor = Util2D.AnguloEntreDoisPontos(xyCamDrag, xyCursor);

                    _novaPosCamera.X += (float)(Math.Cos(Util2D.Angulo2Radiano(angCursor + _epico.Camera.Angulo.Z)) * distCursor * _epico.Camera.TempoDelta * 0.000001);
                    _novaPosCamera.Y += (float)(Math.Sin(Util2D.Angulo2Radiano(angCursor + _epico.Camera.Angulo.Z)) * distCursor * _epico.Camera.TempoDelta * 0.000001);
                }

                #region Efeito suave no ângulo da câmera
                _epico.Camera.Angulo = Eixos.Lerp(
                    _epico.Camera.Angulo, _novoAnguloCamera, 1F * _epico.Camera.TempoDelta * 0.000001F, out bool lerpAngCamCompletado);

                if (lerpAngCamCompletado)
                {
                    propGrid.Refresh();
                }
                #endregion

                #region Efeito suave no Zoom da câmera
                _epico.Camera.ZoomCamera = Eixos.Lerp(
                    _epico.Camera.ZoomCamera, _novoZoomCamera, 1F * _epico.Camera.TempoDelta * 0.000001F, out bool lerpZoomCamCompletado);

                if (lerpZoomCamCompletado)
                {
                    propGrid.Refresh();
                }
                #endregion

                #region Efeito suave na transladação da câmera

                _epico.Camera.Pos = Eixos.Lerp(
                    _epico.Camera.Pos, new Vetor2(_novaPosCamera.X, _novaPosCamera.Y), 1F * _epico.Camera.TempoDelta * 0.000001F, out bool lerpPosCamCompletado);

                _epico.Camera.PosZ = Eixos.Lerp(
                    _epico.Camera.PosZ, _novaPosCamera.Z, 1F * _epico.Camera.TempoDelta * 0.000001F, out bool lerpPosZCamCompletado);

                if (lerpPosCamCompletado && lerpPosZCamCompletado)
                {
                    propGrid.Refresh();
                }
                #endregion

                #region Reajuste na resolução da tela
                if (_epico.Camera.ResWidth != picScreen.ClientRectangle.Width ||
                    _epico.Camera.ResHeight != picScreen.ClientRectangle.Height)
                {
                    _epico.Camera.RedefinirResolucao(picScreen.ClientRectangle.Width, picScreen.ClientRectangle.Height);
                }
                #endregion

                #region Renderização
                picScreen.Image = _epico.Camera.Renderizar();
                Application.DoEvents();
                #endregion
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
        Eixos2 objetoDragMove = null;
        Eixos3 objetoDragAng = null;

        Eixos2 objetoDragMoveDiff = new Vetor2();
        Eixos3 objetoDragAngDiff = new Vetor3();

        private void PicDesign_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = e;

            rotacaoVoltas = 0;
            direcaoRotacao = 1;
            direcaoRotacaoAnterior = 1;
            quadranteAnteriorRotacao = 1;

            if (_obj_sel.Count == 1) // 1 Objeto selecionado
            {
                Objeto2D obj = ((Objeto2D)cboObjeto2D.SelectedValue);

                // Intercepta o cursor do mouse sobre o objeto
                Eixos2 pos2D = Util2D.ObterPosEspaco2DMouseXY((Camera2D)cboCamera.SelectedValue, new Vertice2(e.X, e.Y));
                bool cliqueSobreObjeto = Util2D.IntersecaoEntrePoligonos(new Vertice2[] { new Vertice2(pos2D.X, pos2D.Y) },
                    obj.Vertices.Select(v => new Vertice2(v.Global.X, v.Global.Y)).ToArray());

                if (toolStripMove.Checked)
                {
                    if (cliqueSobreObjeto)
                    {
                        objetoDragMove = new Vetor2(pos2D);
                        objetoDragMoveDiff.X = pos2D.X - obj.Pos.X;
                        objetoDragMoveDiff.Y = pos2D.Y - obj.Pos.Y;
                    }
                    else
                    {
                        objetoDragMove = null;
                    }
                }
                else if (toolStripAngulo.Checked)
                {
                    if (cliqueSobreObjeto)
                    {
                        objetoDragAng = new Vetor3(obj.Angulo);

                        Vetor2 centro = new Vetor2(obj);
                        if (toolStripSelecao.Checked || toolStripRaio.Checked || toolStripEscala.Checked)
                        {
                            centro.X = obj.Centro.X;
                            centro.Y = obj.Centro.Y;
                        }
                        else if (toolStripOrigem.Checked)
                        {
                            centro.X = ((Origem2)cboOrigem.SelectedValue).X;
                            centro.Y = ((Origem2)cboOrigem.SelectedValue).Y;
                        }
                        else if (toolStripVertice.Checked)
                        {
                            centro.X = ((Vertice2)cboVertices.SelectedValue).X;
                            centro.Y = ((Vertice2)cboVertices.SelectedValue).Y;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        float angMouseClick = Util2D.AnguloEntreDoisPontos(
                            new Vetor2(centro.Global.X, centro.Global.Y), pos2D);

                        objetoDragAngDiff.Z = angMouseClick;
                    }
                    else
                    {
                        objetoDragAng = null;
                    }
                }
                else if (toolStripEscala.Checked)
                {

                }
            }

            if (e.Button == MouseButtons.Middle)
            {
                _moveCamera = false;
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

        private Vetor2 PosAleatorio(Objeto2D obj)
        {
            int x = new Random(Environment.TickCount).Next(0, picScreen.ClientRectangle.Width);
            int y = new Random(Environment.TickCount + x).Next(0, picScreen.ClientRectangle.Height);

            _epico.Camera.Pos.X = x;
            _epico.Camera.Pos.Y = y;

            return new Vetor2(obj, x, y);
        }

        private void AtualizarComboObjetos2D()
        {
            cboObjeto2D.BeginUpdate();
            cboObjeto2D.DisplayMember = "Nome";
            cboObjeto2D.ValueMember = "o";
            cboObjeto2D.DataSource = _epico.objetos2D
                .Select(o => new
                {
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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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

            
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _mouseMove = e;

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
                _moveCamera = true;
            }
        }

        private void TxtCamPosX_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamPosX.Focused)
            {
                if (float.TryParse(txtCamPosX.Text, out float camPosX))
                {
                    _novaPosCamera.X = camPosX;
                }
            }
        }

        private void TxtCamPosY_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamPosY.Focused)
            {
                if (float.TryParse(txtCamPosY.Text, out float camPosY))
                {
                    _novaPosCamera.Y = camPosY;
                }
            }
        }


        private void TxtEscalaY_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtEscalaY.Focused)
            {
                if (float.TryParse(txtEscalaY.Text, out float escalaY))
                {
                    throw new NotImplementedException();
                }
            }
        }
        private void TxtEscalaX_ValueChanged(object sender, EventArgs e)
        {
            if (_obj_sel != null && txtEscalaX.Focused)
            {
                if (float.TryParse(txtEscalaX.Text, out float escalaX))
                {
                    throw new NotImplementedException();
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
                if (_epico.Camera.Objeto2DVisivelCamera(_obj_sel.First()))
                    txtVisivel.Text = "Sim";
                else
                    txtVisivel.Text = "Não";
            }

            if (!txtCamPosX.Focused) txtCamPosX.Value = (decimal)((Camera2D)cboCamera.SelectedValue).Pos.X;
            if (!txtCamPosY.Focused) txtCamPosY.Value = (decimal)((Camera2D)cboCamera.SelectedValue).Pos.Y;
            if (!txtCamPosZ.Focused) txtCamPosZ.Value = (decimal)((Camera2D)cboCamera.SelectedValue).PosZ;
            if (!txtCamAngulo.Focused) txtCamAngulo.Value = (decimal)((Camera2D)cboCamera.SelectedValue).Angulo.Z;
            if (!txtCamZoom.Focused) txtCamZoom.Value = (decimal)((Camera2D)cboCamera.SelectedValue).ZoomCamera;

            if (cboVertices.SelectedValue != null)
            {
                if (!txtOrigemPosX.Focused) txtOrigemPosX.Value = (decimal)((Origem2)cboOrigem.SelectedValue).X;
                if (!txtOrigemPosY.Focused) txtOrigemPosY.Value = (decimal)((Origem2)cboOrigem.SelectedValue).Y;
            }

#warning Falta Vetor

            if (cboVertices.SelectedValue != null)
            {
                if (!txtVerticePosX.Focused) txtVerticePosX.Value = (decimal)((Vertice2)cboVertices.SelectedValue).X;
                if (!txtVerticePosY.Focused) txtVerticePosY.Value = (decimal)((Vertice2)cboVertices.SelectedValue).Y;
                if (!txtVerticeAngulo.Focused) txtVerticeAngulo.Value = (decimal)((Vertice2)cboVertices.SelectedValue).Ang;
                if (!txtVerticeRaio.Focused) txtVerticeRaio.Value = (decimal)((Vertice2)cboVertices.SelectedValue).Raio;
            }
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
                var pos = _epico.Camera.PosFoco((Objeto2D)cboObjeto2D.SelectedValue);
                _novaPosCamera = new Vetor3(pos.X, pos.Y, _novaPosCamera.Z);
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
                cboOrigem.DataSource = _obj_sel.First().Origens.Select(
                (c, i) => new
                {
                    i = "Ponto " + i,
                    c
                }).ToList();
                cboOrigem.EndUpdate();
                #endregion

                if (chkAutoFocarObjeto.Checked)
                    BtnFocarObjeto_Click(sender, e);
            }
        }

        private void FPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fPSToolStripMenuItem.Checked = !fPSToolStripMenuItem.Checked;
            _epico.Debug = fPSToolStripMenuItem.Checked;
        }

        private void BtnNovaCamera_Click(object sender, EventArgs e)
        {
            #region Cria a Câmera 2D
            Camera2D camera_old = (Camera2D)cboCamera.SelectedValue;
            Camera2D nova_camera = _epico.CriarCamera(picScreen.Width, picScreen.Height);

            if (cboObjeto2D.SelectedValue != null)
            {
                nova_camera.Pos.X = camera_old.Pos.X;
                nova_camera.Pos.Y = camera_old.Pos.Y;
                _novaPosCamera.X = camera_old.Pos.X;
                _novaPosCamera.Y = camera_old.Pos.Y;
                _novoZoomCamera = 1F;
            }
            #endregion

            cboCamera.DataSource = _epico.Cameras
                .Select(cam => new
                {
                    cam.Nome,
                    cam
                }).ToList();

            cboCamera.SelectedValue = nova_camera;
        }

        private void CboCamera_SelectedValueChanged(object sender, EventArgs e)
        {
            Camera2D sel_camera = (Camera2D)cboCamera.SelectedValue;

            if (sel_camera != null)
            {
                _novaPosCamera.X = sel_camera.Pos.X;
                _novaPosCamera.Y = sel_camera.Pos.Y;
                _novoAnguloCamera.Z = sel_camera.Angulo.Z;
                _novoZoomCamera = sel_camera.ZoomCamera;
                _epico.Camera = sel_camera;
            }
        }

        private void BtnQuadrilatero_Click(object sender, EventArgs e)
        {
            Quadrilatero obj = new Quadrilatero();
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA(255, (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.GerarGeometria(rnd.Next(0, 359), _raio_padrao, (int)(_raio_padrao * 1.5F));
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

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
            _epico.AddObjeto2D(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void PicScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Camera2D cam = (Camera2D)cboCamera.SelectedValue;
            Objeto2D obj = (Objeto2D)cboObjeto2D.SelectedValue;

            float angulo_inicial = 0F;
            float angulo_final = 0F;
            Eixos2 ponto_inicial = Util2D.ObterPosEspaco2DMouseXY(cam, new Vetor2(_mouseDown.X, _mouseDown.Y));
            Eixos2 ponto_final = Util2D.ObterPosEspaco2DMouseXY(cam, new Vetor2(_mouseMove.X, _mouseMove.Y));

            bool transformandoObjetoComMouse = false;
            if (picScreen.Image != null)
            {
                if (_obj_sel.Count == 1) // 1 Objeto selecionado
                {
                    if (objetoDragMove != null && toolStripMove.Checked)
                    {
                        // Clicar com botão esquerdo e arrastar a posição do objeto selecionado
                        if (_mouseMove.Button == MouseButtons.Left)
                        {
                            transformandoObjetoComMouse = true;

                            if (_keyDown.Control) // Transladação na grade
                            {
                                ponto_final.X = Eixos.Grade(ponto_final.X, 10);
                                ponto_final.Y = Eixos.Grade(ponto_final.Y, 10);
                            }

                            obj.Pos.X = ponto_final.X - objetoDragMoveDiff.X;
                            obj.Pos.Y = ponto_final.Y - objetoDragMoveDiff.Y;
                        }
                    }
                    else if (objetoDragAng != null && toolStripAngulo.Checked)
                    {
                        // Clicar com botão esquerdo e arrastar o ângulo do objeto selecionado
                        if (_mouseMove.Button == MouseButtons.Left)
                        {
                            transformandoObjetoComMouse = true;

                            Vetor2 centro = new Vetor2(obj);
                            if (toolStripSelecao.Checked || toolStripRaio.Checked || toolStripEscala.Checked) {
                                centro.X = obj.Centro.X;
                                centro.Y = obj.Centro.Y;
                            }
                            else if (toolStripOrigem.Checked) {
                                centro.X = ((Origem2)cboOrigem.SelectedValue).X;
                                centro.Y = ((Origem2)cboOrigem.SelectedValue).Y;
                            }
                            else if (toolStripVertice.Checked) {
                                centro.X = ((Vertice2)cboVertices.SelectedValue).X;
                                centro.Y = ((Vertice2)cboVertices.SelectedValue).Y;
                            }

                            angulo_inicial = Util2D.AnguloEntreDoisPontos(
                                new Vetor2(centro.Global.X, centro.Global.Y), new Vetor2(ponto_inicial.X, ponto_inicial.Y));

                            angulo_final = Util2D.AnguloEntreDoisPontos(
                                new Vetor2(centro.Global.X, centro.Global.Y), new Vetor2(ponto_final.X, ponto_final.Y));

                            if (_keyDown.Control) // Angulação na grade
                            {
                                angulo_final = Eixos.Grade(angulo_final, 10);
                            }
                            obj.DefinirAngulo(centro, angulo_final - objetoDragAngDiff.Z + objetoDragAng.Z);
                        }
                    }
                    else if (toolStripRaio.Checked)
                    {

                    }
                }

                if (!transformandoObjetoComMouse)
                    if (selRect.Width > 0 && selRect.Height > 0) // Seleção no formato Retângulo
                    {
                        // Desenha o retângulo multi-seleção na tela
                        e.Graphics.FillRectangle(selBrush, selRect);

                        #region Retângulo de colisão no ambiente 2S
                        Vertice2[] rect = new Vertice2[4];
                        rect[0] = new Vertice2(selRect.X, selRect.Y);                                      // Superior Esquerdo
                        rect[1] = new Vertice2(selRect.X + selRect.Width, selRect.Y);                      // Superior Direito
                        rect[2] = new Vertice2(selRect.X + selRect.Width, selRect.Y + selRect.Height);     // Inferior Direito
                        rect[3] = new Vertice2(selRect.X, selRect.Y + selRect.Height);                     // Inferior Esquerdo
                        #endregion

                        if (toolStripSelecao.Checked)
                        {
                            _obj_sel.ForEach(x => x.Selecionado = false);
                            _obj_sel.Clear();
                            _obj_sel = _epico.ObterObjetos2DMouseXY(_epico.Camera, rect).ToList();
                            _obj_sel.ForEach(x => x.Selecionado = true);

                            // Informa a quantidade de objetos presentes na área do retângulo
                            e.Graphics.DrawString(
                                $"{_obj_sel.Count()} objetos", new Font("Lucida Console", 10),
                                new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                                new RectangleF(selRect.Location, selRect.Size),
                                new StringFormat(StringFormatFlags.NoWrap));
                        }
                        else if (toolStripOrigem.Checked)
                        {
                            // Informa a quantidade de objetos presentes na área do retângulo
                            _origem_sel.ForEach(x => x.Sel = false);
                            _origem_sel.Clear();
                            _origem_sel = Util2D.ObterOrigensObjeto2DPelaTela(_epico.Camera, _obj_sel, rect).ToList();
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
                            _vetor_sel = Util2D.ObterVetoresObjeto2DPelaTela(_epico.Camera, _obj_sel, selRect).ToList();
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
                            _vertice_sel = Util2D.ObterVerticesObjeto2DPelaTela(_epico.Camera, _obj_sel, rect).ToList();
                            _vertice_sel.ForEach(x => x.Sel = true);

                            e.Graphics.DrawString(
                                $"{_vertice_sel.Count()} vértices", new Font("Lucida Console", 10),
                                new SolidBrush(Color.FromArgb(selAlpha, 255, 255, 255)),
                                new RectangleF(selRect.Location, selRect.Size),
                                new StringFormat(StringFormatFlags.NoWrap));
                        }
                    }

                #region Representação da Orientação dos Eixos de Coordenadas
                if (_obj_sel.Count == 1) // 1 objeto selecionado
                {
                    if (toolStripMove.Checked)
                    {
                        Vetor2 ponto2D = new Vetor2();
                        if (toolStripSelecao.Checked) // Objeto
                        {
                            Vetor2 centro = ((Objeto2D)cboObjeto2D.SelectedValue).Centro;
                            ponto2D.X = centro.Global.X;
                            ponto2D.Y = centro.Global.Y;
                        }
                        else if (toolStripVertice.Checked) // Vértice
                        {
                            Vertice2 vertice = ((Vertice2)cboVertices.SelectedValue);
                            ponto2D.X = vertice.Global.X;
                            ponto2D.Y = vertice.Global.Y;
                        }
                        else if (toolStripOrigem.Checked) // Origem
                        {
                            Origem2 origem = ((Origem2)cboOrigem.SelectedValue);
                            ponto2D.X = origem.Global.X;
                            ponto2D.Y = origem.Global.Y;
                        }
                        DesenharLinhasOrientacaoEixosXYNaTela(e.Graphics, ponto2D, SentidoManipulaObjeto);
                    }
                    else if (toolStripAngulo.Checked)
                    {
                        Vetor2 ponto2D = new Vetor2();
                        if (toolStripSelecao.Checked) // Objeto
                        {
                            Vetor2 centro = ((Objeto2D)cboObjeto2D.SelectedValue).Centro;
                            ponto2D.X = centro.Global.X;
                            ponto2D.Y = centro.Global.Y;
                        }
                        else if (toolStripVertice.Checked) // Vértice
                        {
                            Vertice2 vertice = ((Vertice2)cboVertices.SelectedValue);
                            ponto2D.X = vertice.Global.X;
                            ponto2D.Y = vertice.Global.Y;
                        }
                        else if (toolStripOrigem.Checked) // Origem
                        {
                            Origem2 origem = ((Origem2)cboOrigem.SelectedValue);
                            ponto2D.X = origem.Global.X;
                            ponto2D.Y = origem.Global.Y;
                        }

                        DesenharAnguloEixosXYTela(
                            e.Graphics, ponto2D, SentidoManipulaObjeto,
                            angulo_inicial + -cam.Angulo.Z, angulo_final + -cam.Angulo.Z);

                        if (_mouseUp.Button == MouseButtons.Left)
                        {
                            //((Objeto2D)cboObjeto2D.SelectedValue).DefinirAngulo(ponto2D,
                            //    ((Objeto2D)cboObjeto2D.SelectedValue).Angulo.Z + -grausRotacao);

                            //obj.DefinirAngulo(ponto2D, (angulo_final - objetoDragAngDiff.Z + objetoDragAng.Z) - grausRotacao);
                        }
                    }
                    else if (toolStripEscala.Checked)
                    {
                        Vetor2 ponto2D = new Vetor2();
                        if (toolStripSelecao.Checked) // Objeto
                        {
                            Vetor2 centro = ((Objeto2D)cboObjeto2D.SelectedValue).Centro;
                            ponto2D.X = centro.Global.X;
                            ponto2D.Y = centro.Global.Y;
                        }
                        else if (toolStripVertice.Checked) // Vértice
                        {
                            Vertice2 vertice = ((Vertice2)cboVertices.SelectedValue);
                            ponto2D.X = vertice.Global.X;
                            ponto2D.Y = vertice.Global.Y;
                        }
                        else if (toolStripOrigem.Checked) // Origem
                        {
                            Origem2 origem = ((Origem2)cboOrigem.SelectedValue);
                            ponto2D.X = origem.Global.X;
                            ponto2D.Y = origem.Global.Y;
                        }
                        DesenharEscalaEixosXYTela(
                            e.Graphics, ponto2D, SentidoManipulaObjeto,
                            ponto_inicial, ponto_final);
                    }
                }
                #endregion
            }
            DesenharLinhasOrientacaoEixosXYNaTela(e.Graphics,
                new Vetor2(30, cam.ResHeight - 30), SentidoEixos.ESPACIAL, false, 30);

            if (_mouseUp.Button != MouseButtons.None)
                _mouseUp = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
        }

        private void DesenharEscalaEixosXYTela(
            Graphics g, Eixos2 centro, SentidoEixos sentido, Eixos2 pos_inicial, Eixos2 pos_final,
            bool espaco2D = true, float compLinhas = 40)
        {
            Camera2D cam = (Camera2D)cboCamera.SelectedValue;
            Objeto2D obj = (Objeto2D)cboObjeto2D.SelectedValue;

            PointF pontoA = new PointF();
            PointF pontoB = new PointF();
            PointF pontoC = new PointF();

            // Pontos da linha escalar XY
            PointF pontoD = new PointF();
            PointF pontoE = new PointF();

            // Triângulo Retângulo Escalar XY
            PointF pontoF = new PointF();
            PointF pontoG = new PointF();

            if (espaco2D)
            {
                pontoA = Util2D.ObterXYTelaPeloEspaco2D(cam, centro);
            }
            else
            {
                pontoA.X = centro.X;
                pontoA.Y = centro.Y;
            }

            switch (sentido)
            {
                case SentidoEixos.ESPACIAL:
                    _anguloZ_SentidoManipulaObjeto = -cam.Angulo.Z;
                    break;
                case SentidoEixos.OBJETO:
                    _anguloZ_SentidoManipulaObjeto = obj.Angulo.Z + -cam.Angulo.Z;
                    break;
                case SentidoEixos.CAMERA:
                    _anguloZ_SentidoManipulaObjeto = 0;
                    break;
                default:
                    throw new NotImplementedException(nameof(SentidoManipulaObjeto));
            }

            // Traça a linha Escalar do eixo X
            pontoB.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * compLinhas;
            pontoB.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * compLinhas;

            // Traça a linha Escalar do eixo Y
            pontoC.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * compLinhas;
            pontoC.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * compLinhas;

            // Traça a linha Escalar XY Externa
            pontoD.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (compLinhas / 100 * 70); // 70%
            pontoD.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (compLinhas / 100 * 70); // 70%

            pontoE.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (compLinhas / 100 * 70); 
            pontoE.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (compLinhas / 100 * 70); 

            // Triângulo Retângulo Escalar XY
            pontoF.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (compLinhas / 100 * 60); // 60%
            pontoF.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (compLinhas / 100 * 60); // 60%

            pontoG.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (compLinhas / 100 * 60);
            pontoG.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (compLinhas / 100 * 60);

            g.DrawLine(new Pen(new SolidBrush(Color.Red), 2), pontoA, pontoB);
            g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), pontoA, pontoC);
            g.DrawLine(new Pen(new SolidBrush(Color.Aquamarine), 1), pontoD, pontoE);
            g.FillPolygon(new SolidBrush(Color.FromArgb(70, Color.Aquamarine)), new PointF[] { pontoA, pontoF, pontoG }, System.Drawing.Drawing2D.FillMode.Alternate);

            PointF LetraCoordX = new PointF(
                pontoB.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * 8,
                pontoB.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * 8
                );

            PointF LetraCoordY = new PointF(
                pontoC.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * 8,
                pontoC.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * 8
                );

            Font fonte = new Font("Lucida Console", 10, FontStyle.Bold);
            SizeF sizeX = g.MeasureString("X", fonte);
            SizeF sizeY = g.MeasureString("Y", fonte);

            LetraCoordX.X -= sizeX.Width / 2;
            LetraCoordX.Y -= sizeX.Height / 2;

            LetraCoordY.X -= sizeY.Width / 2;
            LetraCoordY.Y -= sizeY.Height / 2;

            g.DrawString("X", fonte, new SolidBrush(Color.White), LetraCoordX);
            g.DrawString("Y", fonte, new SolidBrush(Color.White), LetraCoordY);
        }

        int rotacaoVoltas = 0;
        int direcaoRotacao = 0;
        int direcaoRotacaoAnterior = 0;
        int quadranteAnteriorRotacao = 0;
        float grausRotacao = 0;

        private void DesenharAnguloEixosXYTela(
            Graphics g, Eixos2 centro, SentidoEixos sentido, float angulo_inicial, float angulo_final, 
            bool espaco2D = true, float raio = 60)
        {
            float diametro = raio * 2;

            Camera2D cam = (Camera2D)cboCamera.SelectedValue;
            Objeto2D obj = (Objeto2D)cboObjeto2D.SelectedValue;

            RectangleF rect = new RectangleF();
            PointF posTela = new PointF();
            if (espaco2D)
            {
                rect.Width = diametro / 2;
                rect.Height = diametro / 2;
                posTela = Util2D.ObterXYTelaPeloEspaco2D(cam, centro);
                rect.X = posTela.X - rect.Width / 2;
                rect.Y = posTela.Y - rect.Height / 2;
            }
            else
            {
                throw new NotImplementedException("Não implementado!");
            }

            switch (sentido)
            {
                case SentidoEixos.ESPACIAL:
                    _anguloZ_SentidoManipulaObjeto = -cam.Angulo.Z;
                    break;
                case SentidoEixos.OBJETO:
                    _anguloZ_SentidoManipulaObjeto = obj.Angulo.Z + -cam.Angulo.Z;
                    break;
                case SentidoEixos.CAMERA:
                    _anguloZ_SentidoManipulaObjeto = 0;
                    break;
                default:
                    throw new NotImplementedException(nameof(SentidoManipulaObjeto));
            }

            Font fontM = new Font("Lucida Console", 8, FontStyle.Regular);

            if (angulo_inicial != angulo_final)
            {
                // Linha do Ângulo Inicial
                PointF pontoInicialA = new PointF();
                PointF pontoInicialB = new PointF();

                pontoInicialA.X = posTela.X;
                pontoInicialA.Y = posTela.Y;

                pontoInicialB.X = posTela.X + (float)Math.Cos(Util2D.Angulo2Radiano(angulo_inicial)) * (raio / 2);
                pontoInicialB.Y = posTela.Y + (float)Math.Sin(Util2D.Angulo2Radiano(angulo_inicial)) * (raio / 2);

                // Linha do Ângulo Final
                PointF pontoFinalA = new PointF();
                PointF pontoFinalB = new PointF();

                pontoFinalA.X = posTela.X;
                pontoFinalA.Y = posTela.Y;

                pontoFinalB.X = posTela.X + (float)(Math.Cos(Util2D.Angulo2Radiano(angulo_final)) * (raio / 2));
                pontoFinalB.Y = posTela.Y + (float)(Math.Sin(Util2D.Angulo2Radiano(angulo_final)) * (raio / 2));

                Rectangle rect2 = new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);

                // Corrige o ângulo final para desenhar o pedaço de pizza da elipse
                if (angulo_inicial < 0 && angulo_final < 0 &&
                    angulo_final > angulo_inicial) {
                }
                else if (angulo_final < 0)
                {
                    angulo_final = 180 + (180 - Math.Abs(angulo_final));
                }
                else if (angulo_final >= 0 && angulo_final < angulo_inicial)
                {
                    angulo_final = 360 + angulo_final;
                }

                float anguloRotacao = angulo_final - angulo_inicial;
                int quadrante = 1;

                if (anguloRotacao >= 180) {
                    direcaoRotacao = -1; // Esquerda

                    if (anguloRotacao > 225)
                        quadrante = 2;
                    else if (anguloRotacao >= 180 && anguloRotacao <= 225)
                        quadrante = 3;
                }
                else if (anguloRotacao >= 0) {
                    direcaoRotacao = 1; // Direita

                    if (anguloRotacao >= 0 && anguloRotacao <= 90)
                        quadrante = 1;
                    else if (anguloRotacao > 90 && anguloRotacao < 180)
                        quadrante = 4;
                }

                if (direcaoRotacao != direcaoRotacaoAnterior)
                {
                    if (direcaoRotacao == -1 && 
                        quadranteAnteriorRotacao == 1 && quadrante == 2)
                    {
                        rotacaoVoltas--;
                    }
                    else if (direcaoRotacao == 1 &&
                        quadranteAnteriorRotacao == 2 && quadrante == 1)
                    {
                        rotacaoVoltas++;
                    }
                }

                direcaoRotacaoAnterior = direcaoRotacao;
                quadranteAnteriorRotacao = quadrante;

                // Desenha o pedaço da pizza
                g.FillPie(new SolidBrush(Color.FromArgb(50, Color.White)), rect2,
                    angulo_inicial, angulo_final - angulo_inicial);

                if (rotacaoVoltas < 0)
                {
                    anguloRotacao = -(360 - anguloRotacao);
                    anguloRotacao = ((rotacaoVoltas + 1) * 360) + anguloRotacao;
                }
                else
                {
                    anguloRotacao = rotacaoVoltas * 360 + anguloRotacao;
                }

                grausRotacao = anguloRotacao;

                string txtM = $"[{anguloRotacao}º]";
                SizeF sizeM = g.MeasureString(txtM, fontM);

                PointF pontoM = new PointF();
                pontoM.X = posTela.X - sizeM.Width / 2;
                pontoM.Y = (posTela.Y - rect.Height / 2) - sizeM.Height * 2;

                //g.DrawLine(new Pen(new SolidBrush(Color.Black)), pontoInicialA, pontoInicialB);
                g.DrawLine(new Pen(new SolidBrush(Color.Red)), pontoFinalA, pontoFinalB);
                g.DrawString(txtM, fontM, new SolidBrush(Color.Aquamarine), pontoM);

            }

            PointF pontoA = new PointF();
            PointF pontoB = new PointF();
            PointF pontoC = new PointF();
            PointF pontoD = new PointF();

            // Traça a linha do eixo X
            pontoA.X = posTela.X - (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (raio / 2);
            pontoA.Y = posTela.Y - (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (raio / 2);
            pontoB.X = posTela.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (raio / 2);
            pontoB.Y = posTela.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * (raio / 2);
            g.DrawLine(new Pen(new SolidBrush(Color.Red), 1), pontoA, pontoB);

            // Traça a linha do eixo Y
            pontoC.X = posTela.X - (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (raio / 2);
            pontoC.Y = posTela.Y - (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (raio / 2);
            pontoD.X = posTela.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (raio / 2);
            pontoD.Y = posTela.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * (raio / 2);
            g.DrawLine(new Pen(new SolidBrush(Color.Green), 1), pontoC, pontoD);

            // Círculo do Ângulo
            g.DrawEllipse(new Pen(new SolidBrush(Color.Blue), 1), rect);

            // Centro do Ângulo
            g.FillEllipse(new SolidBrush(Color.Blue), posTela.X - 2, posTela.Y - 2, 4, 4);

            if (Math.Abs(rotacaoVoltas) > 0)
            {
                string strVoltas = $"{rotacaoVoltas}x";
                SizeF sVoltas = g.MeasureString(strVoltas, fontM);
                PointF pontoV = new PointF();
                pontoV.X = posTela.X - sVoltas.Width / 2;
                pontoV.Y = posTela.Y + sVoltas.Height;
                g.DrawString(strVoltas, fontM, new SolidBrush(Color.Aquamarine), pontoV);
            }
        }

        private void DesenharLinhasOrientacaoEixosXYNaTela(
            Graphics g, Eixos2 centro, SentidoEixos sentido, bool espaco2D = true, float compLinhas = 40)
        {
            Camera2D cam = (Camera2D)cboCamera.SelectedValue;
            Objeto2D obj = (Objeto2D)cboObjeto2D.SelectedValue;

            PointF pontoA = new PointF();
            PointF pontoB = new PointF();
            PointF pontoC = new PointF();

            if (espaco2D)
            {
                pontoA = Util2D.ObterXYTelaPeloEspaco2D(cam, centro);
            }
            else
            {
                pontoA.X = centro.X;
                pontoA.Y = centro.Y;
            }

            switch (sentido)
            {
                case SentidoEixos.ESPACIAL:
                    _anguloZ_SentidoManipulaObjeto = -cam.Angulo.Z;
                    break;
                case SentidoEixos.OBJETO:
                    _anguloZ_SentidoManipulaObjeto = obj.Angulo.Z + -cam.Angulo.Z;
                    break;
                case SentidoEixos.CAMERA:
                    _anguloZ_SentidoManipulaObjeto = 0;
                    break;
                default:
                    throw new NotImplementedException(nameof(SentidoManipulaObjeto));
            }

            // Traça a linha do eixo X
            pontoB.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * compLinhas;
            pontoB.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * compLinhas;

            // Traça a linha do eixo Y
            pontoC.X = pontoA.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * compLinhas;
            pontoC.Y = pontoA.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * compLinhas;

            g.DrawLine(new Pen(new SolidBrush(Color.Red), 2), pontoA, pontoB);
            g.DrawLine(new Pen(new SolidBrush(Color.Green), 2), pontoA, pontoC);

            
            PointF LetraCoordX = new PointF(
                pontoB.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * 8, 
                pontoB.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto)) * 8
                );

            PointF LetraCoordY = new PointF(
                pontoC.X + (float)Math.Cos(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * 8,
                pontoC.Y + (float)Math.Sin(Util2D.Angulo2Radiano(_anguloZ_SentidoManipulaObjeto - 90)) * 8
                );

            Font fonte = new Font("Lucida Console", 10, FontStyle.Bold);
            SizeF sizeX = g.MeasureString("X", fonte);
            SizeF sizeY = g.MeasureString("Y", fonte);

            LetraCoordX.X -= sizeX.Width / 2;
            LetraCoordX.Y -= sizeX.Height / 2;

            LetraCoordY.X -= sizeY.Width / 2;
            LetraCoordY.Y -= sizeY.Height / 2;

            g.DrawString("X", fonte, new SolidBrush(Color.White), LetraCoordX);
            g.DrawString("Y", fonte, new SolidBrush(Color.White), LetraCoordY);
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
                    _novoZoomCamera = camZoom;
                }
            }
        }

        private void DesligarZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            desligarZoomToolStripMenuItem.Checked = !desligarZoomToolStripMenuItem.Checked;
            _epico.Camera.DesligarSistemaZoom = desligarZoomToolStripMenuItem.Checked;
        }

        private void BtnDeformado_Click(object sender, EventArgs e)
        {
            Deformado obj = new Deformado();
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            obj.Mat_render.CorBorda = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
            obj.Mat_render.CorSolida = new RGBA((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));

            obj.GerarGeometria(0, 5, 50);
            _epico.AddObjeto2D(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void BtnLuzPonto_Click(object sender, EventArgs e)
        {
            LuzPonto obj = new LuzPonto(150, 150);
            obj.Pos = PosAleatorio(obj);
            var rnd = new Random(Environment.TickCount);
            _epico.AddObjeto2D(obj);

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
                _vertice_sel.Add((Vertice2)cboVertices.SelectedValue);
                if (toolStripVertice.Checked) _vertice_sel.First().Sel = true;
                AtualizarControlesVertice(_vertice_sel);

                if (chkAutoFocarVertice.Checked)
                    BtnFocarVertice_Click(sender, e);
            }
        }

        private void AtualizarControlesVertice(List<Vertice2> selecionados)
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
            _mouseUp = e;

            if (e.Button == MouseButtons.Left)
            {
                if (toolStripSelecao.Checked) // Ferramenta Seleção de Objetos
                {
                    if (selRect.Width == 0 && selRect.Height == 0) // Não formou o retângulo multi-seleção?
                    {
                        _vertice_sel.ForEach(x => x.Sel = false);
                        _vertice_sel.Clear();
                        _obj_sel.ForEach(x => x.Selecionado = false);
                        Objeto2D objSel = Util2D.ObterUnicoObjeto2DMouseXY(_epico, _epico.Camera, new Vetor2(selStart.X, selStart.Y));
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
                _moveCamera = false;
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
            
            propGrid.Refresh();
        }

        private void ToolStripSelecao_Click(object sender, EventArgs e)
        {
            ResetaFerramentasSelecao((ToolStripButton)sender);
            HabilitarFerramentasTransformacao();
            toolStripSelecao.Checked = true;
            LimparSelecoesGeometricas();
            tabControlObjeto.SelectedTab = tabObjeto;

            CboObjeto2D_SelectedIndexChanged(sender, new EventArgs()); // Exibe o ponto de origem
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

        private void ResetaFerramentasOrientacao(ToolStripButton exceto)
        {
            _ferramentasOrientacao.Except(new List<ToolStripButton>() { exceto })
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
                _origem_sel.Add((Origem2)cboOrigem.SelectedValue);
                if (toolStripOrigem.Checked) _origem_sel.First().Sel = true;
                AtualizarControlesPontoCentro(_origem_sel);

                if (chkAutoFocarOrigem.Checked)
                    BtnFocarOrigem_Click(sender, e);
            }
        }

        private void AtualizarControlesPontoCentro(List<Origem2> selecionados)
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
                var pos = ((Camera2D)cboCamera.SelectedValue).PosFoco((Origem2)cboOrigem.SelectedValue);
                _novaPosCamera = new Vetor3(pos.X, pos.Y, _novaPosCamera.Z);
            }
        }

        private void TxtVerticePosX_ValueChanged(object sender, EventArgs e)
        {
            if (_vertice_sel != null && txtVerticePosX.Focused)
            {
                if (float.TryParse(txtVerticePosX.Text, out float posX))
                {
                    _vertice_sel.First().X = posX;
                    _obj_sel.First().AtualizarRaio(_vertice_sel.First());
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
                    _vertice_sel.First().Y = posY;
                    _obj_sel.First().AtualizarRaio(_vertice_sel.First());
                    propGrid.Refresh();
                }
            }
        }

        private void TxtVerticeRaio_ValueChanged(object sender, EventArgs e)
        {
            if (_vertice_sel != null && txtVerticeRaio.Focused)
            {
                if (float.TryParse(txtVerticeRaio.Text, out float raio_v))
                {
                    _obj_sel.First().DefinirRaio(_vertice_sel.First(), raio_v);
                    propGrid.Refresh();
                }
            }
        }

        private void TxtVerticeAngulo_ValueChanged(object sender, EventArgs e)
        {
            if (_vertice_sel != null && txtVerticeAngulo.Focused)
            {
                if (float.TryParse(txtVerticeAngulo.Text, out float ang_v))
                {
                    _obj_sel.First().DefinirAngulo(_vertice_sel.First(), ang_v);
                    propGrid.Refresh();
                }
            }
        }

        private void TxtCamAngulo_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamAngulo.Focused)
            {
                if (float.TryParse(txtCamAngulo.Text, out float ang))
                {
                    _novoAnguloCamera.Z = ang;
                }
            }
        }

        private void BtnForm2D_Click(object sender, EventArgs e)
        {
            Form2D form = new Form2D();
            form.Pos = new Vetor2(form, 200, 200);
            _epico.AddObjeto2D(form);

            ((Camera2D)cboCamera.SelectedValue).Focar(form);

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

            Panel2D panel = new Panel2D(_epico, (Controle2D)cboObjeto2D.SelectedValue);
            panel.MouseDown += Panel_MouseDown;
            panel.Pos = new Vetor2(panel, 200, 200);
            _epico.AddObjeto2D(panel);

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

            Button2D obj = new Button2D(_epico, (Controle2D)cboObjeto2D.SelectedValue);
            obj.MouseDown += Panel_MouseDown;
            obj.Pos = new Vetor2(obj, 200, 200);
            _epico.AddObjeto2D(obj);

            AtualizarComboObjetos2D();
            cboObjeto2D.SelectedValue = obj;
        }

        private void MultiplicarQuadrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            multiplicarQuadrosToolStripMenuItem.Checked = !multiplicarQuadrosToolStripMenuItem.Checked;
            _epico.Camera.EfeitoMotionBlur = multiplicarQuadrosToolStripMenuItem.Checked;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void AndePeloEspaço2DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoopAndePeloEspaco(_epico, this);
        }

        private async void LoopAndePeloEspaco(EpicoGraphics engine, Form owner)
        {
            float AngPerson = 0;
            Triangulo person = new Triangulo(50);
            person.Nome = "Ator";
            person.Mat_render.CorSolida = new RGBA(255, 0, 255, 255);
            person.Pos.X = 100;
            person.Pos.Y = 100;
            engine.AddObjeto2D(person);
            person.CriarArestasConvexa();

            engine.Camera.Focar(person);

            int frente = 0, esquerda = 0;
            owner.KeyPreview = true;

            owner.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Up) frente = 5;
                if (e.KeyCode == Keys.Down) frente = -5;
                if (e.KeyCode == Keys.Left) esquerda = -5;
                if (e.KeyCode == Keys.Right) esquerda = 5;
            };
            owner.KeyUp += (sender, e) =>
            {
                if (e.KeyCode == Keys.Up) frente = 0;
                if (e.KeyCode == Keys.Down) frente = 0;
                if (e.KeyCode == Keys.Left) esquerda = 0;
                if (e.KeyCode == Keys.Right) esquerda = 0;
            };

            await Task.Factory.StartNew(() =>
            {
                Colisao2D colisao = new Colisao2D();
                double fatorTempo = 0.00000003;

                while (!_sair)
                {
                    AngPerson += esquerda * (float)(engine.Camera.TempoDelta * (fatorTempo / 1.5));

                    Vetor2 movimento = new Vetor2(person);
                    movimento.X += (float)(Math.Cos(Util2D.Angulo2Radiano(AngPerson + 90)) * frente * engine.Camera.TempoDelta * fatorTempo);
                    movimento.Y += (float)(Math.Sin(Util2D.Angulo2Radiano(AngPerson + 90)) * frente * engine.Camera.TempoDelta * fatorTempo);

                    // Detecção de colisão
                    for (int i = 0; i < engine.objetos2D.Count(); i++)
                    {
                        Objeto2D obj = engine.objetos2D[i];

                        // Ignora o próprio personagem
                        if (obj == person) continue;

                        // Testa a colisão com os polígonos
                        ColisaoPoligonoConvexoResultado r = colisao.PoligonoConvexo(
                            person, obj, movimento);

                        if (r.Interceptar)
                        {
                            person.Pos = movimento + r.TranslacaoMinimaVetor.Global;
                            break;
                        }
                    }

                    person.DefinirAngulo(AngPerson);
                    engine.Camera.Angulo.Z = AngPerson + 180;
                    person.Pos += movimento;
                    engine.Camera.Focar(person);
                }
            });
        }

        private void BtnFocarVertice_Click(object sender, EventArgs e)
        {
            if (cboVertices.SelectedValue != null)
            {
                var pos = _epico.Camera.PosFoco((Vertice2)cboVertices.SelectedValue);
                _novaPosCamera = new Vetor3(pos.X, pos.Y, _novaPosCamera.Z);
            }
        }

        private void TxtVerticeRaio_Enter(object sender, EventArgs e)
        {
            ToolStripRaio_Click(toolStripRaio, new EventArgs());
        }

        private void TxtVerticeAngulo_Enter(object sender, EventArgs e)
        {
            ToolStripAngulo_Click(toolStripAngulo, new EventArgs());
        }

        private void TxtVerticePosY_Enter(object sender, EventArgs e)
        {
            ToolStripMove_Click(toolStripMove, new EventArgs());
        }

        private void TxtVerticePosX_Enter(object sender, EventArgs e)
        {
            ToolStripMove_Click(toolStripMove, new EventArgs());
        }

        private void FrmEditor_KeyDown(object sender, KeyEventArgs e)
        {
            _keyDown = e;
            _keyUp = new KeyEventArgs(Keys.None);
        }

        private void FrmEditor_KeyUp(object sender, KeyEventArgs e)
        {
            _keyUp = e;
            _keyDown = new KeyEventArgs(Keys.None);
        }

        private void ToolStripOrientacaoEspacial_Click(object sender, EventArgs e)
        {
            ResetaFerramentasOrientacao((ToolStripButton)sender);
            toolStripOrientacaoEspacial.Checked = true;
            SentidoManipulaObjeto = SentidoEixos.ESPACIAL;
        }

        private void ToolStripOrientacaoObjeto_Click(object sender, EventArgs e)
        {
            ResetaFerramentasOrientacao((ToolStripButton)sender);
            toolStripOrientacaoObjeto.Checked = true;
            SentidoManipulaObjeto = SentidoEixos.OBJETO;
        }

        private void ToolStripOrientacaoCamera_Click(object sender, EventArgs e)
        {
            ResetaFerramentasOrientacao((ToolStripButton)sender);
            toolStripOrientacaoCamera.Checked = true;
            SentidoManipulaObjeto = SentidoEixos.CAMERA;
        }

        private void DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dToolStripMenuItem.Checked = !dToolStripMenuItem.Checked;
            _epico.Camera.Efeito3D = dToolStripMenuItem.Checked;
        }

        private void TxtCamPosZ_ValueChanged(object sender, EventArgs e)
        {
            if (txtCamPosZ.Focused)
            {
                if (float.TryParse(txtCamPosZ.Text, out float camPosZ))
                {
                    _novaPosCamera.Z = camPosZ;
                }
            }
        }
    }
}