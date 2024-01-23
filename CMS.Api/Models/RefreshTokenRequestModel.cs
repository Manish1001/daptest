namespace CMS.Api.Models
{
    public class RefreshTokenRequestModel
    {
        public string AccessToken { get; set; }

        public string ReferenceToken { get; set; }
    }
}
