export interface AuthResponse {
  id: string;
  fullName: string;
  email: string;
  role: 'Admin' | 'Customer';
  token: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}
