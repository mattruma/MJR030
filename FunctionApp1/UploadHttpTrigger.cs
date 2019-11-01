using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class UploadHttpTrigger
    {
        [FunctionName("UploadHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "files")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation(
                $"{nameof(UploadHttpTrigger)} trigger function processed a request.");

            var multipartMemoryStreamProvider =
                new MultipartMemoryStreamProvider();

            await req.Content.ReadAsMultipartAsync(multipartMemoryStreamProvider);

            var file =
                multipartMemoryStreamProvider.Contents.First();
            var fileInfo =
                file.Headers.ContentDisposition;

            log.LogInformation(
                JsonConvert.SerializeObject(fileInfo, Formatting.Indented));

            var cloudStorageAccount =
                CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureStorage:ConnectionString"));

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AzureStorage:FilePath"));

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
