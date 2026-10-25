using backend.Domain.Entities;

namespace backend.Application.Common.Interfaces.Persistence;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default);

    void Update(Booking booking);
}
