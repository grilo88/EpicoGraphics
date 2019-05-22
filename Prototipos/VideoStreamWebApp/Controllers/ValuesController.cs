using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EpicoGraphics;
using EpicoGraphics.Objetos2D.Avancados;
using Microsoft.AspNetCore.Mvc;

namespace VideoStreamWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            EpicoGraphics.EpicoGraphics epico = new EpicoGraphics.EpicoGraphics();
            Estrela obj = new Estrela();
            
            obj.Mat_render.CorBorda = new EpicoGraphics.Sistema2D.RGBA(255, 0, 0, 0);
            obj.Mat_render.CorSolida = new EpicoGraphics.Sistema2D.RGBA(255, 0, 150, 200);
            epico.AddObjeto2D(obj);
            epico.CriarCamera(640, 480);
            epico.Camera.Focar(obj);

            while (true)
            {
                using (Stream stream = new MemoryStream())
                {
                    Bitmap bmp = epico.Camera.Renderizar();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    MultipartResult multipartResult = new MultipartResult
                    {
                        ContentType = "image/jpeg",
                        Stream = stream
                    };
                    return multipartResult;
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
