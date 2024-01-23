namespace CMS.Api.Middleware
{
    using System.Linq;
    using System.Reflection;
    using Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerUI;

    public static class SwaggerConfigMiddleware
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.ToString());
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "CMS Rest API"
                    });

                // options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "CMS.Api.xml"));
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.DescribeAllParametersInCamelCase();
                options.UseInlineDefinitionsForEnums();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.ApiKey,
                            Description = "CMS Rest API Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
                        });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            });
        }

        public static void Configure(WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.IndexStream = () => Assembly.GetExecutingAssembly().GetManifestResourceStream("CMS.Api.Swagger.index.html");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS Rest API v1 Documentation");
                c.DefaultModelsExpandDepth(0);
                c.DefaultModelRendering(ModelRendering.Example);
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.List);
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.DocumentTitle = "CMS Rest API";
                c.RoutePrefix = "api-docs";
                c.ShowExtensions();
                c.SupportedSubmitMethods(SubmitMethod.Post, SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Delete);
            });
        }
    }
}
