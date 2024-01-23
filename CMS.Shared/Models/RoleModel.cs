namespace CMS.Shared.Models
{
    using Utils;

    public class RoleModel : BaseModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string Permissions { get; set; }
    }
}
