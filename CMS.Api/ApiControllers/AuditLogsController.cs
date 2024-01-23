namespace CMS.Api.ApiControllers
{
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Shared.FilterModels;
    using Utilities;

    [Route("api/audit-logs")]
    public class AuditLogsController : BaseApiController
    {
        private readonly IAuditLogService auditLogServices;

        public AuditLogsController(IAuditLogService auditLogServices)
        {
            this.auditLogServices = auditLogServices;
        }

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync([FromBody] AuditLogFilterModel model)
        {
            var auditLogs = await this.auditLogServices.FetchAsync(model);
            return this.Success(auditLogs);
        }

        [HttpPost]
        [Route("find")]
        public async Task<ApiResponse> FindAsync([FromBody] AuditLogFilterModel model)
        {
            var auditLog = await this.auditLogServices.FindAsync(model.AuditLogId);
            return this.Success(auditLog);
        }
    }
}
