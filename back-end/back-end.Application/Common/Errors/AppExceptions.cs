namespace backend.Application.Common.Errors;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }

    public abstract int StatusCode { get; }
}

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message)
    {
    }

    public override int StatusCode => 400;
}

public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message)
    {
    }

    public override int StatusCode => 401;
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public override int StatusCode => 404;
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message)
    {
    }

    public override int StatusCode => 409;
}
