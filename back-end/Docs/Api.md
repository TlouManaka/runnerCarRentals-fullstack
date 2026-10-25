# Runner Car Rentals API

Base URL (development): `https://localhost:7209`

All authenticated endpoints expect a bearer token:

```
Authorization: Bearer <token>
```

Errors are returned as RFC 7807 `application/problem+json`, for example:

```json
{
  "status": 409,
  "title": "Conflict",
  "detail": "This car is already booked for the selected dates.",
  "instance": "/api/bookings"
}
```

## Auth

### Register

```http
POST /api/auth/register
Content-Type: application/json
```

```json
{
  "fullName": "Jane Doe",
  "email": "jane@demo.com",
  "password": "Secure123"
}
```

Response `200 OK`:

```json
{
  "id": "3de9b18d-7c32-490d-ac79-97e6b4c54d81",
  "fullName": "Jane Doe",
  "email": "jane@demo.com",
  "role": "Customer",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

```json
{
  "email": "jane@demo.com",
  "password": "Secure123"
}
```

Response `200 OK`: same shape as register. Invalid credentials return `401`.

## Cars

### List cars

```http
GET /api/cars?search=toyota&availableFrom=2026-11-01&availableTo=2026-11-05
```

All query parameters are optional. When both dates are supplied, cars with an overlapping
non-cancelled booking are excluded. When only one date is supplied, the API returns `400`.

```json
[
  {
    "id": "0f9c2a1e-7d02-4f6f-9a62-2eb6f5e1f7c4",
    "make": "Toyota",
    "model": "Fortuner",
    "year": 2022,
    "licensePlate": "JHB 105 GP",
    "dailyRate": 1100,
    "seats": 7,
    "transmission": "Automatic",
    "fuelType": "Diesel",
    "imageUrl": "https://images.unsplash.com/...",
    "description": "Seven-seat SUV built for family holidays.",
    "isAvailable": true,
    "displayName": "Toyota Fortuner (2022)"
  }
]
```

### Get a car

```http
GET /api/cars/{id}
```

### Create, update and delete (Admin)

```http
POST   /api/cars          (Admin)
PUT    /api/cars/{id}     (Admin)
DELETE /api/cars/{id}     (Admin)
```

Create/update body:

```json
{
  "make": "Toyota",
  "model": "Corolla Quest",
  "year": 2023,
  "licensePlate": "JHB 101 GP",
  "dailyRate": 450,
  "seats": 5,
  "transmission": "Manual",
  "fuelType": "Petrol",
  "imageUrl": "https://...",
  "description": "Reliable sedan for city driving.",
  "isAvailable": true
}
```

A car that already has bookings cannot be deleted (`409 Conflict`). Mark it as
`isAvailable: false` instead.

## Bookings

### Check availability (public)

```http
GET /api/bookings/availability?carId={carId}&startDate=2026-11-01&endDate=2026-11-05
```

```json
{
  "carId": "0f9c2a1e-...",
  "startDate": "2026-11-01T00:00:00",
  "endDate": "2026-11-05T00:00:00",
  "isAvailable": true,
  "estimatedTotal": 4400
}
```

### Create a booking (Customer)

```http
POST /api/bookings
```

```json
{
  "carId": "0f9c2a1e-...",
  "startDate": "2026-11-01",
  "endDate": "2026-11-05"
}
```

Rules:

- pick-up date cannot be in the past
- drop-off must be after pick-up
- rentals are limited to 90 days
- the car must be available and have no overlapping active booking (`409`)
- `totalPrice = dailyRate × rental days`

### My bookings (Customer)

```http
GET /api/bookings/mine
```

### Cancel a booking (Customer or Admin)

```http
POST /api/bookings/{id}/cancel
```

Customers may cancel their own upcoming bookings. Bookings that already started and
`Completed`/`Cancelled` bookings can only be handled by an admin.

### All bookings (Admin)

```http
GET /api/bookings
```

### Update booking status (Admin)

```http
PATCH /api/bookings/{id}/status
```

```json
{ "status": "Confirmed" }
```

Allowed statuses: `Pending`, `Confirmed`, `Completed`, `Cancelled`.
Confirming a booking re-checks that no other confirmed booking overlaps its dates.
