namespace CMS.Infrastructure.Container
{
    using CMS.Domain.Context;
    using Domain;
    using Domain.Helpers;
    using Domain.Services;
    using Helpers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public static class ConfigureServiceContainer
    {
        public static void AddDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddHelpers(IServiceCollection services)
        {
            services.AddScoped<ICurrentUserInfo, CurrentUserInfo>();
            services.AddSingleton<ICookieHelper, CookieHelper>();
            services.AddSingleton<IFileHelper, FileHelper>();
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddTransient<IUserTokenService, UserTokenServices>();
            services.AddTransient<IAuditLogService, AuditLogServices>();
            services.AddTransient<IUserAccountService, UserAccountServices>();
            services.AddTransient<IRoleService, RoleServices>();
            services.AddTransient<IUserService, UserServices>();
            services.AddTransient<ILookupService, LookupServices>();
            services.AddTransient<ILookupTypeService, LookupTypeServices>();
        }
    }
}
