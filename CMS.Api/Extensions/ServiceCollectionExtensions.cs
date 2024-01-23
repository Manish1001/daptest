namespace CMS.Api.Extensions
{
    using AutoMapper;
    using Filters;
    using Handlers;
    using Infrastructure.Container;
    using Microsoft.AspNetCore.Http.Features;
    using Middleware;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class ServiceCollectionExtensions
    {
        public static void AddAutoMapper(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(cfg => cfg.AddMaps(new[]
            {
                "CMS.Api", "CMS.Domain", "CMS.Infrastructure", "CMS.Shared"
            }));

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public static void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Basic")
                .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
        }

        public static void AddMvcApp(IServiceCollection services)
        {
            services
                .AddMvc(config =>
                {
                    config.Filters.Add(typeof(ExceptionFilter));
                    config.RespectBrowserAcceptHeader = false;
                    config.ReturnHttpNotAcceptable = true;
                    config.EnableEndpointRouting = false;
                })
                .AddNewtonsoftJson(a =>
                {
                    a.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    a.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    a.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                });

            services.Configure<FormOptions>(x => { x.ValueLengthLimit = int.MaxValue; x.MultipartBodyLengthLimit = int.MaxValue; });
            services.AddOptions();
        }

        public static void AddCors(IServiceCollection services)
        {
            services.AddCors(policyBuilder => policyBuilder.AddDefaultPolicy(policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader()));
        }

        public static void AddSwagger(IServiceCollection services)
        {
            SwaggerConfigMiddleware.ConfigureServices(services);
        }

        public static void AddFramework(IServiceCollection services, string connectionString)
        {
            ConfigureServiceContainer.AddDbContext(services, connectionString);
            ConfigureServiceContainer.AddHelpers(services);
            ConfigureServiceContainer.AddServices(services);
        }
    }
}
