namespace CMS.Domain.Entities
{

    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public int RoleId { get; set; }

        public int UserAccountId { get; set; }

        public int? CustomerId { get; set; }

        public string TableName { get; set; }

        public int? ReferenceId { get; set; }

        public string LogType { get; set; }

        public DateTime LogDate { get; set; }

        public string LogDescription { get; set; }
    }
}
