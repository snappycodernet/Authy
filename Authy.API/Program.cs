using Authy.Data.Interfaces;
using Authy.Common.Entities;
using Authy.Data.Repositories;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Authy.Domain.Interfaces;
using Authy.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authyDbConnStr = builder.Configuration.GetConnectionString("Authy");

var secretPhrase = builder.Configuration["secretPhrase"];

Environment.SetEnvironmentVariable("secretPhrase", secretPhrase);

builder.Services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(authyDbConnStr, SqlServer2019Dialect.Provider));
builder.Services.AddSingleton<IAsyncRepository<User, long>, UserAuthRepository>();
builder.Services.AddSingleton<IAsyncRepository<ApiKey, string>, ApiKeyRepository>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
