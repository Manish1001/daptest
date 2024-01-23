namespace CMS.Api.ApiControllers
{
    using System;
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Shared.FilterModels;
    using Shared.Handlers;
    using Shared.Library;
    using Shared.Models;
    using Utilities;

    [Route("api/user-accounts")]
    public class UserAccountController : BaseApiController
    {
        private readonly IAuditLogService auditLogServices;
        private readonly IUserAccountService userAccountServices;
        private readonly IUserTokenService userTokenServices;

        public UserAccountController(IAuditLogService auditLogServices, IUserAccountService userAccountServices, IUserTokenService userTokenServices)
        {
            this.auditLogServices = auditLogServices;
            this.userAccountServices = userAccountServices;
            this.userTokenServices = userTokenServices;
        }

        #region Self Annynomus

        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate")]
        public async Task<ApiResponse> AuthenticateAsync([FromBody] LoginRequestModel request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return this.BadRequest("Please enter username and password.");
            }

            var response = await this.userAccountServices.ValidateAsync(request.Username, request.Password);
            if (response == null)
            {
                return this.Failed();
            }

            var status = response.Item1;
            var account = response.Item2;

            if (status != AccountStatus.Success)
            {
                switch (status)
                {
                    case AccountStatus.InvalidUsername:
                    case AccountStatus.InvalidPassword:
                        return this.BadRequest("Sorry! We don't have a user in the system with that credentials. Please try again with correct credentials.");
                    case AccountStatus.PasswordExpired:
                        return this.BadRequest("Oh no! Your password has been expired. Please contact administrator to reset your password.");
                    case AccountStatus.InactiveUser:
                        return this.BadRequest("Oh no! You're in inactive status. Please try again.");
                    case AccountStatus.LockedUser:
                        return this.BadRequest("Oh no! Your account has been locked for 5 failed login attempts. Please contact an administrator.");
                    default:
                        return this.Failed();
                }
            }

            if (account == null || account.UserAccountId == 0)
            {
                return this.Failed();
            }

            account.LoggedInDate = DateTime.Now;

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Login",
                LogDescription = "Has been logged in.",
                TableName = "UserAccount",
                ReferenceId = account.UserAccountId,
                UserAccountId = account.UserAccountId,
                RoleId = account.RoleId
            });

            return this.Success(account);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh-token")]
        public async Task<ApiResponse> RefreshTokenAsync([FromBody] RefreshTokenRequestModel request)
        {
            var userToken = await this.userTokenServices.FindAsync(request.ReferenceToken, request.AccessToken);
            if (userToken.UserTokenId <= 0)
            {
                return this.Failed($"No user token found, Invalid request params. Please try again.");
            }

            var token = Guid.NewGuid().ToString();
            var response = await this.userTokenServices.UpdateAsync(userToken.UserTokenId, token);
            if (response <= 0)
            {
                return this.Failed($"Failed to refresh the token. Please try again.");
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Refresh Token",
                LogDescription = "Has refreshed the token.",
                TableName = "UserToken",
                ReferenceId = userToken.UserTokenId,
                UserAccountId = userToken.UserAccountId,
                RoleId = userToken.RoleId
            });

            return this.Success(null, token);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<ApiResponse> ForgotPasswordAsync([FromBody] ForgotPasswordRequestModel request)
        {
            var userAccount = await this.userAccountServices.FindAsync(request.Username, request.Email);
            if (userAccount.UserAccountId <= 0)
            {
                return this.Failed($"Failed to perform action on this user account. Please try again.");
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Forgot Password",
                LogDescription = "Has requested for forgot password.",
                TableName = "UserAccount",
                ReferenceId = userAccount.UserAccountId,
                UserAccountId = userAccount.UserAccountId,
                RoleId = userAccount.RoleId
            });

            var cipherUserAccountId = CipherHandler.Encrypt(userAccount.UserAccountId.ToString());
            return this.Success(null, string.IsNullOrEmpty(cipherUserAccountId) ? userAccount.UserAccountId.ToString() : cipherUserAccountId);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("get-account-info")]
        public async Task<ApiResponse> GetAccountInfoAsync([FromBody] AccountInfoRequestModel request)
        {
            var userAccountId = request.UserAccountId;
            if (!string.IsNullOrEmpty(request.CipherUserAccountId))
            {
                var decryptedId = CipherHandler.Decrypt(request.CipherUserAccountId);
                if (decryptedId != null)
                {
                    userAccountId = Convert.ToInt32(decryptedId);
                }
            }

            var accountInfo = await this.userAccountServices.GetInfoAsync(userAccountId);
            return this.Success(accountInfo);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-reset-password")]
        public async Task<ApiResponse> ForgotResetPasswordAsync([FromBody] ResetPasswordRequestModel request)
        {
            var roleId = await this.userAccountServices.ResetPasswordAsync(request.UserAccountId, request.Password, false);
            if (roleId == 0)
            {
                return this.Failed();
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Reset Password",
                LogDescription = "Has reset account's password using forgot password.",
                TableName = "UserAccount",
                ReferenceId = request.UserAccountId,
                UserAccountId = request.UserAccountId,
                RoleId = roleId
            });
            return this.Success();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("logout")]
        public async Task<ApiResponse> LogoutAsync([FromBody] AccountInfoRequestModel request)
        {
            await this.HttpContext.SignOutAsync();

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Logout",
                LogDescription = "Has been signed out.",
                TableName = "UserAccount",
                ReferenceId = request.UserAccountId
            });

            return this.Success();
        }

        #endregion

        #region Self Authentication

        [HttpPost]
        [Route("change-password")]
        public async Task<ApiResponse> ChangePasswordAsync([FromBody] ResetPasswordRequestModel request)
        {
            var roleId = await this.userAccountServices.ChangePasswordAsync(request.Password);
            if (roleId == 0)
            {
                return this.Failed();
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Reset Password",
                LogDescription = "Has changed account's password.",
                TableName = "UserAccount",
                ReferenceId = request.UserAccountId,
                UserAccountId = request.UserAccountId,
                RoleId = roleId
            });
            return this.Success();
        }

        [HttpPost]
        [Route("force-reset-password")]
        public async Task<ApiResponse> ForceResetPasswordAsync([FromBody] ResetPasswordRequestModel request)
        {
            var roleId = await this.userAccountServices.ChangePasswordAsync(request.Password, true);
            if (roleId == 0)
            {
                return this.Failed();
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Reset Password",
                LogDescription = "Has forcefully reset account's password.",
                TableName = "UserAccount",
                ReferenceId = request.UserAccountId,
                UserAccountId = request.UserAccountId,
                RoleId = roleId
            });
            return this.Success();
        }

        #endregion

        #region User Accounts

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync([FromBody] UserAccountFilterModel model)
        {
            var userAccounts = await this.userAccountServices.FetchAsync(model);
            return this.Success(userAccounts);
        }

        [HttpPost]
        [Route("modify-status")]
        public async Task<ApiResponse> ModifyStatusAsync([FromBody] UserAccountModel model)
        {
            var response = await this.userAccountServices.ModifyStatusAsync(model.UserAccountId, model.IsActive);
            if (response == 0)
            {
                this.BadRequest($"Failed to {(model.IsActive ? "enable" : "disable")} user account. Please try again.");
            }

            // Audit Log
            var description = $"Successfully {(model.IsActive ? "enabled" : "disabled")} the user account.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Modify User Account's Active Status",
                LogDescription = description,
                TableName = "UserAccount",
                ReferenceId = model.UserAccountId
            });
            return this.Success(description);
        }

        [HttpPost]
        [Route("mark-force-reset-password")]
        public async Task<ApiResponse> MarkForceResetPasswordAsync([FromBody] UserAccountModel model)
        {
            var response = await this.userAccountServices.ForceResetPasswordAsync(model.UserAccountId);
            if (response == 0)
            {
                this.BadRequest("Failed to mark the user account's password for reset. Please try again.");
            }

            const string Description = "The user account's password was successfully marked for reset.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Force Reset Password",
                LogDescription = Description,
                TableName = "UserAccount",
                ReferenceId = model.UserAccountId
            });
            return this.Success(Description);
        }

        [HttpPost]
        [Route("modify-lock-status")]
        public async Task<ApiResponse> ModifyLockStatusAsync([FromBody] UserAccountModel model)
        {
            var response = await this.userAccountServices.ModifyLockStatusAsync(model.UserAccountId, model.IsLocked);
            if (response == 0)
            {
                this.BadRequest($"Failed to {(model.IsActive ? "lock" : "unlock")} user account. Please try again.");
            }

            // Audit Log
            var description = $"Successfully {(model.IsActive ? "locked" : "unlocked")} the user account.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Modify User Account's Lock Status",
                LogDescription = description,
                TableName = "UserAccount",
                ReferenceId = model.UserAccountId
            });
            return this.Success(description);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<ApiResponse> ResetPasswordAsync([FromBody] ResetPasswordRequestModel model)
        {
            var roleId = await this.userAccountServices.ResetPasswordAsync(model.UserAccountId, model.Password, model.IsSystemGeneratedPassword);
            if (roleId == 0)
            {
                return this.Failed();
            }

            // Audit Log
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Reset Password",
                LogDescription = "Has reset account's password.",
                TableName = "UserAccount",
                ReferenceId = model.UserAccountId
            });
            return this.Success();
        }

        #endregion
    }
}
