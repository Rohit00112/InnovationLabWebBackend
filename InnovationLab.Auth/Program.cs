using InnovationLab.Auth.DbContexts;
using InnovationLab.Auth.Interfaces;
using InnovationLab.Auth.Middlewares;
using InnovationLab.Auth.Models;
using InnovationLab.Auth.Services;
using InnovationLab.Shared.Constants;
using InnovationLab.Shared.Extensions;
using InnovationLab.Shared.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDefaultConfiguration(builder.Environment);

builder.Services.AddOptionsConfigurations(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(ConfigurationKeys.DbConnection))
);

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddCloudinary(builder.Configuration);

builder.Services.AddSlidingWindowRateLimiter();

// Register Dependency Injections
builder.Services.AddSharedServices();
builder.Services.AddScoped<ITokenService, TokenService>();


var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
db.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseMiddleware<SecurityStampValidatorMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
