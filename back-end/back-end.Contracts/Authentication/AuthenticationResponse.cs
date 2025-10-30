namespace backend.Contracts;

public record AuthenticationResponse(
    Guid Id,
    string FulltName,
    string Email,
    string DriverLicenseNumber,
    string Token

);