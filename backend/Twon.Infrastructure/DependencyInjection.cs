using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Resend;
using StackExchange.Redis;
using Twon.Application.Admin.Repositories;
using Twon.Application.Auth.Repositories;
using Twon.Application.Catalog.Repositories;
using Twon.Application.Common.Interfaces;
using Twon.Application.Library.Repositories;
using Twon.Application.Payment.Repositories;
using Twon.Application.Store.Repositories;
using Twon.Infrastructure.Admin;
using Twon.Infrastructure.Auth;
using Twon.Infrastructure.Catalog;
using Twon.Infrastructure.Library;
using Twon.Infrastructure.Payment;
using Twon.Infrastructure.Persistence;
using Twon.Infrastructure.Services;
using Twon.Infrastructure.Store;

namespace Twon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // PostgreSQL via EF Core
        services.AddDbContext<TwonDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Postgres")));

        // MongoDB
        services.AddSingleton<MongoDbContext>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]!));

        // AWS S3 / Cloudflare R2
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var credentials = new BasicAWSCredentials(
                config["R2:AccessKeyId"], config["R2:SecretAccessKey"]);
            var s3Config = new AmazonS3Config
            {
                ServiceURL = config["R2:AccountUrl"],
                ForcePathStyle = true
            };
            return new AmazonS3Client(credentials, s3Config);
        });

        // Resend email
        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
            o.ApiToken = config["Resend:ApiKey"]!);
        services.AddTransient<IResend, ResendClient>();

        // Repository implementations
        services.AddScoped<IAuthRepository, AuthRepositoryImpl>();
        services.AddScoped<ICatalogRepository, CatalogRepositoryImpl>();
        services.AddScoped<ILibraryRepository, LibraryRepositoryImpl>();
        services.AddScoped<IStoreRepository, StoreRepositoryImpl>();
        services.AddScoped<IPaymentRepository, PaymentRepositoryImpl>();
        services.AddScoped<IAdminRepository, AdminRepositoryImpl>();

        // Infrastructure services
        services.AddScoped<ICacheService, RedisService>();
        services.AddScoped<IStorageService, R2StorageService>();
        services.AddScoped<IEmailService, ResendEmailService>();

        return services;
    }
}
