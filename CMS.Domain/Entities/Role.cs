namespace CMS.Domain.Entities
{
    using Common;

    public class Role : BaseEntity
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Permissions { get; set; }
    }
}
