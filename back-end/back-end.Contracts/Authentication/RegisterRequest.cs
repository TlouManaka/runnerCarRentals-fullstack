using System.ComponentModel.DataAnnotations;

namespace backend.Contracts;

public record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 2)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, StringLength(100, MinimumLength = 8)] string Password
);
