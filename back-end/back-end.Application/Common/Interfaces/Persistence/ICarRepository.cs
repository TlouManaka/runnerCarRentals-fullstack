using backend.Domain.Entities;

namespace backend.Application.Common.Interfaces.Persistence;

public interface ICarRepository
{
    Task<IReadOnlyList<Car>> SearchAsync(
        string? search,
        DateTime? availableFrom,
        DateTime? availableTo,
        CancellationToken cancellationToken = default);

    Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> LicensePlateExistsAsync(
        string licensePlate,
        Guid? excludeCarId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingBookingAsync(
        Guid carId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasAnyBookingAsync(Guid carId, CancellationToken cancellationToken = default);

    Task AddAsync(Car car, CancellationToken cancellationToken = default);

    void Update(Car car);

    void Remove(Car car);
}
