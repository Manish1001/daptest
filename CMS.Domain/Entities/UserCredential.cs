namespace CMS.Domain.Entities
{
    using Common;

    public class UserCredential : BaseCreatedEntity
    {
        public int UserCredentialId { get; set; }

        public int UserAccountId { get; set; }

        public string PasswordHash { get; set; }
    }
}
