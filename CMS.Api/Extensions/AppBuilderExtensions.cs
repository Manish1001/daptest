namespace CMS.Api.Extensions
{
    using System.Reflection;
    using AutoWrapper;
    using Contracts;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.FileProviders;
    using Middleware;

    public static class AppBuilderExtensions
    {
        public static void Register(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Files"), "ProfileImages")),
                RequestPath = new PathString("/profile-images")
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseApiResponseAndExceptionWrapper<MapResponseObject>(new AutoWrapperOptions
            {
                ShowApiVersion = false,
                IgnoreNullValue = true,
                IsApiOnly = false,
                WrapWhenApiPathStartsWith = "/api",
                ExcludePaths = new[]
                {
                    new AutoWrapperExcludePath("/identity/.*|/identity", ExcludeMode.Regex),
                    new AutoWrapperExcludePath("/resources/.*|/resources", ExcludeMode.Regex)
                },
                ShowStatusCode = true
            });

            SwaggerConfigMiddleware.Configure(app);

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
        }
    }
}
