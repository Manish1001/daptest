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
    using Infrastructure.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Shared.FilterModels;
    using Shared.Handlers;
    using Shared.Models;

    public class UserServices : IUserService
    {
        private readonly IMapper mapper;
        private readonly ICurrentUserInfo currentUserInfo;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileHelper fileHelper;

        public UserServices(IMapper mapper, ICurrentUserInfo currentUserInfo, IUnitOfWork unitOfWork, IFileHelper fileHelper)
        {
            this.mapper = mapper;
            this.currentUserInfo = currentUserInfo;
            this.currentUserInfo.Seed();
            this.unitOfWork = unitOfWork;
            this.fileHelper = fileHelper;
        }

        public Task<IEnumerable<UserModel>> FetchAsync(UserFilterModel model)
        {
            var where = $@" WHERE usr.""IsActive"" IS {(model.IsActive == "A")}";
            if (model.CafeteriaLocationId != null && model.CafeteriaLocationId != 0)
            {
                where += $@" AND ua.""CafeteriaLocationId"" = {model.CafeteriaLocationId}";
            }

            if (model.RoleId != null && model.RoleId != 0)
            {
                where += $@" AND ua.""RoleId"" = {model.RoleId}";
            }

            if (!string.IsNullOrEmpty(model.EmployeeId))
            {
                where += $@" AND ua.""EmployeeId"" ILIKE '%{model.EmployeeId}%'";
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

            if (!string.IsNullOrEmpty(model.FromDate) && string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"")::DATE = '{model.FromDate}'::DATE";
            }

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.ToDate))
            {
                where += $@" AND COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"")::DATE >= '{model.FromDate}'::DATE";
                where += $@" AND COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"")::DATE <= '{model.ToDate}'::DATE";
            }

            var query = $@"SELECT COUNT(*) OVER() AS ""TotalItems"", usr.*, COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"") AS ""LastModifiedDate"",
                        ua.""UserAccountId"", ua.""EmployeeId"", ua.""Username"", ua.""RoleId"", rl.""RoleName"", cl.""LocationName"" AS ""CafeteriaLocationName""
                        FROM ""User"" usr
                        JOIN ""UserAccount"" ua ON ua.""UserId"" = usr.""UserId""
                        LEFT JOIN ""CafeteriaLocation"" cl ON cl.""CafeteriaLocationId"" = usr.""CafeteriaLocationId""
                        JOIN ""Role"" rl ON rl.""RoleId"" = ua.""RoleId"" AND rl.""IsActive"" IS TRUE {where}";

            switch (model.SortOrder)
            {
                case "NEW":
                    query += @" Order by usr.""UserId"" DESC";
                    break;
                case "OLD":
                    query += @" Order by usr.""UserId"" ASC";
                    break;
                case "RECENT":
                    query += @" Order by COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"") DESC";
                    break;
            }

            if (model.IsPaginationEnabled)
            {
                model.PageIndex -= 1;
                query += " LIMIT " + model.PageSize + " offset " + (model.PageIndex * model.PageSize);
            }

            return this.unitOfWork.Connection.QueryAsync<UserModel>(query);
        }

        public Task<UserModel> FindAsync(int userId)
        {
            var where = $@" WHERE 1 = 1 AND usr.""UserId"" = {userId}";
            var query = $@"SELECT usr.*, COALESCE(usr.""ModifiedDate"", usr.""CreatedDate"") AS ""LastModifiedDate"",
                        ua.""UserAccountId"", ua.""EmployeeId"", ua.""Username"", ua.""RoleId"", rl.""RoleName"", cl.""LocationName"" AS ""CafeteriaLocationName"",
                        crt.""FullName"" AS ""CreatedByName"", mdf.""FullName"" AS ""ModifiedByName""
                        FROM ""User"" usr
                        JOIN ""UserAccount"" ua ON ua.""UserId"" = usr.""UserId""
                        LEFT JOIN ""CafeteriaLocation"" cl ON cl.""CafeteriaLocationId"" = usr.""CafeteriaLocationId""
                        JOIN ""Role"" rl ON rl.""RoleId"" = ua.""RoleId"" AND rl.""IsActive"" IS TRUE
                        LEFT JOIN ""UserAccount"" crt ON crt.""UserAccountId"" = usr.""CreatedBy""
                        LEFT JOIN ""UserAccount"" mdf ON mdf.""UserAccountId"" = usr.""ModifiedBy"" {where}";

            return this.unitOfWork.Connection.QueryFirstOrDefaultAsync<UserModel>(query);
        }

        public async Task<int> AddAsync(UserModel model)
        {
            var isExists = this.unitOfWork.UserRepository.Table.Any(m => m.Email.ToLower() == model.Email.ToLower());
            if (isExists)
            {
                return -1;
            }

            isExists = this.unitOfWork.UserRepository.Table.Any(m => m.Mobile == model.Mobile);
            if (isExists)
            {
                return -2;
            }

            isExists = this.unitOfWork.UserAccountRepository.Table.Any(m => m.Username.ToLower() == model.Username.ToLower());
            if (isExists)
            {
                return -3;
            }

            this.unitOfWork.BeginTransaction();
            var fullName = model.FirstName + " " + model.LastName;

            // User
            var user = this.mapper.Map<User>(model);
            user.FullName = fullName;
            user.IsActive = true;
            user.CreatedBy = this.currentUserInfo.UserAccountId;
            user.CreatedDate = DateTime.Now;
            await this.unitOfWork.UserRepository.AddAsync(user);
            await this.unitOfWork.CommitAsync();

            // User Account
            var passwordKey = CoreHelper.RandomString(10);
            var userAccount = new UserAccount
            {
                UserId = user.UserId,
                EmployeeId = await this.GetLatestEmployeeIdAsync(),
                FullName = user.FullName,
                RoleId = model.RoleId,
                Username = model.Username,
                HashKey = passwordKey,
                IsSystemGeneratedPassword = true,
                IsActive = true,
                CreatedBy = this.currentUserInfo.UserAccountId,
                CreatedDate = DateTime.Now
            };
            await this.unitOfWork.UserAccountRepository.AddAsync(userAccount);
            await this.unitOfWork.CommitAsync();

            // User Credential
            var passwordHash = CipherHandler.GenerateHash(model.Password, passwordKey);
            var accountCredential = new UserCredential
            {
                UserAccountId = userAccount.UserAccountId,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedBy = this.currentUserInfo.UserAccountId,
                CreatedDate = DateTime.Now
            };
            await this.unitOfWork.UserCredentialRepository.AddAsync(accountCredential);
            await this.unitOfWork.CommitAsync();

            this.unitOfWork.CommitTransaction();
            return user.UserId;
        }

        public async Task<int> UpdateAsync(UserModel model)
        {
            var isExists = this.unitOfWork.UserRepository.Table.Any(m => m.UserId != model.UserId && m.Email.ToLower() == model.Email.ToLower());
            if (isExists)
            {
                return -1;
            }

            isExists = this.unitOfWork.UserRepository.Table.Any(m => m.UserId != model.UserId && m.Mobile == model.Mobile);
            if (isExists)
            {
                return -2;
            }

            isExists = this.unitOfWork.UserAccountRepository.Table.Any(m => m.UserId != model.UserId && m.Username.ToLower() == model.Username.ToLower());
            if (isExists)
            {
                return -3;
            }

            this.unitOfWork.BeginTransaction();
            var fullName = model.FirstName + " " + model.LastName;

            // User
            var user = await this.unitOfWork.UserRepository.FindAsync(model.UserId);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.FullName = fullName;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.Email = model.Email;
            user.Mobile = model.Mobile;
            user.Address = model.Address;
            user.ModifiedBy = this.currentUserInfo.UserAccountId;
            user.ModifiedDate = DateTime.Now;
            await this.unitOfWork.UserRepository.UpdateAsync(user);
            var updated = await this.unitOfWork.CommitAsync();

            // User Account
            var userAccount = await this.unitOfWork.UserAccountRepository.FindAsync(model.UserAccountId);
            userAccount.FullName = user.FullName;
            userAccount.RoleId = model.RoleId;
            userAccount.Username = model.Username;
            userAccount.ModifiedBy = this.currentUserInfo.UserAccountId;
            userAccount.ModifiedDate = DateTime.Now;
            await this.unitOfWork.UserAccountRepository.UpdateAsync(userAccount);
            updated += await this.unitOfWork.CommitAsync();

            this.unitOfWork.CommitTransaction();
            return updated;
        }

        public async Task<int> UpdateProfileImageAsync(FileRequestModel model)
        {
            var profileImagePath = await this.fileHelper.SaveProfileImageAsync(model.Base64Image, model.FileName);
            if (string.IsNullOrEmpty(profileImagePath))
            {
                return 0;
            }

            var user = await this.unitOfWork.UserRepository.FindAsync(model.UserId);
            user.ThumbnailPath = profileImagePath;
            user.ModifiedBy = this.currentUserInfo.UserAccountId;
            user.ModifiedDate = DateTime.Now;
            await this.unitOfWork.UserRepository.UpdateAsync(user);
            return await this.unitOfWork.CommitAsync();
        }

        public async Task<int> ModifyStatusAsync(int userId, bool isActive)
        {
            this.unitOfWork.BeginTransaction();

            // User
            var user = await this.unitOfWork.UserRepository.FindAsync(userId);
            user.IsActive = isActive;
            user.ModifiedBy = this.currentUserInfo.UserAccountId;
            user.ModifiedDate = DateTime.Now;
            await this.unitOfWork.UserRepository.UpdateAsync(user);

            // User Account
            var userAccount = await this.unitOfWork.UserAccountRepository.FindAsync(userId);
            if (userAccount != null)
            {
                userAccount.IsActive = isActive;
                userAccount.ModifiedBy = this.currentUserInfo.UserAccountId;
                userAccount.ModifiedDate = DateTime.Now;
                await this.unitOfWork.UserAccountRepository.UpdateAsync(userAccount);

                // User Credential
                var userCredential = await this.unitOfWork.UserCredentialRepository.Table.OrderByDescending(m => m.UserAccountId).FirstOrDefaultAsync(m => m.UserAccountId == userAccount.UserAccountId);
                if (userCredential != null)
                {
                    userCredential.IsActive = isActive;
                    await this.unitOfWork.UserCredentialRepository.UpdateAsync(userCredential);
                }
            }

            var updated = await this.unitOfWork.CommitAsync();
            this.unitOfWork.CommitTransaction();
            return updated;
        }

        private async Task<string> GetLatestEmployeeIdAsync()
        {
            // Ex: CME22120001
            var employeeId = await this.unitOfWork.Connection.QueryFirstOrDefaultAsync<string>(@"SELECT ""EmployeeId"" FROM ""UserAccount"" ORDER BY ""UserAccountId"" DESC");
            return CoreHelper.GetYearMonthSequence(employeeId, "CME");
        }
    }
}