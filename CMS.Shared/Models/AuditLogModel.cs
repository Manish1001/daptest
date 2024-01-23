namespace CMS.Shared.Models
{
    public class AuditLogModel
    {
        public int AuditLogId { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int UserAccountId { get; set; }

        public string Username { get; set; }

        public string UserFullName { get; set; }

        public string EmployeeId { get; set; }

        public int? CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string LogType { get; set; }

        public DateTime LogDate { get; set; }

        public string LogDescription { get; set; }

        public int TotalItems { get; set; }
    }
}