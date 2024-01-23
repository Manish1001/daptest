namespace CMS.Shared.Models
{
    using Utils;

    public class UserModel : BaseModel
    {
        public int UserId { get; set; }

        public int CafeteriaLocationId { get; set; }

        public string CafeteriaLocationName { get; set; }

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

        public int UserAccountId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Username { get; set; }

        public string EmployeeId { get; set; }

        public string Password { get; set; } = "Test@1234";
    }
}
