namespace CMS.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Shared.Models;

    public interface IRoleService
    {
        Task<IEnumerable<RoleModel>> FetchAsync();
    }
}