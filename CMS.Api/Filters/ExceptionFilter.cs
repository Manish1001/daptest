namespace CMS.Api.Filters
{
    using System.Net;
    using AutoWrapper.Wrappers;
    using Listeners;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    public class ExceptionFilter : IExceptionFilter
    {
        private readonly DailyTraceListener traceListener;

        private readonly IConfiguration configuration;

        public ExceptionFilter(IConfiguration configuration)
        {
            this.traceListener = new DailyTraceListener(configuration);
            this.configuration = configuration;
        }

        public void OnException(ExceptionContext context)
        {
            var devMode = this.configuration.GetValue<bool>("ApplicationConfiguration:DevMode");
            var logFrom = context.ActionDescriptor.DisplayName;
            var logRoute = context.ActionDescriptor.AttributeRouteInfo?.Template;
            var errorMessage = context.Exception.InnerException != null ? context.Exception.InnerException.Message : context.Exception.Message;
            var errorDescription = JsonConvert.SerializeObject(context.Exception.StackTrace);
            this.traceListener.Invoke(logFrom, logRoute, errorMessage, errorDescription);

            context.ExceptionHandled = true;
            object error = devMode ? errorMessage : "Sorry error occurred. Please try again.";

            if (context.Exception.Message.Contains("AutoWrapper.Wrappers.ApiException"))
            {
                var apiExceptionError = (ApiException)context.Exception;
                if (apiExceptionError.IsCustomErrorObject)
                {
                    error = apiExceptionError.CustomError;
                }
            }

            var response = context.HttpContext.Response;
            response.StatusCode = !errorMessage.Contains("The DELETE statement conflicted with the REFERENCE constraint")
                                    ? (int)HttpStatusCode.InternalServerError
                                    : (int)HttpStatusCode.FailedDependency;
            response.ContentType = "application/json";
            context.Result = new ObjectResult(error);
        }
    }
}
