namespace CMS.Api.ApiControllers
{
    using System.Threading.Tasks;
    using AutoWrapper.Wrappers;
    using Domain.Services;
    using Microsoft.AspNetCore.Mvc;
    using Utilities;

    [Route("api/lookup-types")]
    public class LookupTypesController : BaseApiController
    {
        private readonly ILookupTypeService lookupTypeServices;

        public LookupTypesController(ILookupTypeService lookupTypeServices) => this.lookupTypeServices = lookupTypeServices;

        [HttpPost]
        [Route("fetch")]
        public async Task<ApiResponse> FetchAsync()
        {
            var lookupTypes = await this.lookupTypeServices.FetchAsync();
            return this.Success(lookupTypes);
        }
    }
}
