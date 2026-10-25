import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Car, CarAvailability } from '../../core/models/car.model';
import { AuthService } from '../../core/services/auth.service';
import { BookingService } from '../../core/services/booking.service';
import { CarService } from '../../core/services/car.service';
import { getErrorMessage } from '../../core/utils/error.util';

@Component({
  selector: 'app-car-detail',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './car-detail.component.html',
  styleUrl: './car-detail.component.css'
})
export class CarDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly carService = inject(CarService);
  private readonly bookingService = inject(BookingService);
  private readonly formBuilder = inject(FormBuilder);

  readonly auth = inject(AuthService);

  readonly car = signal<Car | null>(null);
  readonly loading = signal(true);
  readonly checking = signal(false);
  readonly booking = signal(false);
  readonly availability = signal<CarAvailability | null>(null);
  readonly error = signal<string | null>(null);

  readonly minDate = new Date().toISOString().slice(0, 10);

  readonly form = this.formBuilder.nonNullable.group({
    startDate: ['', Validators.required],
    endDate: ['', Validators.required]
  });

  carId = '';

  ngOnInit(): void {
    this.carId = this.route.snapshot.paramMap.get('id') ?? '';

    const startDate = this.route.snapshot.queryParamMap.get('from') ?? '';
    const endDate = this.route.snapshot.queryParamMap.get('to') ?? '';

    if (startDate && endDate) {
      this.form.setValue({ startDate, endDate });
    }

    this.carService.getById(this.carId).subscribe({
      next: (car) => {
        this.car.set(car);
        this.loading.set(false);

        if (startDate && endDate) {
          this.checkAvailability();
        }
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not load this vehicle.'));
        this.loading.set(false);
      }
    });
  }

  checkAvailability(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { startDate, endDate } = this.form.getRawValue();

    this.checking.set(true);
    this.error.set(null);
    this.availability.set(null);

    this.bookingService.checkAvailability(this.carId, startDate, endDate).subscribe({
      next: (availability) => {
        this.availability.set(availability);
        this.checking.set(false);
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not check availability.'));
        this.checking.set(false);
      }
    });
  }

  submit(): void {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { startDate, endDate } = this.form.getRawValue();

    this.booking.set(true);
    this.error.set(null);

    this.bookingService.create({ carId: this.carId, startDate, endDate }).subscribe({
      next: () => {
        this.booking.set(false);
        this.router.navigate(['/bookings'], { queryParams: { booked: '1' } });
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not create the booking.'));
        this.availability.set(null);
        this.booking.set(false);
      }
    });
  }
}
