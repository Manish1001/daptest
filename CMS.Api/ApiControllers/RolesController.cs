namespace CMS.Api.ApiControllers
{
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Utilities;

    [Route("api/roles")]
    public class RolesController : BaseApiController
    {
        private readonly IRoleService roleServices;

        public RolesController(IRoleService roleServices) => this.roleServices = roleServices;

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync()
        {
            var roles = await this.roleServices.FetchAsync();
            return this.Success(roles);
        }
    }
}
