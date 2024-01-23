namespace CMS.Api.ApiControllers
{
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Shared.FilterModels;
    using Shared.Models;
    using Utilities;

    [Route("api/lookups")]
    public class LookupsController : BaseApiController
    {
        private readonly IAuditLogService auditLogServices;
        private readonly ILookupService lookupServices;

        public LookupsController(ILookupService lookupServices, IAuditLogService auditLogServices)
        {
            this.lookupServices = lookupServices;
            this.auditLogServices = auditLogServices;
        }

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync([FromBody] LookupFilterModel model)
        {
            var lookups = await this.lookupServices.FetchAsync(model);
            return this.Success(lookups);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ApiResponse> AddAsync([FromBody] LookupModel model)
        {
            var response = await this.lookupServices.AddAsync(model);
            switch (response)
            {
                case 0:
                    return this.BadRequest("Failed to add lookup. Please try again.");
                case -1:
                    return this.Conflict("Conflict! Lookup already exist with the same 'Lookup Type' and 'Lookup Value'.");
            }

            // Audit Log
            const string Description = "Successfully added the lookup.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Add Lookup",
                LogDescription = Description,
                TableName = "Lookup",
                ReferenceId = response
            });

            return this.Success(Description);
        }

        [HttpPost]
        [Route("modify")]
        public async Task<ApiResponse> ModifyAsync([FromBody] LookupModel model)
        {
            var response = await this.lookupServices.UpdateAsync(model);
            switch (response)
            {
                case 0:
                    return this.BadRequest("Failed to modify lookup. Please try again.");
                case -1:
                    return this.Conflict("Conflict! Lookup already exist with the same 'Lookup Type' and 'Lookup Value'.");
            }

            // Audit Log
            const string Description = "Successfully updated the lookup.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = "Update Lookup",
                LogDescription = Description,
                TableName = "Lookup",
                ReferenceId = model.LookupId
            });

            return this.Success(Description);
        }

        [HttpPost]
        [Route("modify-status")]
        public async Task<ApiResponse> ModifyStatusAsync([FromBody] LookupModel model)
        {
            var response = await this.lookupServices.ModifyStatusAsync(model.LookupId, model.IsActive);
            if (response == 0)
            {
                this.BadRequest($"Failed to {(model.IsActive ? "enable" : "delete")} lookup. Please try again.");
            }

            // Audit Log
            var description = $"Successfully {(model.IsActive ? "enabled" : "deleted")} the lookup.";
            await this.auditLogServices.AddAsync(new AuditLogEntryModel
            {
                LogType = model.IsActive ? "Enable Lookup" : "Delete Lookup",
                LogDescription = description,
                TableName = "Lookup",
                ReferenceId = model.LookupId
            });

            return this.Success(description);
        }
    }
}
