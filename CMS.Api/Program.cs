using CMS.Api.Extensions;

using ServiceCollectionExtensions = CMS.Api.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DBConnection");
var baseUrl = builder.Configuration.GetValue<string>("BaseUrl");

// Service Collection
builder.Services.AddHttpContextAccessor();
ServiceCollectionExtensions.AddAutoMapper(builder.Services);
ServiceCollectionExtensions.AddAuthentication(builder.Services);
ServiceCollectionExtensions.AddCors(builder.Services);
ServiceCollectionExtensions.AddMvcApp(builder.Services);
ServiceCollectionExtensions.AddSwagger(builder.Services);
ServiceCollectionExtensions.AddFramework(builder.Services, connectionString);


// App Builder
var app = builder.Build();
AppBuilderExtensions.Register(app);
app.Run();