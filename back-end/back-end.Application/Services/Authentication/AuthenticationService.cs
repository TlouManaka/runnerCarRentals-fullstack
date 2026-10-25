using backend.Application.Common.Errors;
using backend.Application.Common.Interfaces.Authentication;
using backend.Application.Common.Interfaces.Persistence;
using backend.Domain.Entities;

namespace backend.Application;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordHasher passwordHasher,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken)
            ?? throw new UnauthorizedException("Incorrect email or password.");

        if (!_passwordHasher.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedException("Incorrect email or password.");
        }

        return new AuthenticationResult(user, _jwtTokenGenerator.GenerateToken(user));
    }

    public async Task<AuthenticationResult> RegisterAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("An account with this email already exists.");
        }

        var user = new User
        {
            FullName = fullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(password),
            UserRole = User.CustomerRole
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(user, _jwtTokenGenerator.GenerateToken(user));
    }
}
