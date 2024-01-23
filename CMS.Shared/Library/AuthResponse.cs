namespace CMS.Shared.Library
{
    public class AuthResponse
    {
        public int UserAccountId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string FullName { get; set; }

        public DateTime LoggedInDate { get; set; }

        public DateTime? LastLoggedInDate { get; set; }

        public int ExpiresIn { get; set; }

        public bool IsExpired { get; set; }

        public bool IsSystemGeneratedPassword { get; set; }

        public bool IsPasswordReset { get; set; }

        public string AccessToken { get; set; }

        public string ReferenceToken { get; set; }
    }
}
