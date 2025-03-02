using InventorySalesManagement.BusinessLayer.Interfaces;
using InventorySalesManagement.BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventorySalesManagement.Extensions;

public static class ApplicationServicesExtensions
{
    // interfaces sevices [IAccountService, IPhotoHandling,[ INotificationService, FcmNotificationSetting, FcmSender,ApnSender ], AddAutoMapper, hangfire  ]
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {


        // Session Service
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(12);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Application Service 
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
