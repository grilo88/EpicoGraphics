using Epico.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if Editor2D || NetStandard2 || NetCore
using System.Drawing;
using System.Drawing.Imaging;
#elif EtoForms
using Eto.Drawing;
#endif

namespace Epico
{
    public class Epico2D
    {
        #region Propriedades
        public long TempoDelta => Camera.TempoDelta;
        public Camera2D Camera { get; set; }
        public List<Camera2D> Cameras { get; set; } = new List<Camera2D>();

        public bool Debug { get; set; }
        #endregion

        public List<Objeto2D> objetos { get; set; } = new List<Objeto2D>();
        public int MaximoFPS { get; internal set; }

        public void AddObjeto(Objeto2D objeto2d)
        {
            Objeto2D ambiguo = objetos.Where(x => x.Nome.StartsWith(objeto2d.Nome)).LastOrDefault();

            int number = 0;
            if (ambiguo != null)
            {
                int length = objeto2d.Nome.Length;
                int.TryParse(ambiguo.Nome.Substring(length), out number);
            }

            objeto2d.Nome += (++number).ToString("D2");
            objetos.Add(objeto2d);
        }

        public Camera2D CriarCamera(int width, int heigth)
        {
            return CriarCamera(width, heigth,
#if Editor2D || NetStandard2 || NetCore
                 PixelFormat.Format32bppArgb
#elif EtoForms
                PixelFormat.Format32bppRgba
#endif
                );
        }
        public Camera2D CriarCamera(int width, int heigth,

#if NetStandard2
            System.Drawing.Imaging.PixelFormat pixelFormat
#else
            PixelFormat pixelFormat
#endif
            )
        {
            Camera2D camera = new Camera2D(this, width, heigth, pixelFormat);
            Camera2D ambiguo = Cameras.Where(x => x.Nome.StartsWith(camera.Nome)).LastOrDefault();

            int number = 0;
            if (ambiguo != null)
            {
                int length = camera.Nome.Length;
                int.TryParse(ambiguo.Nome.Substring(length), out number);
            }

            camera.Nome += (++number).ToString("D2");
            Cameras.Add(camera);

            // Define a primeira camera adicionada como camera padrão
            if (Cameras.Count == 1) Camera = camera;

            return camera;
        }

        public void AtualizarFisica(float deltaTime)
        {
            int tick = Environment.TickCount;

            // TODO: Física

            int tempoGasto = Environment.TickCount - tick;
        }
    }
}
