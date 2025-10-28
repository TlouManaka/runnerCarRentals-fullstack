namespace backend.Application;

public interface IAuthenticationService
{

    AuthenticationResult Login(string email, string password);

    AuthenticationResult Register(string fullName, string email, string password,string driverLicenseNumber);

}