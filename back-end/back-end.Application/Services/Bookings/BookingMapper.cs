using backend.Contracts;

namespace backend.Application.Services.Bookings;

internal static class BookingMapper
{
    public static BookingResponse ToResponse(this backend.Domain.Entities.Booking booking) => new(
        booking.Id,
        booking.UserId,
        booking.User?.FullName ?? string.Empty,
        booking.User?.Email ?? string.Empty,
        booking.CarId,
        booking.Car is null ? string.Empty : $"{booking.Car.Make} {booking.Car.Model} ({booking.Car.Year})",
        booking.Car?.LicensePlate ?? string.Empty,
        booking.Car?.ImageUrl,
        booking.StartDate,
        booking.EndDate,
        booking.RentalDays,
        booking.Car?.DailyRate ?? 0m,
        booking.TotalPrice,
        booking.Status.ToString(),
        booking.CreatedAtUtc);
}
