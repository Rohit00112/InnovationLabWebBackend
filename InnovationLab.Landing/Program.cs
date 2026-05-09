using InnovationLab.Landing.DbContexts;
using InnovationLab.Landing.Services;
using InnovationLab.Landing.SwaggerFilters;
using InnovationLab.Shared.Constants;
using InnovationLab.Shared.Extensions;
using InnovationLab.Shared.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDefaultConfiguration(builder.Environment);

builder.Services.AddOptionsConfigurations(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configure Swagger to handle file uploads in form data
    options.OperationFilter<FileUploadOperationFilter>();
    options.SchemaFilter<FormDataSchemaFilter>();
});

builder.Services.AddDbContext<LandingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(ConfigurationKeys.DbConnection))
);

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddCloudinary(builder.Configuration);

// Register Dependency Injections
builder.Services.AddSharedServices();
builder.Services.AddSingleton<IEventRegistrationNotificationService, EventRegistrationNotificationService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<LandingDbContext>();
db.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
