using System.ComponentModel.DataAnnotations;

namespace backend.Contracts;

public record CreateCarRequest(
    [Required, StringLength(50)] string Make,
    [Required, StringLength(50)] string Model,
    [Range(1950, 2100)] int Year,
    [Required, StringLength(20)] string LicensePlate,
    [Range(0.01, 100000)] decimal DailyRate,
    [Range(1, 60)] int Seats,
    [Required] string Transmission,
    [Required] string FuelType,
    string? ImageUrl,
    [StringLength(1000)] string? Description,
    bool IsAvailable = true
);
