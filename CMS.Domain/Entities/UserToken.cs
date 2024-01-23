namespace CMS.Domain.Entities
{
    public class UserToken
    {
        public int UserTokenId { get; set; }

        public int UserAccountId { get; set; }

        public string Token { get; set; }

        public string Reference { get; set; }

        public DateTime CreatedDate { get; set; }

        public short ExpiresIn { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
