namespace CMS.Domain.Services
{
    using Shared.Models;

    public interface IUserTokenService
    {
        Task<UserTokenModel> FindAsync(string token);

        Task<UserTokenModel> FindAsync(string reference, string token);

        Task AddAsync(UserTokenModel model);

        Task<int> UpdateAsync(int userTokenId, string token);

        Task DeleteExpiredTokensAsync(int userAccountId);

        Task DeleteAllExpiredTokensAsync();
    }
}