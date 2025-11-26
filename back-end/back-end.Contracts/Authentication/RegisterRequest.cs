namespace backend.Contracts;

public record RegisterRequest(

    string Email,
    string Password,
    string Token

);