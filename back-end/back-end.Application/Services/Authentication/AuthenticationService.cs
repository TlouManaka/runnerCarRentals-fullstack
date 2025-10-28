namespace backend.Application;


public class AuhthenticationService : IAuthenticationService
{
    public AuthenticationResult Login(string email, string password)
    {
        return new AuthenticationResult(
         Guid.NewGuid(),
           " fullName",
            email,
            "driverLicenseNumber",
            "token"

        );
    }

    public AuthenticationResult Register(string fullName, string email, string password, string driverLicenseNumber)
    {
        return new AuthenticationResult(
            Guid.NewGuid(),
            fullName,
            email,
            driverLicenseNumber,
            "token");
    }


}