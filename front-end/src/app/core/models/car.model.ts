export interface Car {
  id: string;
  make: string;
  model: string;
  year: number;
  licensePlate: string;
  dailyRate: number;
  seats: number;
  transmission: string;
  fuelType: string;
  imageUrl?: string | null;
  description?: string | null;
  isAvailable: boolean;
  displayName?: string;
}

export interface CarRequest {
  make: string;
  model: string;
  year: number;
  licensePlate: string;
  dailyRate: number;
  seats: number;
  transmission: string;
  fuelType: string;
  imageUrl?: string | null;
  description?: string | null;
  isAvailable: boolean;
}

export interface CarAvailability {
  carId: string;
  startDate: string;
  endDate: string;
  isAvailable: boolean;
  estimatedTotal: number;
}
