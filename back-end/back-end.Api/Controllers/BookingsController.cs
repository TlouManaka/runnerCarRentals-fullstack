using System.Security.Claims;
using backend.Application;
using backend.Contracts;
using backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>Checks whether a car can be rented for a date range and returns the estimated price.</summary>
    [HttpGet("availability")]
    [AllowAnonymous]
    public async Task<ActionResult<CarAvailabilityResponse>> Availability(
        [FromQuery] Guid carId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        var availability = await _bookingService.CheckAvailabilityAsync(
            carId,
            startDate,
            endDate,
            cancellationToken);

        return Ok(availability);
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create(
        CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingService.CreateAsync(CurrentUserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetMine), booking);
    }

    [HttpGet("mine")]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetMine(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetMyBookingsAsync(CurrentUserId, cancellationToken);
        return Ok(bookings);
    }

    [HttpGet]
    [Authorize(Roles = backend.Domain.Entities.User.AdminRole)]
    public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var bookings = await _bookingService.GetAllAsync(cancellationToken);
        return Ok(bookings);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<BookingResponse>> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingService.CancelAsync(
            CurrentUserId,
            id,
            User.IsInRole(backend.Domain.Entities.User.AdminRole),
            cancellationToken);

        return Ok(booking);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = backend.Domain.Entities.User.AdminRole)]
    public async Task<ActionResult<BookingResponse>> UpdateStatus(
        Guid id,
        UpdateBookingStatusRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookingService.UpdateStatusAsync(id, request.Status, cancellationToken);
        return Ok(booking);
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("The current user could not be identified."));
}
