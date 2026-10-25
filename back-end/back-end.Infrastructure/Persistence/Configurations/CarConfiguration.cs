using backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.Infrastructure.Persistence.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Make).IsRequired().HasMaxLength(50);

        builder.Property(c => c.Model).IsRequired().HasMaxLength(50);

        builder.Property(c => c.Year).IsRequired();

        builder.Property(c => c.LicensePlate).IsRequired().HasMaxLength(20);

        builder.HasIndex(c => c.LicensePlate).IsUnique();

        builder.Property(c => c.DailyRate).HasPrecision(18, 2);

        builder.Property(c => c.Seats).IsRequired();

        builder.Property(c => c.Transmission).HasConversion<string>().HasMaxLength(20);

        builder.Property(c => c.FuelType).HasConversion<string>().HasMaxLength(20);

        builder.Property(c => c.ImageUrl).HasMaxLength(500);

        builder.Property(c => c.Description).HasMaxLength(1000);

        builder.Property(c => c.IsAvailable).IsRequired();

        builder.Property(c => c.CreatedAtUtc).IsRequired();
    }
}
