namespace CMS.Api.ApiControllers
{
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Shared.FilterModels;
    using Shared.Models;
    using Utilities;

    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        private readonly IAuditLogService auditLogServices;
        private readonly IUserService userServices;

        public UsersController(IUserService userServices, IAuditLogService auditLogServices)
        {
            this.userServices = userServices;
            this.auditLogServices = auditLogServices;
        }

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync([FromBody] UserFilterModel model)
        {
            var users = await this.userServices.FetchAsync(model);
            return this.Success(users);
        }

        [HttpPost]
        [Route("find")]
        public async Task<ApiResponse> FindAsync([FromBody] UserFilterModel model)
        {
            var user = await this.userServices.FindAsync(model.UserId);

            // Audit Log
            const string Description = "Successfully fetched user details.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Find User",
                LogDescription = Description,
                TableName = "User",
                ReferenceId = model.UserId
            });

            return this.Success(user);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ApiResponse> AddAsync([FromBody] UserModel model)
        {
            var response = await this.userServices.AddAsync(model);
            switch (response)
            {
                case 0:
                    return this.BadRequest("Failed to add user. Please try again.");
                case -1:
                    return this.Conflict("Conflict! User already exist with the same 'Email'.");
                case -2:
                    return this.Conflict("Conflict! User already exist with the same 'Mobile Number'.");
                case -3:
                    return this.Conflict("Conflict! User already exist with the same 'Username'.");
            }

            // Audit Log
            const string Description = "Successfully added the user.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Add User",
                LogDescription = Description,
                TableName = "User",
                ReferenceId = response
            });

            return this.Success(Description);
        }

        [HttpPost]
        [Route("modify")]
        public async Task<ApiResponse> ModifyAsync([FromBody] UserModel model)
        {
            var response = await this.userServices.UpdateAsync(model);
            switch (response)
            {
                case 0:
                    return this.BadRequest("Failed to modify user. Please try again.");
                case -1:
                    return this.Conflict("Conflict! User already exist with the same 'Email'.");
                case -2:
                    return this.Conflict("Conflict! User already exist with the same 'Mobile Number'.");
                case -3:
                    return this.Conflict("Conflict! User already exist with the same 'Username'.");
            }

            // Audit Log
            const string Description = "Successfully updated the user.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Update User",
                LogDescription = Description,
                TableName = "User",
                ReferenceId = model.UserId
            });

            return this.Success(Description);
        }

        [HttpPost]
        [Route("modify-status")]
        public async Task<ApiResponse> ModifyStatusAsync([FromBody] UserModel model)
        {
            var response = await this.userServices.ModifyStatusAsync(model.UserId, model.IsActive);
            if (response == 0)
            {
                this.BadRequest($"Failed to {(model.IsActive ? "enable" : "delete")} user. Please try again.");
            }

            // Audit Log
            var description = $"Successfully {(model.IsActive ? "enabled" : "deleted")} the user.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = model.IsActive ? "Enable User" : "Delete User",
                LogDescription = description,
                TableName = "User",
                ReferenceId = model.UserId
            });

            return this.Success(description);
        }

        [HttpPost]
        [Route("modify-profile-image")]
        public async Task<ApiResponse> ModifyProfileImageAsync([FromBody] FileRequestModel model)
        {
            var response = await this.userServices.UpdateProfileImageAsync(model);
            if (response == 0)
            {
                this.BadRequest($"Failed to upload the user's profile image. Please try again.");
            }

            // Audit Log
            const string Description = $"Successfully updated the user's profile image.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Update User's Profile Image",
                LogDescription = Description,
                TableName = "User",
                ReferenceId = model.UserId
            });

            return this.Success(Description);
        }
    }
}
