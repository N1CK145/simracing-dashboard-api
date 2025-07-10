using Microsoft.EntityFrameworkCore;
using SimRacingDashboard.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SimRacingDashboard.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddSwaggerWithBearerAuth()
    .AddCustomControllers()
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddProjectServices()
    .AddValidators();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sim-Racing Dashboard v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.Run();

public partial class Program { }