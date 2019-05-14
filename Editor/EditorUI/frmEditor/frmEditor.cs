using Eto.Forms;
using Eto.Drawing;
using Epico;

namespace Editor.win32
{
    public partial class frmEditor : Form
    {
        Epico2D _epico2D = new Epico2D();
        bool _sair = false;

        PointF cameraDrag;
        bool moveCamera = false;

        public frmEditor()
        {
            InitializeComponent();
        }

        private void FrmEditor_Load(object sender, System.EventArgs e)
        {

        }

        private void FrmEditor_LoadComplete(object sender, System.EventArgs e)
        {
            #region Cria a Câmera
            _epico2D.CriarCamera(imageView.Size.Width, imageView.Size.Height);
            #endregion

            #region Define os atributos dos controles

            //HabilitarFerramentasTransformacao(false);

            //DefineMaxMinValues(
            //    txtCamPosX, txtCamPosY, txtCamAngulo, txtCamZoom,
            //    txtPosX, txtPosY, txtRaio, txtAngulo, txtEscalaX, txtEscalaY,
            //    txtOrigemPosX, txtOrigemPosY,
            //    txtVerticePosX, txtVerticePosY, txtVerticeRaio, txtVerticeAngulo);

            //BtnCirculo_Click(sender, e);
            //AtualizarControlesObjeto2D(_objs_sel);
            //AtualizarComboObjetos2D();

            //debugToolStripMenuItem.Checked = _engine2D.Debug = true;
            //desligarZoomToolStripMenuItem.Checked = _engine2D.Camera.DesligarSistemaZoom = true;

            //cboCamera.DisplayMember = "Nome";
            //cboCamera.ValueMember = "Cam";
            //cboCamera.DataSource = _engine2D.Cameras.Select(
            //    Cam => new
            //    {
            //        Cam.Id,
            //        Cam.Nome,
            //        Cam
            //    }).ToList();
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
#warning MousePos
                    //_epico2D.Camera.Pos.X += -(float)((cameraDrag.X - Cursor.Position.X) * _epico2D.Camera.TempoDelta * 0.000001);
                    //_epico2D.Camera.Pos.Y += -(float)((cameraDrag.Y - Cursor.Position.Y) * _epico2D.Camera.TempoDelta * 0.000001);
                }

                if (_epico2D.Camera.ResWidth != imageView.Size.Width ||
                    _epico2D.Camera.ResHeigth != imageView.Size.Height)
                {
                    _epico2D.Camera.RedefinirResolucao(imageView.Size.Width, imageView.Size.Height);
                }

                imageView.Image = _epico2D.Camera.Renderizar();
            }
            #endregion
        }
    }
}
