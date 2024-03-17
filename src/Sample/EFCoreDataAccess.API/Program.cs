using EFCoreDataAccess.API.Configurations;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi();
builder.Services.AddDatabase();

var app = builder.Build();

Api.ConfigureApi(app, app.Environment);
Database.ConfigureDatabase(app);

app.Run();