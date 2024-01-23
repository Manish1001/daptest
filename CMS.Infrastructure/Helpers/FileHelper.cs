namespace CMS.Infrastructure.Helpers
{
    using Domain.Helpers;
    using Microsoft.AspNetCore.Hosting;

    public class FileHelper : IFileHelper
    {
        private const string FilesDirectory = "Files";

        private const string ProfileImagesDirectory = "ProfileImages";

        private readonly IHostingEnvironment environment;

        public FileHelper(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        public async Task<string> SaveProfileImageAsync(string base64Image, string fileName)
        {
            // Files
            var folderPath = Path.Combine(this.environment.ContentRootPath, FilesDirectory);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Profile Images
            folderPath = Path.Combine(folderPath, ProfileImagesDirectory);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Employee ID
            var parentFolderName = string.Empty;
            if (fileName.IndexOf("/", StringComparison.Ordinal) > 0)
            {
                var parts = fileName.Split('/');
                parentFolderName = parts[0];
                folderPath = Path.Combine(folderPath, parentFolderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                fileName = parts[1];
            }

            fileName = CoreHelper.Replace(" ", "_", fileName);
            await File.WriteAllBytesAsync(Path.Combine(folderPath, CoreHelper.Replace(" ", "_", fileName)), CoreHelper.ToImageBytes(base64Image));
            return "profile-images/" + (string.IsNullOrEmpty(parentFolderName) ? fileName : parentFolderName + "/" + fileName);
        }
    }
}
