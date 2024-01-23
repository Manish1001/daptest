namespace CMS.Shared.FilterModels
{
    using Shared.Utils;

    public class UserAccountFilterModel : BaseFilter
    {
        public int UserAccountId { get; set; }

        public string FullName { get; set; }

        public string EmployeeId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int? RoleId { get; set; }
    }
}