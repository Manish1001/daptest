namespace CMS.Domain.Helpers
{
    public interface IFileHelper
    {
        Task<string> SaveProfileImageAsync(string base64Image, string fileName);
    }
}
