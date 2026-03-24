using Microsoft.AspNetCore.Components.Forms;

namespace ACRMS.Repository.IRepository
{
    public interface IFileUploadService
    {
        Task<string?> UploadAsync(IBrowserFile file, string folderName, long maxFileSizeInBytes = 5 * 1024 * 1024);
        Task<bool> DeleteAsync(string? relativePath);
    }
}
