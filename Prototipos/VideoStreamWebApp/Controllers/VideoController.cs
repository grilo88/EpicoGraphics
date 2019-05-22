using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EpicoGraphics;
using EpicoGraphics.Objetos2D.Avancados;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideoStreamWebApp.Controllers
{
    public class VideoController : Controller
    {
        private bool quadroDisponivel = false;
        private Bitmap frame = null;
        private string BOUNDARY = "frame";
        EpicoGraphics.EpicoGraphics epico;

        /// <summary>
        /// Inicializa o motor de renderização
        /// </summary>
        public VideoController()
        {
            epico = new EpicoGraphics.EpicoGraphics();
            Estrela obj = new Estrela();
            obj.Mat_render.CorBorda = new EpicoGraphics.Sistema2D.RGBA(255, 0, 0, 0);
            obj.Mat_render.CorSolida = new EpicoGraphics.Sistema2D.RGBA(255, 0, 150, 200);
            epico.AddObjeto2D(obj);
            epico.CriarCamera(640, 480);
            epico.Camera.Focar(obj);
        }

        [HttpGet]
        public HttpResponseMessage GetVideoContent()
        {
//#error Obter o request do contexto atual.
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)IniciarFluxo);
            response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/x-mixed-replace; boundary=" + BOUNDARY);
            return response;
        }

        /// <summary>
        /// Cria um cabeçalho apropriado
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        private byte[] CriarCabecalho(int comp)
        {
            string header =
                "--" + BOUNDARY + "\r\n" +
                "Content-Type:image/jpeg\r\n" +
                "Content-Length:" + comp + "\r\n\r\n";

            return Encoding.ASCII.GetBytes(header);
        }

        public byte[] CriarRodape()
        {
            return Encoding.ASCII.GetBytes("\r\n");
        }

        /// <summary>
        /// Escreva o quadro no fluxo
        /// </summary>
        /// <param name="fluxo">Fluxo</param>
        /// <param name="quadro">Formato de quadro bitmap</param>
        private void EscreveQuadro(Stream fluxo, Bitmap quadro)
        {
            // Prepara a imagem
            quadro = epico.Camera.Renderizar();
            byte[] dadosImagem = null;

            // isso é para garantir que o fluxo de memória seja descartado após o uso
            using (MemoryStream ms = new MemoryStream())
            {
                quadro.Save(ms, ImageFormat.Jpeg);
                dadosImagem = ms.ToArray();
            }

            // Prepara o cabeçalho
            byte[] cab = CriarCabecalho(dadosImagem.Length);
            // Prepara o rodapé
            byte[] rodp = CriarRodape();

            // Inicia escrita de dados
            fluxo.Write(cab, 0, cab.Length);
            fluxo.Write(dadosImagem, 0, dadosImagem.Length);
            fluxo.Write(rodp, 0, rodp.Length);
        }

        /// <summary>
        /// Enquanto o Motor de Renderização estiver em execução e os clientes estiverem conectados, continue enviando quadros.
        /// </summary>
        /// <param name="fluxo">Fluxo a escrever</param>
        /// <param name="httpContent">O conteúdo da informação</param>
        /// <param name="transportContext"></param>
        private void IniciarFluxo(Stream fluxo, HttpContent httpContent, TransportContext transportContext)
        {
            // TODO: Implementar /*motor rodando && cliente conectado?*/

            //bool ClienteConectado = HttpContext.Current.Response.IsClientConnected;

            while (true /*motor rodando && cliente conectado?*/)
            {
                if (quadroDisponivel)
                {
                    try
                    {
                        EscreveQuadro(fluxo, epico.Camera.Renderizar());
                        quadroDisponivel = false;
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
        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }
        }
    }
}