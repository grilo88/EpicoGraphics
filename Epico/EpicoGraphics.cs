using Epico.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epico.Sistema;

#if Editor2D || NetStandard2 || NetCore
using System.Drawing;
using System.Drawing.Imaging;
#elif EtoForms
using Eto.Drawing;
#endif

namespace Epico
{
    public class EpicoGraphics
    {
        #region Propriedades
        public long TempoDelta => Camera.TempoDelta;
        public Camera2D Camera { get; set; }
        public List<Camera2D> Cameras { get; set; } = new List<Camera2D>();

        public bool Debug { get; set; }
        #endregion

        public List<Objeto2D> objetos2D { get; private set; } = new List<Objeto2D>();
        public int MaximoFPS { get; internal set; }

        public T CriarObjeto2D<T>() where T : Objeto2D
        {
            object objeto2D = Activator.CreateInstance(typeof(T));
            ((Objeto2D)objeto2D).AssociarEngine(this);
            AddObjeto2D((Objeto2D)objeto2D);
            return (T)objeto2D;
        }

        public void AddObjeto2D(Objeto2D obj2D)
        {
            Objeto2D ambiguo = objetos2D.OfType<Objeto2D> ()
                .Where(x => x.Nome.StartsWith(obj2D.Nome)).LastOrDefault();

            int number = 0;
            if (ambiguo != null)
            {
                int length = obj2D.Nome.Length;
                int.TryParse(ambiguo.Nome.Substring(length), out number);
            }

            obj2D.Nome += (++number).ToString("D2");
            objetos2D.Add(obj2D);
            obj2D.AssociarEngine(this);
        }

        public Camera2D CriarCamera(int width, int heigth)
        {
            return CriarCamera2D(width, heigth,
#if Editor2D || NetStandard2 || NetCore
                 PixelFormat.Format32bppArgb
#elif EtoForms
                PixelFormat.Format32bppRgba
#endif
                );
        }
        public Camera2D CriarCamera2D(int width, int heigth,

#if NetStandard2
            System.Drawing.Imaging.PixelFormat pixelFormat
#else
            PixelFormat pixelFormat
#endif
            )
        {
            Camera2D camera = new Camera2D(this, width, heigth, pixelFormat);
            camera.AssociarEngine(this);
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
    }
}
