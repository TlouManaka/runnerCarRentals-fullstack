using backend.Application.Common.Interfaces.Persistence;
using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Infrastructure.Persistence;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
    }

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _dbContext.Bookings
            .Include(b => b.Car)
            .Include(b => b.User)
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Car)
            .Include(b => b.User)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Bookings
            .AsNoTracking()
            .Include(b => b.Car)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public void Update(Booking booking)
    {
        _dbContext.Bookings.Update(booking);
    }
}
