namespace CMS.Shared.Models
{
    public class AuditLogEntryModel
    {
        public int? CustomerId { get; set; }

        public int UserAccountId { get; set; }

        public int RoleId { get; set; }

        public string TableName { get; set; }

        public int? ReferenceId { get; set; }

        public string LogType { get; set; }

        public string LogDescription { get; set; }
    }
}
