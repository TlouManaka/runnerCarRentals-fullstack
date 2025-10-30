namespace backend.Contracts;

public record LoginRequest(

    string Email,
    string Password

);