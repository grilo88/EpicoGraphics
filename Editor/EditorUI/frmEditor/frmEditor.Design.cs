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

            layout.Size = new Size(100, 100);

            lbl.Text = "Hello World!";
            lbl2.Text = "Seja bem vindo!";

            layout.Add(lbl);
            layout.Add(lbl2);
            layout.Add(btn);
            layout.Add(imageView);
            Content = layout;

            Load += FrmEditor_Load;
            LoadComplete += FrmEditor_LoadComplete;
        }

        

        DynamicLayout layout = new DynamicLayout();
        Label lbl = new Label();
        Label lbl2 = new Label();
        Button btn = new Button();
        ImageView imageView = new ImageView();
    }
}
