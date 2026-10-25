export type BookingStatus = 'Pending' | 'Confirmed' | 'Completed' | 'Cancelled';

export interface Booking {
  id: string;
  userId: string;
  customerName: string;
  customerEmail: string;
  carId: string;
  carName: string;
  licensePlate: string;
  imageUrl?: string | null;
  startDate: string;
  endDate: string;
  rentalDays: number;
  dailyRate: number;
  totalPrice: number;
  status: BookingStatus;
  createdAtUtc: string;
}

export interface CreateBookingRequest {
  carId: string;
  startDate: string;
  endDate: string;
}
