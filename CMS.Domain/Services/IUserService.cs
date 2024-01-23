namespace CMS.Domain.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Shared.FilterModels;
    using Shared.Models;

    public interface IUserService
    {
        Task<IEnumerable<UserModel>> FetchAsync(UserFilterModel model);

        Task<UserModel> FindAsync(int userId);

        Task<int> AddAsync(UserModel model);

        Task<int> UpdateAsync(UserModel model);

        Task<int> UpdateProfileImageAsync(FileRequestModel model);

        Task<int> ModifyStatusAsync(int userId, bool isActive);
    }
}