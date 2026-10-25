import { CurrencyPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Car } from '../../core/models/car.model';
import { CarService } from '../../core/services/car.service';
import { getErrorMessage } from '../../core/utils/error.util';

@Component({
  selector: 'app-cars',
  imports: [FormsModule, RouterLink, CurrencyPipe],
  templateUrl: './cars.component.html',
  styleUrl: './cars.component.css'
})
export class CarsComponent implements OnInit {
  private readonly carService = inject(CarService);

  readonly cars = signal<Car[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly minDate = new Date().toISOString().slice(0, 10);

  searchTerm = '';
  availableFrom = '';
  availableTo = '';

  ngOnInit(): void {
    this.load();
  }

  applyFilters(): void {
    if (Boolean(this.availableFrom) !== Boolean(this.availableTo)) {
      this.error.set('Choose both a pick-up and a drop-off date.');
      return;
    }

    if (this.availableFrom && this.availableTo && this.availableTo <= this.availableFrom) {
      this.error.set('The drop-off date must be after the pick-up date.');
      return;
    }

    this.load();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.availableFrom = '';
    this.availableTo = '';
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.carService.search(this.searchTerm, this.availableFrom, this.availableTo).subscribe({
      next: (cars) => {
        this.cars.set(cars);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not load the fleet. Please try again.'));
        this.loading.set(false);
      }
    });
  }
}
