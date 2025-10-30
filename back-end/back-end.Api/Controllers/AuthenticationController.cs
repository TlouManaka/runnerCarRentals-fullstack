using Microsoft.AspNetCore.Mvc;
namespace backend.Application;
using backend.Contracts;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{

    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest registerRequest)
    {

        var authResult = _authenticationService.Register(
            registerRequest.FullName,
            registerRequest.Password,
            registerRequest.Email,
            registerRequest.DriverLicenseNumber

        );

        var response = new AuthenticationResponse(

            authResult.Id,
            authResult.FulltName,
            authResult.Email,
            authResult.DriverLicenseNumber,
            authResult.Token

        );
        
        return Ok(response);
    }


    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {

        var authResult = _authenticationService.Login(

       loginRequest.Password,
       loginRequest.Email


   );
        
          var response = new AuthenticationResponse(

            authResult.Id,
            authResult.FulltName,
            authResult.Email,
            authResult.DriverLicenseNumber,
            authResult.Token

        );

        return Ok(response);
    }



}


