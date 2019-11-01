using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace FunctionApp1
{
    public static class DownloadHttpTrigger
    {
        [FunctionName("DownloadHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files/{name}")] HttpRequest req,
            string name,
            ILogger log)
        {
            log.LogInformation(
                $"{nameof(DownloadHttpTrigger)} trigger function processed a request.");

            var cloudStorageAccount =
                CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureStorage:ConnectionString"));

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    Environment.GetEnvironmentVariable("AzureStorage:FilePath"));

            await cloudBlobContainer.CreateIfNotExistsAsync();

            var blobName =
                name;

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            var ms = new MemoryStream();

            await cloudBlockBlob.DownloadToStreamAsync(ms);

            return new FileContentResult(ms.ToArray(), cloudBlockBlob.Properties.ContentType);
        }
    }
}
