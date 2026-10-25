using backend.Contracts;

namespace backend.Application;

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(
        Guid userId,
        CreateBookingRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingResponse>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BookingResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<CarAvailabilityResponse> CheckAvailabilityAsync(
        Guid carId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<BookingResponse> CancelAsync(
        Guid userId,
        Guid bookingId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<BookingResponse> UpdateStatusAsync(
        Guid bookingId,
        string status,
        CancellationToken cancellationToken = default);
}
