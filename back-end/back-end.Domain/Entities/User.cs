
namespace backend.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = null!;

    public string Email { get; set; } =  null!;

    public string DriverLicenseNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

}