namespace backend.Application;

public record AuthenticationResult(

Guid Id,
string FulltName,
string Email,
string DriverLicenseNumber,
string Token
);