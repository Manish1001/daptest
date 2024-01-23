namespace CMS.Shared.FilterModels
{
    using Shared.Utils;

    public class UserFilterModel : BaseFilter
    {
        public int UserId { get; set; }

        public int? CafeteriaLocationId { get; set; }

        public string FullName { get; set; }

        public string EmployeeId { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public int? RoleId { get; set; }
    }
}
