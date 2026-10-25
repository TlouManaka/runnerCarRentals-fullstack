using backend.Domain.Enums;

namespace backend.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid CarId { get; set; }

    public Car Car { get; set; } = null!;

    /// <summary>Pick-up date (UTC, date component is what matters).</summary>
    public DateTime StartDate { get; set; }

    /// <summary>Drop-off date (UTC, exclusive end of the rental period).</summary>
    public DateTime EndDate { get; set; }

    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public int RentalDays => Math.Max(1, (EndDate.Date - StartDate.Date).Days);
}
