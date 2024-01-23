namespace CMS.Infrastructure.Helpers
{
    using System.Security.Claims;
    using Domain.Helpers;
    using Microsoft.AspNetCore.Http;

    public class CurrentUserInfo : ICurrentUserInfo
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserInfo(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public CurrentUserInfo(int userAccountId, int userId, string username, int roleId, string roleName, string fullName)
        {
            this.UserAccountId = userAccountId;
            this.UserId = userId;
            this.Username = username;
            this.RoleId = roleId;
            this.RoleName = roleName;
            this.FullName = fullName;
        }

        public int UserAccountId { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string FullName { get; set; }

        public void Seed()
        {
            var identity = this.httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

            var userAccountId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            this.UserAccountId = !string.IsNullOrEmpty(userAccountId) ? Convert.ToInt32(userAccountId) : 0;

            var roleId = identity?.FindFirst("RoleId")?.Value;
            this.RoleId = !string.IsNullOrEmpty(roleId) ? Convert.ToInt32(roleId) : 0;

            var userId = identity?.FindFirst("UserId")?.Value;
            this.UserId = !string.IsNullOrEmpty(userId) ? Convert.ToInt32(userId) : 0;

            this.Username = identity?.FindFirst(ClaimTypes.Name)?.Value;
            this.FullName = identity?.FindFirst(ClaimTypes.GivenName)?.Value;
            this.RoleName = identity?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
