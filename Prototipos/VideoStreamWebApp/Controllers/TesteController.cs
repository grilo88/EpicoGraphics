using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideoStreamWebApp.Controllers
{
    /*
     <html>
<body>
    <video width="480" height="320" controls="controls" autoplay="autoplay">
        <source src="http://localhost:9000/api/camera/fromvideo/?videoName=Christmas" type="video/mp4">
    </video>
</body>
</html>
         */
    [Route("api/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public HttpResponseMessage FromImages()
        {
            var imageStream = new ImageStream();
            Func<Stream, HttpContent, TransportContext, Task> func = imageStream.WriteToStream;

            HttpRequestMessage request = new HttpRequestMessage();
            var response = request.CreateResponse();
            response.Content = new PushStreamContent(func);
            response.Content.Headers.Remove("Content-Type");
            response.Content.Headers.TryAddWithoutValidation("Content-Type", "multipart/x-mixed-replace;boundary=" + imageStream.Boundary);
            return response;
        }

        // GET: api/Teste/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Teste
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Teste/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
