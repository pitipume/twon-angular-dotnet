using Microsoft.Extensions.DependencyInjection;
using Twon.Application.Auth.Managers;
using Twon.Application.Auth.Services;
using Twon.Application.Auth.Repositories;
using Twon.Application.Catalog.Managers;
using Twon.Application.Catalog.Services;
using Twon.Application.Catalog.Repositories;
using Twon.Application.Library.Managers;
using Twon.Application.Library.Services;
using Twon.Application.Library.Repositories;
using Twon.Application.Store.Managers;
using Twon.Application.Store.Services;
using Twon.Application.Store.Repositories;
using Twon.Application.Payment.Managers;
using Twon.Application.Payment.Services;
using Twon.Application.Payment.Repositories;
using Twon.Application.Admin.Managers;
using Twon.Application.Admin.Services;
using Twon.Application.Admin.Repositories;

namespace Twon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Auth
        services.AddScoped<AuthManager>();
        services.AddScoped<AuthService>();
        services.AddScoped<AuthRepository>();

        // Catalog
        services.AddScoped<CatalogManager>();
        services.AddScoped<CatalogService>();
        services.AddScoped<CatalogRepository>();

        // Library
        services.AddScoped<LibraryManager>();
        services.AddScoped<LibraryService>();
        services.AddScoped<LibraryRepository>();

        // Store
        services.AddScoped<StoreManager>();
        services.AddScoped<StoreService>();
        services.AddScoped<StoreRepository>();

        // Payment
        services.AddScoped<PaymentManager>();
        services.AddScoped<PaymentService>();
        services.AddScoped<PaymentRepository>();

        // Admin
        services.AddScoped<AdminManager>();
        services.AddScoped<AdminService>();
        services.AddScoped<AdminRepository>();

        return services;
    }
}
