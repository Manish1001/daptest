namespace CMS.Shared.Models
{
    using Utils;

    public class UserPermissionModel : BaseModifiedModel
    {
        public int UserPermissionId { get; set; }

        public int UserId { get; set; }

        public int PermissionTypeId { get; set; }

        public bool IsRequested { get; set; }

        public int? RequestedTo { get; set; }

        public DateTime? RequestedDate { get; set; }

        public bool? IsApproved { get; set; }

        public string Comments { get; set; }
    }
}
