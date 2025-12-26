using Microsoft.Extensions.DependencyInjection;
using ServiceOrders.Application.Abstractions;
using ServiceOrders.Application.Services;

namespace ServiceOrders.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOrderApplicationService, OrderApplicationService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        return services;
    }
}
