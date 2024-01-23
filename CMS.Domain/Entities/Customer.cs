namespace CMS.Domain.Entities
{
    using Common;

    public class Customer : BaseEntity
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string ThumbnailPath { get; set; }

        public string CustomerCode { get; set; }

        public int RoleId { get; set; }

        public string Username { get; set; }

        public string PasswordKey { get; set; }

        public string PasswordHash { get; set; }
    }
}
