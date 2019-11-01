using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class UploadHttpTrigger2
    {
        [FunctionName("UploadHttpTrigger2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v2/files")] HttpRequestMessage req,
            ILogger log,
            [Blob("%AzureStorage:FilePath%", FileAccess.Write, Connection = "AzureStorage:ConnectionString")] CloudBlobContainer cloudBlobContainer)
        {
            log.LogInformation(
                $"{nameof(UploadHttpTrigger2)} trigger function processed a request.");

            var multipartMemoryStreamProvider =
                new MultipartMemoryStreamProvider();

            await req.Content.ReadAsMultipartAsync(multipartMemoryStreamProvider);

            var file =
                multipartMemoryStreamProvider.Contents.First();
            var fileInfo =
                file.Headers.ContentDisposition;

            log.LogInformation(
                JsonConvert.SerializeObject(fileInfo, Formatting.Indented));

            var blobName =
                $"{Guid.NewGuid()}{Path.GetExtension(fileInfo.FileName)}";

            blobName = blobName.Replace("\"", "");

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            cloudBlockBlob.Properties.ContentType =
               file.Headers.ContentType.MediaType;

            using (var fileStream = await file.ReadAsStreamAsync())
            {
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
            }

            return new OkObjectResult(new { name = blobName });
        }
    }
}
