# Runner Car Rentals — Angular client

Angular 19 application for browsing the fleet and managing bookings.

## Development server

```bash
npm install
npm start
```

Open <http://localhost:4200>. Requests to `/api` are proxied to the ASP.NET Core API at
`http://localhost:5031` (see `proxy.conf.json`), so start the backend first:

```bash
cd ../back-end
dotnet run --project back-end.Api
```

## Structure

```
src/app/
├── core/
│   ├── models/          # Car, Booking, Auth contracts
│   ├── services/        # AuthService, CarService, BookingService
│   ├── guards/          # authGuard, adminGuard
│   ├── interceptors/    # bearer-token interceptor
│   └── utils/           # API error extraction
├── components/navbar/   # Role-aware navigation
└── pages/
    ├── home/            # Landing page + featured cars
    ├── cars/            # Catalog with text/date filters
    ├── car-detail/      # Vehicle details + booking form
    ├── my-bookings/     # Customer booking list
    ├── admin/           # Fleet CRUD + booking status management
    ├── login/
    └── create-account/
```

## Commands

```bash
npm start        # dev server
npm run build    # production build (SSR bundle included)
npm test         # Karma/Jasmine unit tests
```

Routes are rendered client-side (`RenderMode.Client`) because every page depends on the
authenticated session and live API data.
