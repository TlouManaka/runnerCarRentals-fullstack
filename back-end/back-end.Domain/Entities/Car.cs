using backend.Domain.Enums;

namespace backend.Domain.Entities;

public class Car
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string LicensePlate { get; set; } = null!;

    public decimal DailyRate { get; set; }

    public int Seats { get; set; }

    public TransmissionType Transmission { get; set; } = TransmissionType.Automatic;

    public FuelType FuelType { get; set; } = FuelType.Petrol;

    public string? ImageUrl { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Admin switch that takes the vehicle off the market entirely.
    /// Per-date availability is derived from bookings.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
