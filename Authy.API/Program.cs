using Authy.Data.Interfaces;
using Authy.Common.Entities;
using Authy.Data.Repositories;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Authy.Domain.Interfaces;
using Authy.Domain.Services;
using ServiceStack;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Authy.API.Middleware;
using Authy.Common.Enums;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
var secretPhrase = builder.Configuration[EnvironmentVariableEnum.SECRET_PHRASE];
var jwtTokenName = builder.Configuration[EnvironmentVariableEnum.JWT_TOKEN_NAME];
var dbConnStr = builder.Configuration[EnvironmentVariableEnum.DATABASE_CONNECTION_STRING];

Environment.SetEnvironmentVariable(EnvironmentVariableEnum.SECRET_PHRASE, secretPhrase);
Environment.SetEnvironmentVariable(EnvironmentVariableEnum.JWT_TOKEN_NAME, jwtTokenName);
Environment.SetEnvironmentVariable(EnvironmentVariableEnum.DATABASE_CONNECTION_STRING, dbConnStr);
#endif

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://localhost:5001",
            ValidAudience = "https://localhost:3000",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable(EnvironmentVariableEnum.SECRET_PHRASE))),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    });

builder.Services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(dbConnStr, SqlServer2019Dialect.Provider));
builder.Services.AddSingleton<IAsyncRepository<User, long>, UserAuthRepository>();
builder.Services.AddSingleton<IRepository<User, long>, UserAuthRepositorySync>();
builder.Services.AddSingleton<IAsyncRepository<ApiKey, long>, ApiKeyRepository>();
builder.Services.AddSingleton<IRepository<ApiKey, long>, ApiKeyRepositorySync>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();
builder.Services.AddSingleton<IUserRoleService, UserRoleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JWTCookieMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
