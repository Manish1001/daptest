namespace CMS.Shared.FilterModels
{
    using Utils;

    public class AuditLogFilterModel : BaseFilter
    {
        public AuditLogFilterModel()
        {
            this.LogTypes = new List<string>();
        }

        public int AuditLogId { get; set; }

        public List<string> LogTypes { get; set; }

        public string LogDescription { get; set; }

        public int? CustomerId { get; set; }

        public int? UserAccountId { get; set; }

        public int? RoleId { get; set; }
    }
}
