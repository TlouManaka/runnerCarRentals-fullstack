namespace backend.Contracts;

public record BookingResponse(
    Guid Id,
    Guid UserId,
    string CustomerName,
    string CustomerEmail,
    Guid CarId,
    string CarName,
    string LicensePlate,
    string? ImageUrl,
    DateTime StartDate,
    DateTime EndDate,
    int RentalDays,
    decimal DailyRate,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAtUtc
);

public record CarAvailabilityResponse(
    Guid CarId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAvailable,
    decimal EstimatedTotal
);
