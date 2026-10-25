using backend.Application.Common.Interfaces.Authentication;
using backend.Domain.Entities;
using backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!await dbContext.Users.AnyAsync(u => u.UserRole == User.AdminRole, cancellationToken))
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            dbContext.Users.Add(new User
            {
                FullName = "System Admin",
                Email = (configuration["SeedAdmin:Email"] ?? "admin@runnercars.local").ToLowerInvariant(),
                PasswordHash = passwordHasher.Hash(configuration["SeedAdmin:Password"] ?? "Admin123!"),
                UserRole = User.AdminRole
            });
        }

        if (!await dbContext.Cars.AnyAsync(cancellationToken))
        {
            dbContext.Cars.AddRange(BuildSeedCars());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Car> BuildSeedCars()
    {
        return new List<Car>
        {
            new()
            {
                Make = "Toyota",
                Model = "Corolla Quest",
                Year = 2023,
                LicensePlate = "JHB 101 GP",
                DailyRate = 450m,
                Seats = 5,
                Transmission = TransmissionType.Manual,
                FuelType = FuelType.Petrol,
                ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?auto=format&fit=crop&w=1200&q=80",
                Description = "Reliable and fuel-efficient sedan, perfect for city driving and business trips."
            },
            new()
            {
                Make = "Volkswagen",
                Model = "Polo Vivo",
                Year = 2022,
                LicensePlate = "JHB 102 GP",
                DailyRate = 400m,
                Seats = 5,
                Transmission = TransmissionType.Manual,
                FuelType = FuelType.Petrol,
                ImageUrl = "https://images.unsplash.com/photo-1541899481282-d53bffe3c35d?auto=format&fit=crop&w=1200&q=80",
                Description = "Compact hatchback with low running costs and easy parking."
            },
            new()
            {
                Make = "BMW",
                Model = "3 Series",
                Year = 2023,
                LicensePlate = "JHB 103 GP",
                DailyRate = 950m,
                Seats = 5,
                Transmission = TransmissionType.Automatic,
                FuelType = FuelType.Petrol,
                ImageUrl = "https://images.unsplash.com/photo-1555215695-3004980ad54e?auto=format&fit=crop&w=1200&q=80",
                Description = "Executive sedan with a premium interior and responsive handling."
            },
            new()
            {
                Make = "Mercedes-Benz",
                Model = "C-Class",
                Year = 2024,
                LicensePlate = "JHB 104 GP",
                DailyRate = 1200m,
                Seats = 5,
                Transmission = TransmissionType.Automatic,
                FuelType = FuelType.Petrol,
                ImageUrl = "https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?auto=format&fit=crop&w=1200&q=80",
                Description = "Luxury sedan for weddings, business travel and special occasions."
            },
            new()
            {
                Make = "Toyota",
                Model = "Fortuner",
                Year = 2022,
                LicensePlate = "JHB 105 GP",
                DailyRate = 1100m,
                Seats = 7,
                Transmission = TransmissionType.Automatic,
                FuelType = FuelType.Diesel,
                ImageUrl = "https://images.unsplash.com/photo-1533473359331-0135ef1b58bf?auto=format&fit=crop&w=1200&q=80",
                Description = "Seven-seat SUV built for family holidays and long-distance travel."
            },
            new()
            {
                Make = "Hyundai",
                Model = "i20",
                Year = 2023,
                LicensePlate = "JHB 106 GP",
                DailyRate = 420m,
                Seats = 5,
                Transmission = TransmissionType.Manual,
                FuelType = FuelType.Petrol,
                ImageUrl = "https://images.unsplash.com/photo-1550355291-bbee04a92027?auto=format&fit=crop&w=1200&q=80",
                Description = "Modern, light hatchback with excellent fuel economy."
            },
            new()
            {
                Make = "Ford",
                Model = "Ranger",
                Year = 2023,
                LicensePlate = "JHB 107 GP",
                DailyRate = 1050m,
                Seats = 5,
                Transmission = TransmissionType.Automatic,
                FuelType = FuelType.Diesel,
                ImageUrl = "https://images.unsplash.com/photo-1551830820-330a71b99659?auto=format&fit=crop&w=1200&q=80",
                Description = "Double-cab bakkie for heavy loads, work sites and outdoor trips."
            },
            new()
            {
                Make = "Tesla",
                Model = "Model 3",
                Year = 2024,
                LicensePlate = "JHB 108 GP",
                DailyRate = 1400m,
                Seats = 5,
                Transmission = TransmissionType.Automatic,
                FuelType = FuelType.Electric,
                ImageUrl = "https://images.unsplash.com/photo-1560958089-b8a1929cea89?auto=format&fit=crop&w=1200&q=80",
                Description = "Fully electric sedan with instant acceleration and zero fuel costs."
            }
        };
    }
}
