using System.ComponentModel.DataAnnotations;

namespace backend.Contracts;

public record UpdateBookingStatusRequest(
    [Required] string Status
);
