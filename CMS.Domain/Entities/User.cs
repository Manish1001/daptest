namespace CMS.Domain.Entities
{
    using Common;

    public class User : BaseEntity
    {
        public int UserId { get; set; }

        public int CafeteriaLocationId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string MaritalStatus { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string ThumbnailPath { get; set; }
    }
}
