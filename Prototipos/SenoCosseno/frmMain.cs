using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SenoCosseno
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        float rad;
        int distancia = 10;
        private void Timer1_Tick(object sender, EventArgs e)
        {
            rad += 0.1F;

            float posX = (float)Math.Sin(rad) * distancia;
            float posY = (float)Math.Cos(rad) * distancia;

            int newPosX = 50 + (int)posX; // Define a posição inicial X + Seno
            int newPosY = 50 + (int)posY; // Define a posição inicial Y + Cosseno

            // Seno
            button1.Location = new Point(newPosX, button1.Location.Y);

            // Cosseno
            button2.Location = new Point(button2.Location.X, newPosY);

            // Seno + Cosseno
            button3.Location = new Point(newPosX, newPosY);
        }
    }
}
