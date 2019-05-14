//#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RadialMenu2
{
    public partial class frmMain : Form
    {
        Point cursorPosTela;
        Point cursorPosForm;
        float distanciaCursor;
        float angulo;

        public frmMain()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void FrmMain_Paint(object sender, PaintEventArgs e)
        {
            pnPedacoRadial.Refresh();
        }

        private void PnPedacoRadial_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = pnPedacoRadial.Bounds;
            rect.X = 0;
            rect.Y = 0;
            rect.Width = pnPedacoRadial.ClientRectangle.Width;
            rect.Height = pnPedacoRadial.ClientRectangle.Height;

            g.FillEllipse(new SolidBrush(Color.Blue), rect);
        }

        private void PnNucleo_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            string txt = "►";
            //txt = "Teste";

            Font font = new Font("Times New Roman", 100, FontStyle.Regular);
            SizeF size = g.MeasureString(txt, font);

            Point texto_centro = new Point((int)((pnNucleo.ClientSize.Width - size.Width) / 2),(int)((pnNucleo.ClientSize.Height - size.Height) / 2));
            Point centro_obj = new Point(pnNucleo.ClientSize.Width / 2, pnNucleo.ClientSize.Height / 2);
            float angulo_cursor = ObterAnguloEntreDoisPontos(cursorPosForm.X, cursorPosForm.Y, pnNucleo.Bounds.X + centro_obj.X, pnNucleo.Bounds.Y + centro_obj.Y);
            float distancia_cursor = CalcularDistanciaEntreDoisPontos(cursorPosForm.X, cursorPosForm.Y, pnNucleo.Bounds.X + centro_obj.X, pnNucleo.Bounds.Y + centro_obj.Y);

            float tamPonteiro = distancia_cursor;
            if (tamPonteiro > 110) tamPonteiro = 110; // Define o limite do ponto de luz

            float x2 = (float)Math.Cos((((Math.PI) / 180) * angulo_cursor) + Math.PI) * tamPonteiro;
            float y2 = (float)Math.Sin((((Math.PI) / 180) * angulo_cursor) + Math.PI) * tamPonteiro;

            Rectangle bounds = ((Panel)sender).ClientRectangle;
            using (GraphicsPath ellipsePath = new GraphicsPath())
            {
                ellipsePath.AddEllipse(bounds);
                using (PathGradientBrush brush = new PathGradientBrush(ellipsePath))
                {
                    Point pontoObjeto = pnNucleo.PointToClient(cursorPosTela);      // Posição da luz do ponto
                    brush.CenterPoint = new PointF(centro_obj.X + x2, centro_obj.Y + y2);   // Define a luz do ponto
                    ColorBlend cb = new ColorBlend(4);
                    cb.Colors = new Color[]
                       {  Color.DarkRed, Color.Firebrick, Color.IndianRed, Color.IndianRed };
                    // {  Color.DarkRed, Color.Firebrick, Color.IndianRed, Color.PeachPuff }; default
                    cb.Positions = new float[] { 0f, 0.1f, 0.6f, 1f };
                    // cb.Positions = new float[] { 0f, 0.3f, 0.6f, 1f }; default
                    brush.InterpolationColors = cb;
                    brush.FocusScales = new PointF(0, 0);

                    g.FillRectangle(brush, bounds);
                }
            }
            g.DrawString(txt, font, new SolidBrush(Color.White), texto_centro);

            if (chkDebug.Checked)
            {
                g.DrawLine(new Pen(new SolidBrush(Color.Blue), 1), centro_obj.X, centro_obj.Y, centro_obj.X + x2, centro_obj.Y + y2);
            }
        }

        private float ObterRaioDeUmRectangulo(Rectangle rect, float angulo)
        {
            return 0;
        }

        private float CalcularDistanciaEntreDoisPontos(float x1, float y1, float x2, float y2)
        {
            return (float)(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
        }

        private float ObterAnguloEntreDoisPontos(float x1, float y1, float x2, float y2)
        {
            return (float)(Math.Atan2(y2 - y1, x2 - x1) * 360 / (Math.PI * 2));
        }

        private void TmrMouse_Tick(object sender, EventArgs e)
        {
            cursorPosTela = Cursor.Position;
            lblMousePosTela.Text = $"MousePos Tela: {cursorPosTela.X},{cursorPosTela.Y}";

            cursorPosForm = this.PointToClient(cursorPosTela);
            lblMousePosForm.Text = $"MousePos Form: {cursorPosForm.X},{cursorPosForm.Y}";

            Point cursorPosObjeto = pnNucleo.PointToClient(cursorPosTela);
            lblMousePosObjeto.Text = $"MousePos Objeto: {cursorPosObjeto.X},{cursorPosObjeto.Y}";

            Point CentroInteriorObjeto = new Point(picTeste.ClientRectangle.Width / 2, picTeste.ClientRectangle.Height / 2);

            distanciaCursor = CalcularDistanciaEntreDoisPontos(
                cursorPosForm.X, cursorPosForm.Y,
                picTeste.Bounds.X + CentroInteriorObjeto.X, 
                picTeste.Bounds.Y + CentroInteriorObjeto.Y);

            angulo = ObterAnguloEntreDoisPontos(
                cursorPosForm.X, cursorPosForm.Y,
                picTeste.Bounds.X + CentroInteriorObjeto.X,
                picTeste.Bounds.Y + CentroInteriorObjeto.Y);

            lblDistancia.Text = $"Distância: {distanciaCursor}";
            lblAngulo.Text = $"Ângulo: {angulo}";

            pnNucleo.Refresh();
            picTeste.Refresh();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            SetDoubleBuffered(pnNucleo);
            //SetDoubleBuffered(pnPedacoRadial);

            UpdateStyles();
        }

        private void PicTeste_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float tamPonteiro = 30;

            // Ou pode usar a variável 'distancia do cursor' para igualar ao cursor do mouse
            // Obs.: Esta variável 'distancia' pode ser usada para detectar colisão esférica
            // tamPonteiro = distanciaCursor;

            float x2 = (float)Math.Cos((((Math.PI) / 180) * angulo) + Math.PI) * tamPonteiro;
            float y2 = (float)Math.Sin((((Math.PI) / 180) * angulo) + Math.PI) * tamPonteiro;

            Point centro = new Point(picTeste.ClientRectangle.Width / 2, picTeste.ClientRectangle.Height / 2);
            g.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), centro.X, centro.Y, centro.X + x2, centro.Y + y2);
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }
    }
}