using ACRMS.Repository.IRepository;
using Microsoft.AspNetCore.Components.Forms;

namespace ACRMS.Repository
{
    public class FileUploadService: IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> UploadAsync(IBrowserFile file, string folderName, long maxFileSizeInBytes = 5 * 1024 * 1024)
        {
            if (file is null || file.Size == 0)
                return null;

            var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(file.Name);
            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsRoot, safeFileName);

            await using var stream = file.OpenReadStream(maxFileSizeInBytes);
            await using var fileStream = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            return $"/uploads/{folderName}/{safeFileName}";
        }

        public Task<bool> DeleteAsync(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.FromResult(false);

            var cleanPath = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_environment.WebRootPath, cleanPath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}