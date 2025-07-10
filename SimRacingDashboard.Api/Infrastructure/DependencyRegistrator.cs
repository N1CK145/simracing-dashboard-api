using System;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimRacingDashboard.Api.Dtos.Auth;
using SimRacingDashboard.Api.Services;
using SimRacingDashboard.Api.Validator.Auth;

namespace SimRacingDashboard.Api.Infrastructure;

public static class DependencyRegistrator
{
    private static (string key, string iv) GetEncryptionKeyAndIV(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var env = provider.GetService<IHostEnvironment>();

        var encryptionKey = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
        var encryptionIV = Environment.GetEnvironmentVariable("ENCRYPTION_IV");

        // Use default values in Development if not set
        if (env != null && env.IsDevelopment())
        {
            if (string.IsNullOrWhiteSpace(encryptionKey) || string.IsNullOrWhiteSpace(encryptionIV))
            {
                // These are for development only! Replace with secure values in production.
                encryptionKey = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="; // 32 bytes base64
                encryptionIV = "AAAAAAAAAAAAAAAAAAAAAA=="; // 16 bytes base64
            }
        }

        if (string.IsNullOrWhiteSpace(encryptionKey) || string.IsNullOrWhiteSpace(encryptionIV))
            throw new InvalidOperationException("Encryption key and IV must be set in environment variables.");

        return (encryptionKey, encryptionIV);
    }

    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        var (encryptionKey, encryptionIV) = GetEncryptionKeyAndIV(services);

        services.AddSingleton(new EncryptionService(encryptionKey, encryptionIV));
        services.AddScoped<AuthService>();

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequest>();

        return services;
    }
}
