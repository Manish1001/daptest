namespace CMS.Api.Models
{
    public class ResetPasswordRequestModel
    {
        public int UserAccountId { get; set; }

        public string Password { get; set; }

        public bool IsSystemGeneratedPassword { get; set; }
    }
}
