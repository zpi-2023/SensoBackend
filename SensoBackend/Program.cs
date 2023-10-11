using SensoBackend.Application;
using SensoBackend.Infrastructure;
using SensoBackend.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddWebApiLayer();

var app = builder.Build();

app.UseWebApiLayer();
app.AutoMigrateDatabase();

app.Run();
