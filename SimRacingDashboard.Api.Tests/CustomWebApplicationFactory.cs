using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using SimRacingDashboard.Api.Data;
using Microsoft.Extensions.DependencyInjection;

namespace SimRacingDashboard.Api.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all DbContext registrations
            var dbContextDescriptors = services
                .Where(d => d.ServiceType.Name.Contains("DbContext"))
                .ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<SimRacingDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
        });
    }
}
