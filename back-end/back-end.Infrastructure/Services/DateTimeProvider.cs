using backend.Application.Common.Interfaces.Services;

namespace backend.Infrastructure.Services;




public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}