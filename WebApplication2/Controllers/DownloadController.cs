using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace WebApplication2.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DownloadController(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAsync(
            string name)
        {
            var cloudStorageAccount =
                CloudStorageAccount.Parse(
                    _configuration["AzureStorage:ConnectionString"]);

            var cloudBlobClient =
                cloudStorageAccount.CreateCloudBlobClient();

            var cloudBlobContainer =
                cloudBlobClient.GetContainerReference(
                    _configuration["AzureStorage:FilePath"]);


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