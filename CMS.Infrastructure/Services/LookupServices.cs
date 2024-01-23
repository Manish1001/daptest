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

    public class LookupServices : ILookupService
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserInfo currentUserInfo;
        private readonly IUnitOfWork unitOfWork;

        public LookupServices(IMapper mapper, ICurrentUserInfo currentUserInfo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.currentUserInfo = currentUserInfo;
            this.currentUserInfo.Seed();
            this.unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<LookupModel>> FetchAsync(LookupFilterModel model)
        {
            var where = $@" WHERE lkp.""IsActive"" IS {(model.IsActive == "A")}  ";
            if (model.LookupTypeId != null && model.LookupTypeId != 0)
            {
                where += $@" AND lkp.""LookupTypeId"" = {model.LookupTypeId}";
            }

            if (!string.IsNullOrEmpty(model.LookupValue))
            {
                where += $@" AND lkp.""LookupValue"" ILIKE '%{model.LookupValue}%'";
            }

            if (!string.IsNullOrEmpty(model.LookupCode))
            {
                where += $@" AND lkp.""LookupCode"" ILIKE '%{model.LookupCode}%'";
            }

            if (!string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(lkp.ModifiedDate, lkp.CreatedDate)::DATE = '{model.FromDate}'::DATE";
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(lkp.ModifiedDate, lkp.CreatedDate)::DATE >= '{model.FromDate}'::DATE";
                where += $@" AND COALESCE(lkp.ModifiedDate, lkp.CreatedDate)::DATE <= '{model.ToDate}'::DATE";
            }

            var query = $@"SELECT COUNT(*) OVER() AS ""TotalItems"", lkp.*, COALESCE(lkp.""ModifiedDate"", lkp.""CreatedDate"") As ""LastModifiedDate"",
                        lkpType.""LookupTypeName"", crt.""FullName"" AS ""CreatedByName"", mdf.""FullName"" AS ""ModifiedByName""
                        FROM ""Lookup"" lkp
                        JOIN ""LookupType"" lkpType ON lkpType.""LookupTypeId"" = lkp.""LookupTypeId"" AND lkpType.""IsActive"" IS TRUE
                        LEFT JOIN ""UserAccount"" crt ON crt.""UserAccountId"" = lkp.""CreatedBy""
                        LEFT JOIN ""UserAccount"" mdf ON mdf.""UserAccountId"" = lkp.""ModifiedBy"" {where}";

            switch (model.SortOrder)
            {
                case "NEW":
                    query += @" Order by lkp.""LookupId"" DESC";
                    break;
                case "OLD":
                    query += @" Order by lkp.""LookupId"" ASC";
                    break;
                case "RECENT":
                    query += @" Order by COALESCE(lkp.""ModifiedDate"", lkp.""CreatedDate"") DESC";
                    break;
            }

            if (model.IsPaginationEnabled)
            {
                model.PageIndex -= 1;
                query += " LIMIT " + model.PageSize + " offset " + (model.PageIndex * model.PageSize);
            }

            return this.unitOfWork.Connection.QueryAsync<LookupModel>(query);
        }

        public async Task<int> AddAsync(LookupModel model)
        {
            var isExists = this.unitOfWork.LookupRepository.Table.Any(m => m.LookupTypeId == model.LookupTypeId && m.LookupName.ToLower() == model.LookupName.ToLower());
            if (isExists)
            {
                return -1;
            }

            var lookup = this.mapper.Map<Lookup>(model);
            lookup.IsActive = true;
            lookup.CreatedBy = this.currentUserInfo.UserAccountId;
            lookup.CreatedDate = DateTime.Now;

            await this.unitOfWork.LookupRepository.AddAsync(lookup);
            await this.unitOfWork.CommitAsync();
            return lookup.LookupId;
        }

        public async Task<int> UpdateAsync(LookupModel model)
        {
            var isExists = this.unitOfWork.LookupRepository.Table.Any(m => m.LookupTypeId == model.LookupTypeId && m.LookupId != model.LookupId && m.LookupValue.ToLower() == model.LookupValue.ToLower());
            if (isExists)
            {
                return -1;
            }

            var lookup = await this.unitOfWork.LookupRepository.FindAsync(model.LookupId);
            lookup.LookupTypeId = model.LookupTypeId;
            lookup.LookupValue = model.LookupValue;
            lookup.LookupCode = model.LookupCode;
            lookup.LookupName = model.LookupName;
            lookup.Description = model.Description;
            lookup.ModifiedBy = this.currentUserInfo.UserAccountId;
            lookup.ModifiedDate = DateTime.Now;

            await this.unitOfWork.LookupRepository.UpdateAsync(lookup);
            return await this.unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteAsync(int lookupId)
        {
            var lookup = await this.unitOfWork.LookupRepository.FindAsync(lookupId);
            await this.unitOfWork.LookupRepository.DeleteAsync(lookup);
            return await this.unitOfWork.CommitAsync();
        }

        public async Task<int> ModifyStatusAsync(int lookupId, bool isActive)
        {
            var lookup = await this.unitOfWork.LookupRepository.FindAsync(lookupId);
            lookup.IsActive = isActive;
            lookup.ModifiedBy = this.currentUserInfo.UserAccountId;
            lookup.ModifiedDate = DateTime.Now;

            await this.unitOfWork.LookupRepository.UpdateAsync(lookup);
            return await this.unitOfWork.CommitAsync();
        }
    }
}