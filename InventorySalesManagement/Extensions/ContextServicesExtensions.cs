using InventorySalesManagement.Core;
using InventorySalesManagement.RepositoryLayer.Interfaces;
using InventorySalesManagement.RepositoryLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace InventorySalesManagement.Extensions;

public static class ContextServicesExtensions
{
    public static IServiceCollection AddContextServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(config.GetConnectionString("url")));
        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }

}