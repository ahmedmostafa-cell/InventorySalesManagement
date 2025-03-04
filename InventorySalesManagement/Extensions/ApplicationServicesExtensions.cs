using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.BusinessLayer.Services;

namespace InventorySalesManagement.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(12);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        services.AddScoped<IAccountService, AccountService>();

        services.AddAutoMapper(typeof(Program).Assembly);

        return services;
    }

    public static IApplicationBuilder UseApplicationMiddleware(this IApplicationBuilder app)
    {
        app.UseSession();

        return app;
    }
}
