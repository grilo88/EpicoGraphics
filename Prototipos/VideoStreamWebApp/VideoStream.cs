using Epico;
using Epico.Objetos2D.Avancados;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamWebApp
{
    internal class ImageStream
    {
        /*
         <html>
<head>
    <script>#REPLACE_WITH_JS#</script>
</head>
<body ng-app="CameraApp">
    <div ng-controller="MainController">
        <div>
            <img src="http://localhost:9000/api/camera/fromimages" width="480" height="320" alt="">
        </div>
        <div>
            <video id="videoPlayer" width="480" height="320" controls="controls">
                <source data-ng-src="{{selectedVideoUrl}}" type="video/mp4">
                <!--  <source src="http://localhost:9000/api/camera/fromvideo/?videoName=Christmas" type="video/mp4">-->
            </video>
        </div>
        <div>
            <select ng-model="selectedVideo">
                <option ng-repeat="video in videos" value="{{video}}">{{video}}</option>
            </select>
        </div>
        <div>
            <button ng-click="start()">Start</button>
            <button ng-click="stop()">Stop</button>
        </div>
    </div>
</body>
</html>
             */
        Epico2D epico;

        public ImageStream()
        {
            epico = new Epico2D();
            Estrela obj = new Estrela();
            obj.Mat_render.CorBorda = new Epico.Sistema.RGBA(255, 0, 0, 0);
            obj.Mat_render.CorSolida = new Epico.Sistema.RGBA(255, 0, 150, 200);
            obj.GerarGeometria(0, 0, 50, 10);
            epico.AddObjeto(obj);
            epico.CriarCamera(640, 480);
            epico.Camera.Focar(obj);
        }

        public object Boundary { get; private set; } = "HintDesk";

        public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            //byte[] newLine = Encoding.UTF8.GetBytes("\r\n");

            //var header = $"--{Boundary}\r\nContent-Type: image/jpeg\r\nContent-Length: {fileInfo.Length}\r\n\r\n";
            //var headerData = Encoding.UTF8.GetBytes(header);

            //Bitmap bmp = epico.Camera.Renderizar();

            //foreach (var file in Directory.GetFiles(@"TestData\Images", "*.jpg"))
            //{
            //    var fileInfo = new FileInfo(file);
            //    var header = $"--{Boundary}\r\nContent-Type: image/jpeg\r\nContent-Length: {fileInfo.Length}\r\n\r\n";
            //    var headerData = Encoding.UTF8.GetBytes(header);
            //    await outputStream.WriteAsync(headerData, 0, headerData.Length);
            //    await fileInfo.OpenRead().CopyToAsync(outputStream);
            //    await outputStream.WriteAsync(newLine, 0, newLine.Length);
            //    await Task.Delay(1000 / 30);
            //}
        }

        /*
         public async Task WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            byte[] newLine = Encoding.UTF8.GetBytes("\r\n");

            foreach (var file in Directory.GetFiles(@"TestData\Images", "*.jpg"))
            {
                var fileInfo = new FileInfo(file);
                var header = $"--{Boundary}\r\nContent-Type: image/jpeg\r\nContent-Length: {fileInfo.Length}\r\n\r\n";
                var headerData = Encoding.UTF8.GetBytes(header);
                await outputStream.WriteAsync(headerData, 0, headerData.Length);
                await fileInfo.OpenRead().CopyToAsync(outputStream);
                await outputStream.WriteAsync(newLine, 0, newLine.Length);
                await Task.Delay(1000 / 30);
            }
        }
         */
    }
}
