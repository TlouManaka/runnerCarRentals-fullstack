import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Booking } from '../../core/models/booking.model';
import { Car, CarRequest } from '../../core/models/car.model';
import { BookingService } from '../../core/services/booking.service';
import { CarService } from '../../core/services/car.service';
import { getErrorMessage } from '../../core/utils/error.util';

type AdminTab = 'cars' | 'bookings';

@Component({
  selector: 'app-admin',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit {
  private readonly carService = inject(CarService);
  private readonly bookingService = inject(BookingService);
  private readonly formBuilder = inject(FormBuilder);

  readonly tab = signal<AdminTab>('cars');
  readonly cars = signal<Car[]>([]);
  readonly bookings = signal<Booking[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);

  readonly showForm = signal(false);
  readonly editingId = signal<string | null>(null);
  readonly statuses = ['Pending', 'Confirmed', 'Completed', 'Cancelled'] as const;

  readonly form = this.formBuilder.nonNullable.group({
    make: ['', Validators.required],
    model: ['', Validators.required],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1950), Validators.max(2100)]],
    licensePlate: ['', Validators.required],
    dailyRate: [500, [Validators.required, Validators.min(1)]],
    seats: [5, [Validators.required, Validators.min(1), Validators.max(60)]],
    transmission: ['Manual', Validators.required],
    fuelType: ['Petrol', Validators.required],
    imageUrl: [''],
    description: [''],
    isAvailable: [true]
  });

  ngOnInit(): void {
    this.loadAll();
  }

  switchTab(tab: AdminTab): void {
    this.tab.set(tab);
  }

  startCreate(): void {
    this.editingId.set(null);
    this.resetForm();
    this.showForm.set(true);
    this.error.set(null);
    this.success.set(null);
  }

  startEdit(car: Car): void {
    this.editingId.set(car.id);
    this.form.setValue({
      make: car.make,
      model: car.model,
      year: car.year,
      licensePlate: car.licensePlate,
      dailyRate: car.dailyRate,
      seats: car.seats,
      transmission: car.transmission,
      fuelType: car.fuelType,
      imageUrl: car.imageUrl ?? '',
      description: car.description ?? '',
      isAvailable: car.isAvailable
    });
    this.showForm.set(true);
    this.error.set(null);
    this.success.set(null);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const request: CarRequest = {
      ...value,
      imageUrl: value.imageUrl.trim() || null,
      description: value.description.trim() || null
    };

    const id = this.editingId();
    const request$ = id
      ? this.carService.update(id, request)
      : this.carService.create(request);

    this.saving.set(true);
    this.error.set(null);

    request$.subscribe({
      next: () => {
        this.success.set(id ? 'Vehicle updated.' : 'Vehicle added to the fleet.');
        this.saving.set(false);
        this.showForm.set(false);
        this.resetForm();
        this.loadCars();
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not save the vehicle.'));
        this.saving.set(false);
      }
    });
  }

  remove(car: Car): void {
    if (!confirm(`Delete ${car.make} ${car.model} (${car.licensePlate})?`)) {
      return;
    }

    this.error.set(null);

    this.carService.delete(car.id).subscribe({
      next: () => {
        this.success.set('Vehicle deleted.');
        this.loadCars();
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not delete the vehicle.'));
      }
    });
  }

  changeStatus(booking: Booking, event: Event): void {
    const status = (event.target as HTMLSelectElement).value;

    if (status === booking.status) {
      return;
    }

    this.error.set(null);

    this.bookingService.updateStatus(booking.id, status).subscribe({
      next: (updated) => {
        this.bookings.update((bookings) =>
          bookings.map((item) => (item.id === updated.id ? updated : item))
        );
        this.success.set('Booking status updated.');
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not update the booking status.'));
      }
    });
  }

  statusClass(status: string): string {
    return `status status-${status.toLowerCase()}`;
  }

  private loadAll(): void {
    this.loading.set(true);

    forkJoin({
      cars: this.carService.search(),
      bookings: this.bookingService.getAll()
    }).subscribe({
      next: ({ cars, bookings }) => {
        this.cars.set(cars);
        this.bookings.set(bookings);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not load admin data.'));
        this.loading.set(false);
      }
    });
  }

  private loadCars(): void {
    this.carService.search().subscribe({
      next: (cars) => this.cars.set(cars),
      error: (error: unknown) => this.error.set(getErrorMessage(error, 'Could not refresh the fleet.'))
    });
  }

  private resetForm(): void {
    this.form.reset({
      make: '',
      model: '',
      year: new Date().getFullYear(),
      licensePlate: '',
      dailyRate: 500,
      seats: 5,
      transmission: 'Manual',
      fuelType: 'Petrol',
      imageUrl: '',
      description: '',
      isAvailable: true
    });
  }
}
