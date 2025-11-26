
namespace backend.Application;

using  backend.Application.Common.Interfaces.Authentication;
using backend.Application.Common.Interfaces.Persistence;
using backend.Domain.Entities;

public class AuhthenticationService : IAuthenticationService
{

    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuhthenticationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }

    public AuthenticationResult Login(string email, string password)
    {

        // 1. Validate the user exists
        if(_userRepository.GetUserByEmail(email) is not User user)
        {

            throw new Exception("User with given email does not exist.");
            
        }
        // 2.Validate the password is correct 
            if(user.Password != password)
        {
            throw new Exception("Invalid Password");
        }

         var token =  _jwtTokenGenerator.GenerateToken(user);


        // 3.Create JWT token
        return new AuthenticationResult(
            user,
            token

        );
    }

    public AuthenticationResult Register(string fullName, string email, string password, string driverLicenseNumber)
    {

        //check if user already exists
         if(_userRepository.GetUserByEmail(email) is not null)
        {
            throw new Exception("User with given email already exists.");
        }
        //create user (generate unique ID)
        var user  =  new User
        {
            FullName = fullName,
            Email = email,
            Password = password,
            DriverLicenseNumber = driverLicenseNumber

        };

        _userRepository.Add(user);
     
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthenticationResult(
            user,
            token);
    }


}