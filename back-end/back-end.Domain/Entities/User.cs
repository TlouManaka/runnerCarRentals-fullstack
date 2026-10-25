namespace backend.Domain.Entities;

public class User
{
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";

    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>PBKDF2 hash produced by <c>IPasswordHasher</c>. Never the raw password.</summary>
    public string PasswordHash { get; set; } = null!;

    public string UserRole { get; set; } = CustomerRole;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
