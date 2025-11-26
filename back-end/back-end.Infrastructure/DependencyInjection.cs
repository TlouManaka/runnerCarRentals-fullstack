using backend.Application.Common.Interfaces.Authentication;
using backend.Application.Common.Interfaces.Persistence;
using backend.Application.Common.Interfaces.Services;
using backend.Infrastructure.Authentication;
using backend.Infrastructure.Services;
using backend.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
namespace backend.Infrastructure;




public static class DependencInjection
{
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Microsoft.Extensions.Configuration.ConfigurationManager configuration)
    {
        
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IUserRepository, UserRepository>();   

        return services;

    }

}