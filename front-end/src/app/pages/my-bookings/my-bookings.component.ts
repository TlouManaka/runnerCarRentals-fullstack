import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Booking } from '../../core/models/booking.model';
import { BookingService } from '../../core/services/booking.service';
import { getErrorMessage } from '../../core/utils/error.util';

@Component({
  selector: 'app-my-bookings',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.css'
})
export class MyBookingsComponent implements OnInit {
  private readonly bookingService = inject(BookingService);

  readonly bookings = signal<Booking[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly cancellingId = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  cancel(booking: Booking): void {
    if (!confirm(`Cancel your booking for ${booking.carName}?`)) {
      return;
    }

    this.cancellingId.set(booking.id);
    this.error.set(null);

    this.bookingService.cancel(booking.id).subscribe({
      next: (updated) => {
        this.bookings.update((bookings) =>
          bookings.map((item) => (item.id === updated.id ? updated : item))
        );
        this.cancellingId.set(null);
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not cancel the booking.'));
        this.cancellingId.set(null);
      }
    });
  }

  canCancel(booking: Booking): boolean {
    return booking.status === 'Pending' || booking.status === 'Confirmed';
  }

  statusClass(status: string): string {
    return `status status-${status.toLowerCase()}`;
  }

  private load(): void {
    this.loading.set(true);

    this.bookingService.getMine().subscribe({
      next: (bookings) => {
        this.bookings.set(bookings);
        this.loading.set(false);
      },
      error: (error: unknown) => {
        this.error.set(getErrorMessage(error, 'Could not load your bookings.'));
        this.loading.set(false);
      }
    });
  }
}
