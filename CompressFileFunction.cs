using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CompressFileFunction
{
    public static class CompressFileFunction
    {
        [FunctionName("CompressFile")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request,
            ILogger log)
        {
            var fileName = request.Query.ContainsKey("fileName") 
                ? request.Query["fileName"].ToString()
                : string.Empty;

            log.LogInformation("Compression request is received. Output file name: {0}", fileName);

            var zippedStream = await CompressFile.ZipStream(request.Body, fileName);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(zippedStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Zip);

            return response;
        }
    }
}
