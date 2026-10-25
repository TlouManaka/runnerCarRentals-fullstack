using backend.Domain.Entities;

namespace backend.Application;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> RegisterAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken = default);
}
