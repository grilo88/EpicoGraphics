using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VideoStreamWebApp
{
    public class MultipartContent
    {
        public string ContentType { get; set; }

        public Stream Stream { get; set; }
    }

    public class MultipartResult : MultipartContent, IActionResult
    {
        private readonly System.Net.Http.MultipartContent content;

        public MultipartResult(string subtype = "x-mixed-replace", string boundary = "canonliveview")
        {
            if (boundary == null)
            {
                this.content = new System.Net.Http.MultipartContent(subtype);
            }
            else
            {
                this.content = new System.Net.Http.MultipartContent(subtype, boundary);
            }
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            if (Stream != null)
            {
                var content = new StreamContent(Stream);

                if (ContentType != null)
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
                }

                this.content.Add(content);
            }

            context.HttpContext.Response.ContentLength = content.Headers.ContentLength;
            context.HttpContext.Response.ContentType = content.Headers.ContentType.ToString();

            await content.CopyToAsync(context.HttpContext.Response.Body);
        }
    }
}
