using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace WebApplication2.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UploadController(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(
            IFormFile file)
        {
            var cloudStorageAccount =
                CloudStorageAccount.Parse(
                    _configuration["AzureStorage:ConnectionString"]);

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    _configuration["AzureStorage:FilePath"]);

            var blobName =
                $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            blobName = blobName.Replace("\"", "");

            var cloudBlockBlob =
                cloudBlobContainer.GetBlockBlobReference(blobName);

            cloudBlockBlob.Properties.ContentType =
               file.ContentType;

            using (var fileStream = file.OpenReadStream())
            {
                await cloudBlockBlob.UploadFromStreamAsync(fileStream);
            }

            return new OkObjectResult(new { name = blobName });
        }
    }
}