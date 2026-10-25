# Domain model

## User

| Property | Type | Notes |
| --- | --- | --- |
| `Id` | `Guid` | Primary key |
| `FullName` | `string` | Max 100 characters |
| `Email` | `string` | Unique, stored lowercase |
| `PasswordHash` | `string` | PBKDF2-SHA512, never the raw password |
| `UserRole` | `string` | `Customer` or `Admin` |
| `CreatedAtUtc` | `DateTime` | UTC |
| `Bookings` | `ICollection<Booking>` | One-to-many |

## Car

| Property | Type | Notes |
| --- | --- | --- |
| `Id` | `Guid` | Primary key |
| `Make` / `Model` | `string` | Max 50 characters each |
| `Year` | `int` | 1950–2100 |
| `LicensePlate` | `string` | Unique, stored uppercase |
| `DailyRate` | `decimal(18,2)` | ZAR per day |
| `Seats` | `int` | 1–60 |
| `Transmission` | `TransmissionType` | `Manual`, `Automatic` |
| `FuelType` | `FuelType` | `Petrol`, `Diesel`, `Hybrid`, `Electric` |
| `ImageUrl` | `string?` | Optional |
| `Description` | `string?` | Max 1000 characters |
| `IsAvailable` | `bool` | Admin switch to remove the car from the market |
| `CreatedAtUtc` | `DateTime` | UTC |
| `Bookings` | `ICollection<Booking>` | One-to-many |

## Booking

| Property | Type | Notes |
| --- | --- | --- |
| `Id` | `Guid` | Primary key |
| `UserId` | `Guid` | FK to `User` (restrict delete) |
| `CarId` | `Guid` | FK to `Car` (restrict delete) |
| `StartDate` | `DateTime` | Pick-up date (UTC, date component used) |
| `EndDate` | `DateTime` | Drop-off date, exclusive |
| `TotalPrice` | `decimal(18,2)` | Snapshot of `dailyRate × days` at booking time |
| `Status` | `BookingStatus` | `Pending`, `Confirmed`, `Completed`, `Cancelled` |
| `CreatedAtUtc` | `DateTime` | UTC |

### Availability rule

A car is unavailable for a requested period when an existing booking for that car:

1. is **not** `Cancelled`, and
2. satisfies `existing.StartDate < requested.EndDate && requested.StartDate < existing.EndDate`.

This is evaluated in SQL by `CarRepository.HasOverlappingBookingAsync`, so the same rule
drives both catalog filtering and booking creation.

### Status lifecycle

```
Pending ──► Confirmed ──► Completed
   │            │
   └────────────┴────────► Cancelled
```

- Customers create `Pending` bookings and may cancel them before the pick-up date.
- Admins confirm bookings (re-checking overlaps) and move them to `Completed`.
