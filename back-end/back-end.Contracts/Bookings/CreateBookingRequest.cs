using System.ComponentModel.DataAnnotations;

namespace backend.Contracts;

public record CreateBookingRequest(
    [Required] Guid CarId,
    [Required] DateTime StartDate,
    [Required] DateTime EndDate
);
