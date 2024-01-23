namespace CMS.Domain.Services
{
    using Shared.FilterModels;
    using Shared.Models;

    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogModel>> FetchAsync(AuditLogFilterModel model);

        Task<AuditLogModel> FindAsync(int auditLogId);

        Task AddAsync(AuditLogEntryModel model);
    }
}