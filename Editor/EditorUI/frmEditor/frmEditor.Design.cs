using Eto.Forms;
using Eto.Drawing;

namespace Editor.win32
{
    public partial class frmEditor : Form
    {
        public void InitializeComponent()
        {
            Title = "Editor Épico 1.0";
            ClientSize = new Size(200, 200);
            Content = imageView;

            LoadComplete += FrmEditor_LoadComplete;
        }

        ImageView imageView = new ImageView();
    }
}
