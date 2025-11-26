namespace backend.Contracts;

public record AuthenticationResponse(
    Guid Id,
    string Email,
    string Token

);