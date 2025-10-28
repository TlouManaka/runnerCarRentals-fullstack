namespace backend.Contracts;

public record RegisterRequest(

    string FullName,
    string Email,
    string Password,
    string DriverLicenseNumber,
    string Token

);