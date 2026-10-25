using backend.Domain.Entities;

namespace backend.Application;

public record AuthenticationResult(
    User User,
    string Token
);
