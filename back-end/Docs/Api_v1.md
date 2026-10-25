# Runner Car Rental API — legacy draft

> **Superseded.** This file was an early planning draft written before the API was implemented.
> It described routes (`/auth/*`) and a customer-profile CRUD module that do not exist in the
> current codebase. The implemented, verified API is documented in [`Api.md`](Api.md).

## What is actually implemented

| Area | Routes |
| --- | --- |
| Authentication | `POST /api/auth/register`, `POST /api/auth/login` |
| Car catalog | `GET /api/cars`, `GET /api/cars/{id}` (public); `POST/PUT/DELETE /api/cars` (admin) |
| Bookings | `GET /api/bookings/availability` (public), `POST /api/bookings`, `GET /api/bookings/mine`, `POST /api/bookings/{id}/cancel` (customer); `GET /api/bookings`, `PATCH /api/bookings/{id}/status` (admin) |

## Not implemented

- Customer profile create/update/delete endpoints and driver-licence storage.
- Payments, email notifications, file uploads for vehicle images.

The live API surface is also visible in Swagger at `http://localhost:5031/swagger` when the API runs in Development.
