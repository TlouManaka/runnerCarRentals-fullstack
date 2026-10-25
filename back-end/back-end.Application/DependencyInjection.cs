using backend.Application.Common.Interfaces.Persistence;
using backend.Application.Common.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}
