namespace CMS.Domain.Entities
{
    using Common;

    public class UserAccount : BaseEntity
    {
        public int UserAccountId { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public string EmployeeId { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLoggedInDate { get; set; }

        public bool IsLocked { get; set; }

        public short FailedLoginAttempts { get; set; }

        public DateTime? LastFailedLoginDate { get; set; }

        public string HashKey { get; set; }

        public bool IsSystemGeneratedPassword { get; set; }

        public bool IsPasswordReset { get; set; }
    }
}
