namespace CMS.Domain.Services
{
    using Shared.FilterModels;
    using Shared.Library;
    using Shared.Models;

    public interface IUserAccountService
    {
        Task<Tuple<AccountStatus, AuthResponse>> ValidateAsync(string username, string password);

        Task<AccountInfoModel> GetInfoAsync(int userAccountId = 0);

        Task<UserAccountModel> FindAsync(string username, string email);

        Task<int> ResetPasswordAsync(int userAccountId, string password, bool isSystemGeneratedPassword);

        Task<int> ChangePasswordAsync(string password, bool forceResetPassword = false);

        Task<IEnumerable<UserAccountModel>> FetchAsync(UserAccountFilterModel model);

        Task<int> ModifyStatusAsync(int userAccountId, bool isActive);

        Task<int> ModifyLockStatusAsync(int userAccountId, bool isLocked);

        Task<int> ForceResetPasswordAsync(int userAccountId);
    }
}
