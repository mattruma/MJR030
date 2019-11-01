using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace WebApplication1
{
    public class UploadModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UploadModel(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPost(
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

            return RedirectToPage("Download", new { name = blobName });
        }
    }
}