using backend.Application.Common.Errors;
using backend.Application.Common.Interfaces.Persistence;
using backend.Application.Common.Interfaces.Services;
using backend.Application.Services.Bookings;
using backend.Contracts;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Application;

public class BookingService : IBookingService
{
    private const int MaxRentalDays = 90;

    private readonly IBookingRepository _bookingRepository;
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BookingService(
        IBookingRepository bookingRepository,
        ICarRepository carRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _bookingRepository = bookingRepository;
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<BookingResponse> CreateAsync(
        Guid userId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default)
    {
        var startDate = request.StartDate.Date;
        var endDate = request.EndDate.Date;

        ValidateRentalPeriod(startDate, endDate);

        var car = await _carRepository.GetByIdAsync(request.CarId, cancellationToken)
            ?? throw new NotFoundException("Car was not found.");

        if (!car.IsAvailable)
        {
            throw new ConflictException("This car is currently not available for rent.");
        }

        if (await _carRepository.HasOverlappingBookingAsync(car.Id, startDate, endDate, null, cancellationToken))
        {
            throw new ConflictException("This car is already booked for the selected dates.");
        }

        var booking = new Booking
        {
            UserId = userId,
            CarId = car.Id,
            Car = car,
            StartDate = startDate,
            EndDate = endDate,
            TotalPrice = CalculateTotal(car.DailyRate, startDate, endDate),
            Status = BookingStatus.Pending
        };

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _bookingRepository.GetByIdAsync(booking.Id, cancellationToken) ?? booking;
        return created.ToResponse();
    }

    public async Task<IReadOnlyList<BookingResponse>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetByUserAsync(userId, cancellationToken);
        return bookings.Select(b => b.ToResponse()).ToList();
    }

    public async Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
        return bookings.Select(b => b.ToResponse()).ToList();
    }

    public async Task<CarAvailabilityResponse> CheckAvailabilityAsync(
        Guid carId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        ValidateRentalPeriod(start, end);

        var car = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new NotFoundException("Car was not found.");

        var isAvailable = car.IsAvailable &&
            !await _carRepository.HasOverlappingBookingAsync(carId, start, end, null, cancellationToken);

        return new CarAvailabilityResponse(
            car.Id,
            start,
            end,
            isAvailable,
            CalculateTotal(car.DailyRate, start, end));
    }

    public async Task<BookingResponse> CancelAsync(
        Guid userId,
        Guid bookingId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);

        if (booking is null || (!isAdmin && booking.UserId != userId))
        {
            throw new NotFoundException("Booking was not found.");
        }

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
        {
            throw new BadRequestException($"A {booking.Status.ToString().ToLowerInvariant()} booking cannot be cancelled.");
        }

        if (!isAdmin && booking.StartDate <= _dateTimeProvider.UtcNow.Date)
        {
            throw new BadRequestException("Bookings that have already started must be cancelled by an admin.");
        }

        booking.Status = BookingStatus.Cancelled;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.ToResponse();
    }

    public async Task<BookingResponse> UpdateStatusAsync(
        Guid bookingId,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<BookingStatus>(status, true, out var newStatus))
        {
            throw new BadRequestException(
                $"Status must be one of: {string.Join(", ", Enum.GetNames<BookingStatus>())}.");
        }

        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException("Booking was not found.");

        if (newStatus == BookingStatus.Confirmed &&
            await _carRepository.HasOverlappingBookingAsync(
                booking.CarId,
                booking.StartDate,
                booking.EndDate,
                booking.Id,
                cancellationToken))
        {
            throw new ConflictException("Another confirmed booking overlaps these dates.");
        }

        booking.Status = newStatus;

        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.ToResponse();
    }

    private void ValidateRentalPeriod(DateTime startDate, DateTime endDate)
    {
        if (startDate < _dateTimeProvider.UtcNow.Date)
        {
            throw new BadRequestException("Pick-up date cannot be in the past.");
        }

        if (endDate <= startDate)
        {
            throw new BadRequestException("Drop-off date must be after pick-up date.");
        }

        if ((endDate - startDate).Days > MaxRentalDays)
        {
            throw new BadRequestException($"Rentals cannot exceed {MaxRentalDays} days.");
        }
    }

    private static decimal CalculateTotal(decimal dailyRate, DateTime startDate, DateTime endDate)
    {
        var days = Math.Max(1, (endDate - startDate).Days);
        return Math.Round(dailyRate * days, 2);
    }
}
