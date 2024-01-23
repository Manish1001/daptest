namespace CMS.Infrastructure.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::Dapper;
    using Domain;
    using Domain.Services;
    using Shared.Models;

    public class RoleServices : IRoleService
    {
        private readonly IUnitOfWork unitOfWork;

        public RoleServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<RoleModel>> FetchAsync()
        {
            const string Query = @"SELECT COUNT(*) OVER() AS ""TotalItems"", rl.*,
                        (SELECT COUNT(*) FROM ""UserAccount"" WHERE ""UserAccount"".""RoleId"" = rl.""RoleId"") AS ""NoOfAccounts""
                        FROM ""Role"" rl WHERE rl.""IsActive"" IS TRUE
                        Order by rl.""RoleName"" ASC";

            return this.unitOfWork.Connection.QueryAsync<RoleModel>(Query);
        }
    }
}