using Application.Commands;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.InventoryManagement;
using HomeInvManagementAPI.Interfaces.Repositories;
using HomeInvManagementAPI.Repositories;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InventoryManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace HomeInvManagementAPI.DI
{
    // Class for Dependency Injection registrations for projects that need to have certain aspects of the HomeInvManagementAPI injected. Example values from the appsettings.json.
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<HomeInvOptions> configureOptions)
        {
            var options = new HomeInvOptions();
            configureOptions(options);

            // Register the configured HomeInvOptions *directly*
            services.AddSingleton(options);

            services.AddHttpClient<IOpenFoodFactsRepository, OpenFoodFactsRepository>((sp, client) =>
            {
                var options = sp.GetRequiredService<HomeInvOptions>();
            });

            services.AddTransient<IProductCreateRepository, ProductCreateRepository>();
            services.AddTransient<IProductReadRepository, ProductReadRepositry>();

            return services;
        }
    }
}