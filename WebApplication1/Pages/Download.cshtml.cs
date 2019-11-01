using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace WebApplication1
{
    public class DownloadModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public DownloadModel(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(
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