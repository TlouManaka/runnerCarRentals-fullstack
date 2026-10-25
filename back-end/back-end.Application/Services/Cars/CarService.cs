using backend.Application.Common.Errors;
using backend.Application.Common.Interfaces.Persistence;
using backend.Application.Common.Interfaces.Services;
using backend.Application.Services.Cars;
using backend.Contracts;
using backend.Domain.Entities;

namespace backend.Application;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CarService(
        ICarRepository carRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _carRepository = carRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<CarResponse>> SearchAsync(
        string? search,
        DateTime? availableFrom,
        DateTime? availableTo,
        CancellationToken cancellationToken = default)
    {
        if (availableFrom.HasValue != availableTo.HasValue)
        {
            throw new BadRequestException("Provide both availableFrom and availableTo, or neither.");
        }

        if (availableFrom.HasValue && availableTo.HasValue)
        {
            if (availableTo.Value.Date <= availableFrom.Value.Date)
            {
                throw new BadRequestException("Drop-off date must be after pick-up date.");
            }

            if (availableFrom.Value.Date < _dateTimeProvider.UtcNow.Date)
            {
                throw new BadRequestException("Pick-up date cannot be in the past.");
            }
        }

        var cars = await _carRepository.SearchAsync(
            search?.Trim(),
            availableFrom?.Date,
            availableTo?.Date,
            cancellationToken);

        return cars.Select(c => c.ToResponse()).ToList();
    }

    public async Task<CarResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var car = await _carRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Car was not found.");

        return car.ToResponse();
    }

    public async Task<CarResponse> CreateAsync(
        CreateCarRequest request,
        CancellationToken cancellationToken = default)
    {
        var licensePlate = NormalizeLicensePlate(request.LicensePlate);

        if (await _carRepository.LicensePlateExistsAsync(licensePlate, null, cancellationToken))
        {
            throw new ConflictException($"A car with license plate {licensePlate} already exists.");
        }

        var car = new Car
        {
            Make = request.Make.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            LicensePlate = licensePlate,
            DailyRate = request.DailyRate,
            Seats = request.Seats,
            Transmission = request.Transmission.ParseTransmission(),
            FuelType = request.FuelType.ParseFuelType(),
            ImageUrl = request.ImageUrl?.Trim(),
            Description = request.Description?.Trim(),
            IsAvailable = request.IsAvailable
        };

        await _carRepository.AddAsync(car, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return car.ToResponse();
    }

    public async Task<CarResponse> UpdateAsync(
        Guid id,
        UpdateCarRequest request,
        CancellationToken cancellationToken = default)
    {
        var car = await _carRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Car was not found.");

        var licensePlate = NormalizeLicensePlate(request.LicensePlate);

        if (await _carRepository.LicensePlateExistsAsync(licensePlate, id, cancellationToken))
        {
            throw new ConflictException($"A car with license plate {licensePlate} already exists.");
        }

        car.Make = request.Make.Trim();
        car.Model = request.Model.Trim();
        car.Year = request.Year;
        car.LicensePlate = licensePlate;
        car.DailyRate = request.DailyRate;
        car.Seats = request.Seats;
        car.Transmission = request.Transmission.ParseTransmission();
        car.FuelType = request.FuelType.ParseFuelType();
        car.ImageUrl = request.ImageUrl?.Trim();
        car.Description = request.Description?.Trim();
        car.IsAvailable = request.IsAvailable;

        _carRepository.Update(car);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return car.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var car = await _carRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Car was not found.");

        if (await _carRepository.HasAnyBookingAsync(id, cancellationToken))
        {
            throw new ConflictException("This car has bookings and cannot be deleted. Mark it as unavailable instead.");
        }

        _carRepository.Remove(car);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeLicensePlate(string licensePlate) =>
        licensePlate.Trim().ToUpperInvariant();
}
