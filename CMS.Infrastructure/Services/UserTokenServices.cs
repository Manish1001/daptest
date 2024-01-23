namespace CMS.Infrastructure.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using global::Dapper;
    using Domain;
    using Domain.Entities;
    using Domain.Helpers;
    using Domain.Services;
    using Shared.Models;

    public class UserTokenServices : IUserTokenService
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserInfo currentUserInfo;
        private readonly IUnitOfWork unitOfWork;

        public UserTokenServices(IMapper mapper, ICurrentUserInfo currentUserInfo, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.currentUserInfo = currentUserInfo;
            this.currentUserInfo.Seed();
            this.unitOfWork = unitOfWork;
        }

        public Task<UserTokenModel> FindAsync(string token)
        {
            var where = $@" WHERE ut.""Token"" = '{token}' AND ut.""IsActive"" IS TRUE";
            var query = $@"SELECT ut.*, ua.""RoleId"", rl.""RoleName"", ua.""UserId"", ua.""Username"", ua.""FullName""
                        FROM ""UserToken"" ut
                        JOIN ""UserAccount"" ua ON ua.""UserAccountId"" = ut.""UserAccountId"" AND ua.""IsActive"" IS TRUE
                        JOIN ""Role"" rl ON ua.""RoleId"" = rl.""RoleId"" AND rl.""IsActive"" IS TRUE
                        {where}";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserTokenModel>(query);
        }

        public Task<UserTokenModel> FindAsync(string reference, string token)
        {
            var where = $@" WHERE ut.""Token"" = '{token}' AND ut.""Reference"" = '{reference}' AND ut.""IsActive"" IS TRUE";
            var query = $@"SELECT ut.*, ua.""RoleId"", rl.""RoleName"", ua.""UserId"", ua.""Username"", ua.""FullName""
                        FROM ""UserToken"" ut
                        JOIN ""UserAccount"" ua ON ua.""UserAccountId"" = ut.""UserAccountId"" AND ua.""IsActive"" IS TRUE
                        JOIN ""Role"" rl ON ua.""RoleId"" = rl.""RoleId"" AND rl.""IsActive"" IS TRUE
                        {where}";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserTokenModel>(query);
        }

        public async Task AddAsync(UserTokenModel model)
        {
            var userToken = this.mapper.Map<UserToken>(model);
            userToken.UserAccountId = model.UserAccountId == 0 ? this.currentUserInfo.UserAccountId : model.UserAccountId;
            userToken.IsActive = true;
            userToken.CreatedDate = DateTime.Now;
            userToken.ExpiresIn = 20;
            userToken.ExpiryDate = DateTime.Now.AddMinutes(userToken.ExpiresIn);

            await this.unitOfWork.UserTokenRepository.AddAsync(userToken);
            await this.unitOfWork.CommitAsync();
        }

        public async Task<int> UpdateAsync(int userTokenId, string token)
        {
            var userToken = await this.unitOfWork.UserTokenRepository.FindAsync(userTokenId);
            userToken.Token = token;
            userToken.ExpiresIn = 20;
            userToken.ExpiryDate = DateTime.Now.AddMinutes(userToken.ExpiresIn);
            userToken.ModifiedDate = DateTime.Now;

            await this.unitOfWork.UserTokenRepository.UpdateAsync(userToken);
            return await this.unitOfWork.CommitAsync();
        }

        public Task DeleteExpiredTokensAsync(int userTokenId)
        {
            var where = $@" WHERE ""UserTokenId"" = {userTokenId} AND ""ExpiryDate"" < NOW() - INTERVAL '1 minute'";
            var query = $@"DELETE FROM ""UserToken"" {where}";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserTokenModel>(query);
        }

        public Task DeleteAllExpiredTokensAsync()
        {
            const string Where = $@" WHERE ""ExpiryDate"" < NOW() - INTERVAL '1 minute'";
            const string Query = $@"DELETE FROM ""UserToken"" {Where}";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserTokenModel>(Query);
        }
    }
}