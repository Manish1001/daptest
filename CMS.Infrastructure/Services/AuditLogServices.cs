namespace CMS.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using global::Dapper;
    using Domain;
    using Domain.Entities;
    using Domain.Helpers;
    using Domain.Services;
    using Shared.FilterModels;
    using Shared.Models;

    public class AuditLogServices : IAuditLogService
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserInfo currentUserInfo;
        private readonly IUnitOfWork unitOfWork;

        public AuditLogServices(IMapper mapper, ICurrentUserInfo currentUserInfo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.currentUserInfo = currentUserInfo;
            this.currentUserInfo.Seed();
            this.unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<AuditLogModel>> FetchAsync(AuditLogFilterModel model)
        {
            var where = $@" WHERE 1 = 1 ";
            if (!string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND log.""LogDate""::DATE = '{model.FromDate}'::DATE";
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND log.""LogDate""::DATE >= '{model.FromDate}'::DATE";
                where += $@" AND log.""LogDate""::DATE <= '{model.ToDate}'::DATE";
            }

            if (model.LogTypes.Count > 0)
            {
                where += $@" AND log.""LogType"" IN ({string.Join(",", model.LogTypes.Select(m => "'" + m + "'"))})";
            }

            if (!string.IsNullOrEmpty(model.LogDescription))
            {
                where += $@" AND log.""LogDescription"" ILIKE '%{model.LogDescription}%'";
            }

            if (model.CustomerId != null && model.CustomerId > 0)
            {
                where += $@" AND log.""CustomerId"" = {model.CustomerId}";
            }

            if (model.UserAccountId != null && model.UserAccountId > 0)
            {
                where += $@" AND log.""UserAccountId"" = {model.UserAccountId}";
            }

            if (model.RoleId != null && model.RoleId > 0)
            {
                where += $@" AND log.""RoleId"" = {model.RoleId}";
            }

            var query = $@"SELECT COUNT(*) OVER() AS ""TotalItems"", log.*, usrAct.""Username"", usrAct.""FullName"" As ""UserFullName"", usrAct.""EmployeeId"",
                        rl.""RoleName"", cus.""FullName"" as ""CustomerName""
                        FROM ""AuditLog"" log
                        JOIN ""UserAccount"" usrAct ON usrAct.""UserAccountId"" = log.""UserAccountId""
                        JOIN ""Role"" rl ON rl.""RoleId"" = usrAct.""RoleId""
                        LEFT JOIN ""Customer"" cus ON cus.""CustomerId"" = log.""CustomerId"" {where}";

            switch (model.SortOrder)
            {
                case "NEW":
                    query += @" Order by log.""AuditLogId"" DESC";
                    break;
                case "OLD":
                    query += @" Order by log.""AuditLogId"" ASC";
                    break;
            }

            if (model.IsPaginationEnabled)
            {
                model.PageIndex -= 1;
                query += " LIMIT " + model.PageSize + " offset " + (model.PageIndex * model.PageSize);
            }

            return this.unitOfWork.Connection.QueryAsync<AuditLogModel>(query);
        }

        public Task<AuditLogModel> FindAsync(int auditLogId)
        {
            var where = $@" WHERE log.""AuditLogId"" = {auditLogId} ";
            var query = $@"SELECT log.*, usrAct.""Username"", usrAct.""FullName"" As ""UserFullName"", usrAct.""EmployeeId"", rl.""RoleName"",
                        cus.""FullName"" as ""CustomerName""
                        FROM ""AuditLog"" log
                        JOIN ""UserAccount"" usrAct ON usrAct.""UserAccountId"" = log.""UserAccountId""
                        JOIN ""Role"" rl ON rl.""RoleId"" = usrAct.""RoleId""
                        LEFT JOIN ""Customer"" cus ON cus.""CustomerId"" = log.""CustomerId"" {where}";

            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<AuditLogModel>(query);
        }

        public async Task AddAsync(AuditLogEntryModel model)
        {
            var auditLog = this.mapper.Map<AuditLog>(model);
            auditLog.RoleId = model.RoleId == 0 ? this.currentUserInfo.RoleId : model.RoleId;
            auditLog.UserAccountId = model.UserAccountId == 0 ? this.currentUserInfo.UserAccountId : model.UserAccountId;
            auditLog.LogDate = DateTime.Now;

            await this.unitOfWork.AuditLogRepository.AddAsync(auditLog);
            await this.unitOfWork.CommitAsync();
        }
    }
}