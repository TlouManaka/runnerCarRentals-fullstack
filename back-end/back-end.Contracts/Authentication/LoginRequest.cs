using System.ComponentModel.DataAnnotations;

namespace backend.Contracts;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
