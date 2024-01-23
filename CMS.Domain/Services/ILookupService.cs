namespace CMS.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Shared.FilterModels;
    using Shared.Models;

    public interface ILookupService
    {
        Task<IEnumerable<LookupModel>> FetchAsync(LookupFilterModel model);

        Task<int> AddAsync(LookupModel model);

        Task<int> UpdateAsync(LookupModel model);

        Task<int> DeleteAsync(int lookupId);

        Task<int> ModifyStatusAsync(int lookupId, bool isActive);
    }
}