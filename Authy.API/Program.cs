using Authy.Data.Interfaces;
using Authy.Data.Models;
using Authy.Data.Repositories;
using ServiceStack.Data;
using ServiceStack.OrmLite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authyDbConnStr = builder.Configuration.GetConnectionString("Authy");

builder.Services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(authyDbConnStr, SqlServer2019Dialect.Provider));
builder.Services.AddSingleton<IAsyncRepository<User, long>, UserAuthRepository>();
builder.Services.AddSingleton<IAsyncRepository<ApiKey, string>, ApiKeyRepository>();

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
