using backend.Application;
using backend.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers;

[ApiController]
[Route("api/cars")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    /// <summary>Lists cars. Optionally filter by text and by availability for a date range.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CarResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] DateTime? availableFrom,
        [FromQuery] DateTime? availableTo,
        CancellationToken cancellationToken)
    {
        var cars = await _carService.SearchAsync(search, availableFrom, availableTo, cancellationToken);
        return Ok(cars);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CarResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var car = await _carService.GetByIdAsync(id, cancellationToken);
        return Ok(car);
    }

    [HttpPost]
    [Authorize(Roles = backend.Domain.Entities.User.AdminRole)]
    public async Task<ActionResult<CarResponse>> Create(
        CreateCarRequest request,
        CancellationToken cancellationToken)
    {
        var car = await _carService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = backend.Domain.Entities.User.AdminRole)]
    public async Task<ActionResult<CarResponse>> Update(
        Guid id,
        UpdateCarRequest request,
        CancellationToken cancellationToken)
    {
        var car = await _carService.UpdateAsync(id, request, cancellationToken);
        return Ok(car);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = backend.Domain.Entities.User.AdminRole)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _carService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
