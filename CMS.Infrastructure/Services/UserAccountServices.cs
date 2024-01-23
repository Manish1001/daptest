namespace CMS.Infrastructure.Services
{
    using System;
    using System.Threading.Tasks;

    using CMS.Infrastructure.Helpers;

    using global::Dapper;
    using Domain;
    using Domain.Entities;
    using Domain.Helpers;
    using Domain.Services;
    using Microsoft.EntityFrameworkCore;
    using Shared.FilterModels;
    using Shared.Handlers;
    using Shared.Library;
    using Shared.Models;
    using Role = Shared.Enums.Role;

    public class UserAccountServices : IUserAccountService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserInfo currentUserInfo;
        private readonly IUserTokenService userTokenServices;

        public UserAccountServices(IUnitOfWork unitOfWork, ICurrentUserInfo currentUserInfo, IUserTokenService userTokenServices)
        {
            this.unitOfWork = unitOfWork;
            this.currentUserInfo = currentUserInfo;
            this.currentUserInfo.Seed();
            this.userTokenServices = userTokenServices;
        }

        public async Task<Tuple<AccountStatus, AuthResponse>> ValidateAsync(string username, string password)
        {
            // Checking User Account
            var query = $@"SELECT act.*, rl.""RoleName"" FROM ""UserAccount"" act JOIN ""Role"" rl ON rl.""RoleId"" = act.""RoleId"" WHERE UPPER(COALESCE(act.""Username"", '')) = '{username.Trim().ToUpper()}'";
            var account = await this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserAccountModel>(query);
            if (account == null || account.UserAccountId == 0)
            {
                return new Tuple<AccountStatus, AuthResponse>(AccountStatus.InvalidUsername, null);
            }

            query = $@"SELECT *, EXTRACT(DAY FROM NOW()-""CreatedDate"") ""Days"" FROM ""UserCredential"" WHERE ""UserAccountId"" = '{account.UserAccountId}' ORDER BY 1 DESC LIMIT 1";
            var userCredential = await this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserCredentialModel>(query);
            if (userCredential == null || userCredential.UserCredentialId == 0)
            {
                return new Tuple<AccountStatus, AuthResponse>(AccountStatus.InvalidPassword, null);
            }

            // Checking User IsActive Status
            if (!userCredential.IsActive || !account.IsActive)
            {
                return new Tuple<AccountStatus, AuthResponse>(AccountStatus.InactiveUser, null);
            }

            // Checking User Locked Status
            if (account.IsLocked)
            {
                return new Tuple<AccountStatus, AuthResponse>(AccountStatus.LockedUser, null);
            }

            var passwordHash = CipherHandler.GenerateHash(password, account.HashKey);
            if (passwordHash != userCredential.PasswordHash)
            {
                if (!account.IsSystemGeneratedPassword)
                {
                    await this.UpdateFailedLoginAttemptsAsync(account.UserAccountId, account.FailedLoginAttempts, true);
                }

                return new Tuple<AccountStatus, AuthResponse>(AccountStatus.InvalidPassword, null);
            }

            await this.UpdateFailedLoginAttemptsAsync(account.UserAccountId, account.FailedLoginAttempts, false);

            // User Token
            var token = Guid.NewGuid().ToString();
            var referenceToken = CoreHelper.RandomString(10);
            await this.userTokenServices.AddAsync(new UserTokenModel
            {
                UserAccountId = account.UserAccountId,
                Token = token,
                Reference = referenceToken
            });

            var expiresIn = 90 - userCredential.Days;
            var appAccount = new AuthResponse
            {
                UserAccountId = account.UserAccountId,
                UserId = account.UserId,
                RoleId = account.RoleId,
                RoleName = account.RoleName,
                FullName = account.FullName,
                Username = account.Username,
                LastLoggedInDate = account.LastLoggedInDate,
                IsSystemGeneratedPassword = account.IsSystemGeneratedPassword,
                IsPasswordReset = account.IsPasswordReset,
                ExpiresIn = expiresIn > 3 ? 0 : (expiresIn <= 0 ? 0 : expiresIn),
                IsExpired = expiresIn <= 0,
                AccessToken = token,
                ReferenceToken = referenceToken
            };

            return new Tuple<AccountStatus, AuthResponse>(AccountStatus.Success, appAccount);
        }

        public Task<AccountInfoModel> GetInfoAsync(int userAccountId = 0)
        {
            var query = $@"SELECT act.""UserAccountId"", act.""EmployeeId"", act.""FullName"", act.""Username"", act.""RoleId"", rl.""RoleName"", act.""UserId"",
                        usr.""Email"", usr.""ThumbnailPath"" FROM ""UserAccount"" act
                        JOIN ""Role"" rl ON rl.""IsActive"" IS TRUE AND act.""RoleId"" = rl.""RoleId""
                        JOIN ""User"" usr ON usr.""UserId"" = act.""UserId""  AND usr.""IsActive"" IS TRUE
                        WHERE act.""UserAccountId"" = {(userAccountId == 0 ? this.currentUserInfo.UserAccountId : userAccountId)}";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<AccountInfoModel>(query);
        }

        public Task<UserAccountModel> FindAsync(string username, string email)
        {
            var query = $@"SELECT act.* FROM ""UserAccount"" act
                        JOIN ""User"" usr ON usr.""UserId"" = act.""UserId"" AND usr.""IsActive"" IS TRUE
                        WHERE act.""Username"" = '{username}' AND usr.""Email"" = '{email}'";
            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserAccountModel>(query);
        }

        public async Task<int> ChangePasswordAsync(string password, bool forceResetPassword = false)
        {
            var userAccount = await this.unitOfWork.UserAccountRepository.FindAsync(this.currentUserInfo.UserAccountId);
            var passwordHash = CipherHandler.GenerateHash(password, userAccount.HashKey);
            var userCredential = new UserCredential
            {
                IsActive = true,
                UserAccountId = this.currentUserInfo.UserAccountId,
                CreatedDate = DateTime.Now,
                PasswordHash = passwordHash,
                CreatedBy = this.currentUserInfo.UserAccountId
            };

            await this.unitOfWork.UserCredentialRepository.AddAsync(userCredential);
            await this.unitOfWork.CommitAsync();

            if (userCredential.UserCredentialId <= 0)
            {
                return 0;
            }

            await this.unitOfWork.Connection.ExecuteAsync($@"UPDATE ""UserCredential"" SET ""IsActive"" IS FALSE WHERE ""UserAccountId"" = {this.currentUserInfo.UserAccountId} AND ""UserCredentialId"" <> {userCredential.UserCredentialId}");
            if (forceResetPassword)
            {
                await this.unitOfWork.Connection.ExecuteAsync($@"UPDATE ""UserAccount"" SET ""IsSystemGeneratedPassword"" IS FALSE, ""IsPasswordReset"" IS FALSE WHERE ""UserAccountId"" = {this.currentUserInfo.UserAccountId}");
            }

            return userAccount.RoleId;
        }

        public async Task<int> ResetPasswordAsync(int userAccountId, string password, bool isSystemGeneratedPassword)
        {
            var userAccount = await this.unitOfWork.UserAccountRepository.FindAsync(userAccountId);
            var passwordHash = CipherHandler.GenerateHash(password, userAccount.HashKey);
            var userCredential = new UserCredential
            {
                IsActive = true,
                UserAccountId = userAccountId,
                CreatedDate = DateTime.Now,
                PasswordHash = passwordHash,
                CreatedBy = userAccountId
            };

            await this.unitOfWork.UserCredentialRepository.AddAsync(userCredential);
            await this.unitOfWork.CommitAsync();

            if (userCredential.UserCredentialId <= 0)
            {
                return 0;
            }

            await this.unitOfWork.Connection.ExecuteAsync($@"UPDATE ""UserCredential"" SET ""IsActive"" IS FALSE WHERE ""UserAccountId"" = {this.currentUserInfo.UserAccountId} AND ""UserCredentialId"" <> {userCredential.UserCredentialId}");
            if (isSystemGeneratedPassword)
            {
                await this.unitOfWork.Connection.ExecuteAsync($@"UPDATE ""UserAccount"" SET ""IsSystemGeneratedPassword"" IS TRUE WHERE ""UserAccountId"" = {this.currentUserInfo.UserAccountId}");
            }

            return userAccount.RoleId;
        }

        public Task<IEnumerable<UserAccountModel>> FetchAsync(UserAccountFilterModel model)
        {
            var where = $@" WHERE 1 = 1 AND ua.""RoleId"" <> {(int)Role.Customer} ";
            if (model.RoleId != null && model.RoleId != 0)
            {
                where += $@" AND ua.""RoleId"" = {model.RoleId}";
            }

            if (!string.IsNullOrEmpty(model.FullName))
            {
                where += $@" AND usr.""FullName"" ILIKE '%{model.FullName}%'";
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                where += $@" AND usr.""Email"" ILIKE '%{model.Email}%'";
            }

            if (!string.IsNullOrEmpty(model.Mobile))
            {
                where += $@" AND usr.""Mobile"" ILIKE '%{model.Mobile}%'";
            }

            if (!string.IsNullOrEmpty(model.EmployeeId))
            {
                where += $@" AND usr.""EmployeeId"" ILIKE '%{model.EmployeeId}%'";
            }

            if (!string.IsNullOrEmpty(model.IsActive))
            {
                switch (model.IsActive)
                {
                    case "L":
                        where += @" AND ua.""IsLocked"" IS TRUE ";
                        break;
                    case "I":
                        where += @" AND ua.""IsActive"" IS FALSE ";
                        break;
                    case "A":
                        where += @" AND ua.""IsActive"" IS TRUE AND ua.""IsLocked"" IS FALSE ";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(ua.""ModifiedDate"", ua.""CreatedDate"")::DATE = '{model.FromDate}'::DATE";
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(ua.""ModifiedDate"", ua.""CreatedDate"")::DATE >= '{model.FromDate}'::DATE";
                where += $@" AND COALESCE(ua.""ModifiedDate"", ua.""CreatedDate"")::DATE <= '{model.ToDate}'::DATE";
            }

            var query = $@"SELECT COUNT(*) OVER() AS ""TotalItems"", ua.*, rl.""RoleName"", COALESCE(ua.""ModifiedDate"", ua.""CreatedDate"") AS ""LastModifiedDate"",
                        usr.""ThumbnailPath"", usr.""Email"", usr.""Mobile"",
                        crt.""FullName"" AS ""CreatedByName"", mdf.""FullName"" AS ""ModifiedByName""
                        FROM ""UserAccount"" ua
                        JOIN ""User"" usr ON ua.""UserId"" = usr.""UserId""
                        JOIN ""Role"" rl ON rl.""RoleId"" = ua.""RoleId"" AND rl.""IsActive"" IS TRUE
                        LEFT JOIN ""UserAccount"" crt ON crt.""UserAccountId"" = usr.""CreatedBy""
                        LEFT JOIN ""UserAccount"" mdf ON mdf.""UserAccountId"" = usr.""ModifiedBy"" {where}";

            switch (model.SortOrder)
            {
                case "NEW":
                    query += @" Order by ua.""UserAccountId"" DESC";
                    break;
                case "OLD":
                    query += @" Order by ua.""UserAccountId"" ASC";
                    break;
                case "RECENT":
                    query += @" Order by COALESCE(ua.""ModifiedDate"", ua.""CreatedDate"") DESC";
                    break;
            }

            if (model.IsPaginationEnabled)
            {
                model.PageIndex -= 1;
                query += " LIMIT " + model.PageSize + " offset " + (model.PageIndex * model.PageSize);
            }

            return this.unitOfWork.Connection.QueryAsync<UserAccountModel>(query);
        }

        public async Task<int> ModifyStatusAsync(int userAccountId, bool isActive)
        {
            this.unitOfWork.BeginTransaction();

            // User
            var userAccount = await this.unitOfWork.UserAccountRepository.FindAsync(userAccountId);
            userAccount.IsActive = isActive;
            userAccount.ModifiedBy = this.currentUserInfo.UserAccountId;
            userAccount.ModifiedDate = DateTime.Now;
            await this.unitOfWork.UserAccountRepository.UpdateAsync(userAccount);

            // User Account
            var userCredential = await this.unitOfWork.UserCredentialRepository.Table.OrderByDescending(m => m.UserAccountId).FirstOrDefaultAsync(m => m.UserAccountId == userAccount.UserAccountId);
            if (userCredential != null)
            {
                userCredential.IsActive = isActive;
                await this.unitOfWork.UserCredentialRepository.UpdateAsync(userCredential);
            }

            var updated = await this.unitOfWork.CommitAsync();
            this.unitOfWork.CommitTransaction();
            return updated;
        }

        public Task<int> ModifyLockStatusAsync(int userAccountId, bool isLocked)
        {
            var where = $@" WHERE ""UserAccountId"" = '{userAccountId}'";
            var query = isLocked ?
                            $@"UPDATE ""UserAccount"" SET ""IsLocked"" = TRUE, ""ModifiedBy"" = {this.currentUserInfo.UserAccountId}, ""ModifiedDate"" = NOW() {where}" :
                            $@"UPDATE ""UserAccount"" SET ""FailedLoginAttempts"" = 0, ""IsLocked"" = FALSE, ""ModifiedBy"" = {this.currentUserInfo.UserAccountId}, ""ModifiedDate"" = NOW() {where}";

            return this.unitOfWork.Connection.ExecuteAsync(query);
        }

        public Task<int> ForceResetPasswordAsync(int userAccountId)
        {
            var query = $@"UPDATE ""UserAccount"" SET ""IsPasswordReset"" = TRUE, ""ModifiedBy"" = {this.currentUserInfo.UserAccountId}, ""ModifiedDate"" = NOW() WHERE ""UserAccountId"" = {userAccountId}";
            return this.unitOfWork.Connection.ExecuteAsync(query);
        }

        private Task<int> UpdateFailedLoginAttemptsAsync(int userAccountId, short failedLoginAttempts, bool invalidPassword)
        {
            var where = $@" WHERE ""UserAccountId"" = '{userAccountId}'";
            var locked = failedLoginAttempts >= 4;
            var query = invalidPassword ?
                            $@"UPDATE ""UserAccount"" SET ""FailedLoginAttempts"" = ""FailedLoginAttempts"" + 1, ""IsLocked"" = {locked}, ""LastFailedLoginDate"" = NOW() {where}" :
                            $@"UPDATE ""UserAccount"" SET ""FailedLoginAttempts"" = 0, ""IsLocked"" = {locked}, ""LastLoggedInDate"" = NOW() {where}";

            return this.unitOfWork.Connection.ExecuteAsync(query);
        }
    }
}