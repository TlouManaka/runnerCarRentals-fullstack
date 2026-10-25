namespace backend.Contracts;

public record CarResponse(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string LicensePlate,
    decimal DailyRate,
    int Seats,
    string Transmission,
    string FuelType,
    string? ImageUrl,
    string? Description,
    bool IsAvailable
)
{
    public string DisplayName => $"{Make} {Model} ({Year})";
}
