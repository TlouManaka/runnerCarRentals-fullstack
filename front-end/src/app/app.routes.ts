import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component').then((m) => m.HomeComponent)
  },
  {
    path: 'cars',
    loadComponent: () => import('./pages/cars/cars.component').then((m) => m.CarsComponent)
  },
  {
    path: 'cars/:id',
    loadComponent: () => import('./pages/car-detail/car-detail.component').then((m) => m.CarDetailComponent)
  },
  {
    path: 'bookings',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/my-bookings/my-bookings.component').then((m) => m.MyBookingsComponent)
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./pages/admin/admin.component').then((m) => m.AdminComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/create-account/create-account.component').then((m) => m.CreateAccountComponent)
  },
  {
    path: 'create-account',
    redirectTo: 'register',
    pathMatch: 'full'
  },
  {
    path: 'createAccount',
    redirectTo: 'register',
    pathMatch: 'full'
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];
