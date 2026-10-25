using backend.Application;
using backend.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.RegisterAsync(
            request.FullName,
            request.Email,
            request.Password,
            cancellationToken);

        return Ok(ToResponse(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authenticationService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return Ok(ToResponse(result));
    }

    private static AuthenticationResponse ToResponse(AuthenticationResult result) => new(
        result.User.Id,
        result.User.FullName,
        result.User.Email,
        result.User.UserRole,
        result.Token);
}
