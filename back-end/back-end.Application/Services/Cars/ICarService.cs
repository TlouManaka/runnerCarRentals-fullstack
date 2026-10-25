namespace backend.Application;

public interface ICarService
{
    Task<IReadOnlyList<backend.Contracts.CarResponse>> SearchAsync(
        string? search,
        DateTime? availableFrom,
        DateTime? availableTo,
        CancellationToken cancellationToken = default);

    Task<backend.Contracts.CarResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<backend.Contracts.CarResponse> CreateAsync(
        backend.Contracts.CreateCarRequest request,
        CancellationToken cancellationToken = default);

    Task<backend.Contracts.CarResponse> UpdateAsync(
        Guid id,
        backend.Contracts.UpdateCarRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
