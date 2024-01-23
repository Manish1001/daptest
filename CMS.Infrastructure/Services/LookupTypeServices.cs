namespace CMS.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Dapper;
    using Domain;
    using Domain.Services;
    using Shared.Models;

    public class LookupTypeServices : ILookupTypeService
    {
        private readonly IUnitOfWork unitOfWork;

        public LookupTypeServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<LookupTypeModel>> FetchAsync()
        {
            const string Query = @"SELECT COUNT(*) OVER() AS ""TotalItems"", lkpType.*,
                        (SELECT COUNT(*) FROM ""Lookup"" WHERE ""Lookup"".""LookupTypeId"" = lkpType.""LookupTypeId"") AS NoOfLookups,
                        COALESCE(lkpType.""ModifiedDate"", lkpType.""CreatedDate"") As ""LastModifiedDate""
                        FROM ""LookupType"" lkpType WHERE ""IsActive"" IS TRUE
                        Order by lkpType.""LookupTypeName"" ASC";

            return this.unitOfWork.Connection.QueryAsync<LookupTypeModel>(Query);
        }
    }
}