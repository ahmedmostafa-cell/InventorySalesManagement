﻿using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Helpers;
using InventorySalesManagement.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace InventorySalesManagement.Extensions;

public static class IdentityServicesExtensions
{

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

		// Identity service
		services.AddIdentity<ApplicationUser, IdentityRole>(options =>
		{
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireDigit = false;
			options.Password.RequireLowercase = false;
			options.Password.RequiredLength = 6;
			options.User.RequireUniqueEmail = true;
		})
   .AddEntityFrameworkStores<ApplicationContext>();

		services.Configure<Jwt>(config.GetSection("JWT"));

        services.AddAuthentication(options =>
        {
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = config["JWT:Issuer"],
                ValidAudience = config["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
            };
        });

        return services;

    }
}
