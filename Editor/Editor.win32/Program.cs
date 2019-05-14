using Eto.Forms;
using Eto.Drawing;
using System;

namespace Editor.win32
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Application().Run(new frmEditor());
        }
    }
}
