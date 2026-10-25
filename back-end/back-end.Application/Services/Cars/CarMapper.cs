using backend.Contracts;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application.Services.Cars;

internal static class CarMapper
{
    public static CarResponse ToResponse(this Car car) => new(
        car.Id,
        car.Make,
        car.Model,
        car.Year,
        car.LicensePlate,
        car.DailyRate,
        car.Seats,
        car.Transmission.ToString(),
        car.FuelType.ToString(),
        car.ImageUrl,
        car.Description,
        car.IsAvailable);

    public static TransmissionType ParseTransmission(this string value)
    {
        if (Enum.TryParse<TransmissionType>(value, true, out var transmission))
        {
            return transmission;
        }

        throw new backend.Application.Common.Errors.BadRequestException(
            $"Transmission must be one of: {string.Join(", ", Enum.GetNames<TransmissionType>())}.");
    }

    public static FuelType ParseFuelType(this string value)
    {
        if (Enum.TryParse<FuelType>(value, true, out var fuelType))
        {
            return fuelType;
        }

        throw new backend.Application.Common.Errors.BadRequestException(
            $"Fuel type must be one of: {string.Join(", ", Enum.GetNames<FuelType>())}.");
    }
}
