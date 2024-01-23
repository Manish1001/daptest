namespace CMS.Api.Utilities
{
    using AutoWrapper.Wrappers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    [Authorize]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class BaseApiController : ControllerBase
    {
        private const string DefaultErrorMessage = "Sorry error occurred. Please try again.";

        protected static string Serialize(object value) =>
            JsonConvert.SerializeObject(
                value,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

        protected ApiResponse Success() => new ApiResponse("Successful");

        protected ApiResponse Success(object model) => new ApiResponse(model);

        protected ApiResponse Success(string message, object model = null) => new ApiResponse(message, model);

        protected ApiResponse BadRequest(string message = DefaultErrorMessage) => new ApiResponse(400, message);

        protected ApiResponse Forbidden(string message = DefaultErrorMessage) => new ApiResponse(403, message);

        protected ApiResponse Conflict(string message = DefaultErrorMessage) => new ApiResponse(409, message);

        protected ApiResponse Failed(string message = DefaultErrorMessage) => new ApiResponse(500, message);

        protected ApiException Error(Exception ex, string message = DefaultErrorMessage)
        {
            return new ApiException(new { ErrorMessage = message, ExceptionMessage = ex.Message });
        }
    }
}
