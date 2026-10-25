using backend.Application;
using backend.Application.Common.Errors;
using backend.Application.Common.Interfaces.Persistence;
using backend.Application.Common.Interfaces.Services;
using backend.Contracts;
using backend.Domain.Entities;
using backend.Domain.Enums;
using Xunit;

namespace backend.Application.Tests;

public class BookingServiceTests
{
    private static readonly DateTime Today = new(2026, 11, 1);

    private readonly FakeBookingRepository _bookingRepository = new();
    private readonly FakeCarRepository _carRepository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FixedDateTimeProvider _dateTimeProvider = new(Today);

    public BookingServiceTests()
    {
        _carRepository.Cars.Add(new Car
        {
            Id = Guid.NewGuid(),
            Make = "Toyota",
            Model = "Corolla",
            DailyRate = 450m,
            IsAvailable = true
        });
    }

    [Fact]
    public async Task CreateAsync_ComputesTotalFromDailyRateAndDays()
    {
        var service = CreateService();
        var car = _carRepository.Cars[0];

        var booking = await service.CreateAsync(
            Guid.NewGuid(),
            new CreateBookingRequest(car.Id, Today.AddDays(2), Today.AddDays(5)));

        Assert.Equal(1350m, booking.TotalPrice);
        Assert.Equal("Pending", booking.Status);
        Assert.Equal(3, booking.RentalDays);
        Assert.Equal(1, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflict_WhenCarAlreadyBookedForDates()
    {
        _carRepository.HasOverlap = true;
        var service = CreateService();

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateAsync(
                Guid.NewGuid(),
                new CreateBookingRequest(_carRepository.Cars[0].Id, Today.AddDays(1), Today.AddDays(3))));
    }

    [Fact]
    public async Task CreateAsync_ThrowsBadRequest_WhenPickUpDateIsInThePast()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<BadRequestException>(() =>
            service.CreateAsync(
                Guid.NewGuid(),
                new CreateBookingRequest(_carRepository.Cars[0].Id, Today.AddDays(-1), Today.AddDays(2))));
    }

    [Fact]
    public async Task CreateAsync_ThrowsConflict_WhenCarIsMarkedUnavailable()
    {
        _carRepository.Cars[0].IsAvailable = false;
        var service = CreateService();

        await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateAsync(
                Guid.NewGuid(),
                new CreateBookingRequest(_carRepository.Cars[0].Id, Today.AddDays(1), Today.AddDays(2))));
    }

    [Fact]
    public async Task CancelAsync_ThrowsNotFound_WhenBookingBelongsToAnotherCustomer()
    {
        var ownerId = Guid.NewGuid();
        var booking = new Booking
        {
            UserId = ownerId,
            CarId = _carRepository.Cars[0].Id,
            StartDate = Today.AddDays(1),
            EndDate = Today.AddDays(2),
            Status = BookingStatus.Pending
        };

        _bookingRepository.Bookings.Add(booking);

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CancelAsync(Guid.NewGuid(), booking.Id, isAdmin: false));
    }

    [Fact]
    public async Task CancelAsync_AllowsOwnerToCancelUpcomingBooking()
    {
        var ownerId = Guid.NewGuid();
        var booking = new Booking
        {
            UserId = ownerId,
            CarId = _carRepository.Cars[0].Id,
            StartDate = Today.AddDays(1),
            EndDate = Today.AddDays(2),
            Status = BookingStatus.Confirmed
        };

        _bookingRepository.Bookings.Add(booking);

        var service = CreateService();

        var cancelled = await service.CancelAsync(ownerId, booking.Id, isAdmin: false);

        Assert.Equal("Cancelled", cancelled.Status);
    }

    private BookingService CreateService() =>
        new(_bookingRepository, _carRepository, _unitOfWork, _dateTimeProvider);

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public FixedDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCount++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeCarRepository : ICarRepository
    {
        public List<Car> Cars { get; } = new();

        public bool HasOverlap { get; set; }

        public Task<IReadOnlyList<Car>> SearchAsync(
            string? search,
            DateTime? availableFrom,
            DateTime? availableTo,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Car>>(Cars);

        public Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Cars.SingleOrDefault(c => c.Id == id));

        public Task<bool> LicensePlateExistsAsync(
            string licensePlate,
            Guid? excludeCarId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Cars.Any(c => c.LicensePlate == licensePlate && c.Id != excludeCarId));

        public Task<bool> HasOverlappingBookingAsync(
            Guid carId,
            DateTime startDate,
            DateTime endDate,
            Guid? excludeBookingId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(HasOverlap);

        public Task<bool> HasAnyBookingAsync(Guid carId, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public Task AddAsync(Car car, CancellationToken cancellationToken = default)
        {
            Cars.Add(car);
            return Task.CompletedTask;
        }

        public void Update(Car car)
        {
        }

        public void Remove(Car car)
        {
            Cars.Remove(car);
        }
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        public List<Booking> Bookings { get; } = new();

        public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
        {
            Bookings.Add(booking);
            return Task.CompletedTask;
        }

        public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Bookings.SingleOrDefault(b => b.Id == id));

        public Task<IReadOnlyList<Booking>> GetByUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Booking>>(Bookings.Where(b => b.UserId == userId).ToList());

        public Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Booking>>(Bookings);

        public void Update(Booking booking)
        {
        }
    }
}
