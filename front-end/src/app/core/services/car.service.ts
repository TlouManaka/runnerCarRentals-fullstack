import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../api.config';
import { Car, CarRequest } from '../models/car.model';

@Injectable({ providedIn: 'root' })
export class CarService {
  private readonly http = inject(HttpClient);

  search(search?: string, availableFrom?: string, availableTo?: string) {
    let params = new HttpParams();

    if (search?.trim()) {
      params = params.set('search', search.trim());
    }

    if (availableFrom && availableTo) {
      params = params.set('availableFrom', availableFrom).set('availableTo', availableTo);
    }

    return this.http.get<Car[]>(`${API_BASE_URL}/cars`, { params });
  }

  getById(id: string) {
    return this.http.get<Car>(`${API_BASE_URL}/cars/${id}`);
  }

  create(request: CarRequest) {
    return this.http.post<Car>(`${API_BASE_URL}/cars`, request);
  }

  update(id: string, request: CarRequest) {
    return this.http.put<Car>(`${API_BASE_URL}/cars/${id}`, request);
  }

  delete(id: string) {
    return this.http.delete<void>(`${API_BASE_URL}/cars/${id}`);
  }
}
