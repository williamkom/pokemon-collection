// This file contains the TypeScript interfaces for authentication-related models.
export interface Trainer {
  id: number;
  email: string;
}
// This interface represents the structure of a login request, containing the user's email and password.
export interface LoginRequest {
  email: string;
  password: string;
}
// This interface represents the structure of an authentication response, which includes an access token,
//  its expiration time, and the authenticated trainer's information.
export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  trainer: Trainer;
}