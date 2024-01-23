namespace CMS.Shared.Models
{
    using Utils;

    public class UserCredentialModel : BaseCreatedModel
    {
        public int UserCredentialId { get; set; }

        public int UserAccountId { get; set; }

        public string PasswordHash { get; set; }

        public short Days { get; set; }
    }
}
