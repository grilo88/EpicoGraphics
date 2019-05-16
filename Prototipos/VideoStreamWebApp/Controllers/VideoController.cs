using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epico;
using Epico.Objetos2D.Avancados;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideoStreamWebApp.Controllers
{
    public class VideoController : Controller
    {
        private bool frameAvailable = false;
        private Bitmap frame = null;
        private string BOUNDARY = "frame";
        Epico2D epico;

        /// <summary>
        /// Initializer for the MJPEGstream
        /// </summary>
        VideoController()
        {
            epico = new Epico2D();
            Estrela obj = new Estrela();
            obj.Mat_render.CorBorda = new Epico.Sistema.RGBA(255, 0, 0, 0);
            obj.Mat_render.CorSolida = new Epico.Sistema.RGBA(255, 0, 150, 200);
            obj.GerarGeometria(0, 0, 50, 10);
            epico.AddObjeto(obj);
            epico.CriarCamera(640, 480);
            epico.Camera.Focar(obj);

            //mjpegStream.Source = @"{{INSERT STREAM URL}}";
            //mjpegStream.NewFrame += new NewFrameEventHandler(showFrameEvent);
        }

        [HttpGet]
        public HttpResponseMessage GetVideoContent()
        {
            //mjpegStream.Start();

            var response = Request.CreateResponse();
            response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)StartStream);
            response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/x-mixed-replace; boundary=" + BOUNDARY);
            return response;
        }

        /// <summary>
        /// Craete an appropriate header.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] CreateHeader(int length)
        {
            string header =
                "--" + BOUNDARY + "\r\n" +
                "Content-Type:image/jpeg\r\n" +
                "Content-Length:" + length + "\r\n\r\n";

            return Encoding.ASCII.GetBytes(header);
        }

        public byte[] CreateFooter()
        {
            return Encoding.ASCII.GetBytes("\r\n");
        }

        /// <summary>
        /// Write the given frame to the stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="frame">Bitmap format frame</param>
        private void WriteFrame(Stream stream, Bitmap frame)
        {
            // prepare image data
            byte[] imageData = null;

            // this is to make sure memory stream is disposed after using
            using (MemoryStream ms = new MemoryStream())
            {
                frame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageData = ms.ToArray();
            }

            // prepare header
            byte[] header = CreateHeader(imageData.Length);
            // prepare footer
            byte[] footer = CreateFooter();

            // Start writing data
            stream.Write(header, 0, header.Length);
            stream.Write(imageData, 0, imageData.Length);
            stream.Write(footer, 0, footer.Length);
        }

        /// <summary>
        /// While the MJPEGStream is running and clients are connected,
        /// continue sending frames.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="httpContent">The content information</param>
        /// <param name="transportContext"></param>
        private void StartStream(Stream stream, HttpContent httpContent, TransportContext transportContext)
        {
            while (true /*motor rodando && cliente conectado?*/)
            {
                if (frameAvailable)
                {
                    try
                    {
                        WriteFrame(stream, epico.Camera.Renderizar());
                        frameAvailable = false;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }
                else
                {
                    Thread.Sleep(30);
                }
            }
        }
    }
}