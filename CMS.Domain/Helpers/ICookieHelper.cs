namespace CMS.Domain.Helpers
{
    public interface ICookieHelper
    {
        void Create(string key, string value, int? expiryTime);

        string Get(string key);

        void Remove(string key);

        void DeleteAll(string key);

        void DeleteAll();
    }
}