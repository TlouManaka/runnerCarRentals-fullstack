using backend.Application.Common.Interfaces.Persistence;
using backend.Domain.Entities;
using backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _dbContext;

    public CarRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Car>> SearchAsync(
        string? search,
        DateTime? availableFrom,
        DateTime? availableTo,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Cars.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c =>
                c.Make.ToLower().Contains(term) ||
                c.Model.ToLower().Contains(term) ||
                c.LicensePlate.ToLower().Contains(term));
        }

        if (availableFrom.HasValue && availableTo.HasValue)
        {
            var start = availableFrom.Value.Date;
            var end = availableTo.Value.Date;

            query = query.Where(c =>
                c.IsAvailable &&
                !c.Bookings.Any(b =>
                    b.Status != BookingStatus.Cancelled &&
                    b.StartDate < end &&
                    start < b.EndDate));
        }

        return await query
            .OrderBy(c => c.Make)
            .ThenBy(c => c.Model)
            .ToListAsync(cancellationToken);
    }

    public Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Cars.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> LicensePlateExistsAsync(
        string licensePlate,
        Guid? excludeCarId = null,
        CancellationToken cancellationToken = default) =>
        _dbContext.Cars.AnyAsync(
            c => c.LicensePlate == licensePlate && (excludeCarId == null || c.Id != excludeCarId),
            cancellationToken);

    public Task<bool> HasOverlappingBookingAsync(
        Guid carId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeBookingId = null,
        CancellationToken cancellationToken = default) =>
        _dbContext.Bookings.AnyAsync(
            b => b.CarId == carId &&
                 b.Status != BookingStatus.Cancelled &&
                 (excludeBookingId == null || b.Id != excludeBookingId) &&
                 b.StartDate < endDate &&
                 startDate < b.EndDate,
            cancellationToken);

    public Task<bool> HasAnyBookingAsync(Guid carId, CancellationToken cancellationToken = default) =>
        _dbContext.Bookings.AnyAsync(b => b.CarId == carId, cancellationToken);

    public async Task AddAsync(Car car, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cars.AddAsync(car, cancellationToken);
    }

    public void Update(Car car)
    {
        _dbContext.Cars.Update(car);
    }

    public void Remove(Car car)
    {
        _dbContext.Cars.Remove(car);
    }
}
