import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { Booking, CreateBookingRequest } from '../models/booking.model';
import { CarAvailability } from '../models/car.model';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly http = inject(HttpClient);

  create(request: CreateBookingRequest) {
    return this.http.post<Booking>(`${API_BASE_URL}/bookings`, request);
  }

  getMine() {
    return this.http.get<Booking[]>(`${API_BASE_URL}/bookings/mine`);
  }

  getAll() {
    return this.http.get<Booking[]>(`${API_BASE_URL}/bookings`);
  }

  cancel(id: string) {
    return this.http.post<Booking>(`${API_BASE_URL}/bookings/${id}/cancel`, {});
  }

  updateStatus(id: string, status: string) {
    return this.http.patch<Booking>(`${API_BASE_URL}/bookings/${id}/status`, { status });
  }

  checkAvailability(carId: string, startDate: string, endDate: string) {
    const params = new HttpParams()
      .set('carId', carId)
      .set('startDate', startDate)
      .set('endDate', endDate);

    return this.http.get<CarAvailability>(`${API_BASE_URL}/bookings/availability`, { params });
  }
}
