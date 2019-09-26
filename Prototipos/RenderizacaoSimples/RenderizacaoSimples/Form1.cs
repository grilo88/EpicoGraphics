using Epico;
using Epico.Objetos2D.Avancados;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenderizacaoSimples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            EpicoGraphics epico = new EpicoGraphics();
            Estrela obj = new Estrela();

            obj.Mat_render.CorBorda = new Epico.Sistema2D.RGBA(255, 0, 0, 0);
            obj.Mat_render.CorSolida = new Epico.Sistema2D.RGBA(255, 0, 150, 200);
            epico.AddObjeto2D(obj);
            epico.CriarCamera(640, 480);
            epico.Camera.Focar(obj);

            this.BackgroundImage = epico.Camera.Renderizar();
        }
    }
}
