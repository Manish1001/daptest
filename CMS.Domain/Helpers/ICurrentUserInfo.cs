namespace CMS.Domain.Helpers
{
    public interface ICurrentUserInfo
    {
        int UserAccountId { get; }

        int UserId { get; }

        string Username { get; }

        int RoleId { get; }

        string RoleName { get; }

        string FullName { get; }

        void Seed();
    }
}
