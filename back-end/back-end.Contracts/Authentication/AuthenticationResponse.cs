namespace backend.Contracts;

public record AuthenticationResponse(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    string Token
);
