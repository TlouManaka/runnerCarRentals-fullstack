import { CurrencyPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Car } from '../../core/models/car.model';
import { CarService } from '../../core/services/car.service';

@Component({
  selector: 'app-home',
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private readonly carService = inject(CarService);

  readonly cars = signal<Car[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.carService.search().subscribe({
      next: (cars) => {
        this.cars.set(cars.slice(0, 3));
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }
}
