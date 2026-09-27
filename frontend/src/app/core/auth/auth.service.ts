import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, finalize, of, tap } from 'rxjs';

import {AuthResponse,LoginRequest, Trainer} from './auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl = '/api/auth';
  private readonly tokenKey = 'access_token';
 
  private readonly trainerState = signal<Trainer | null>(null);

  readonly trainer = this.trainerState.asReadonly();

  constructor(private readonly http: HttpClient) {}

/**
   * Logs in a user with the provided credentials.
   * @param request The login request containing email and password.
   * @returns An observable of the authentication response.
   */ 
  login(request: LoginRequest): Observable<AuthResponse> {

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response => {
          sessionStorage.setItem(
            this.tokenKey,
            response.accessToken
          );
          this.trainerState.set(
            response.trainer
          );
        })
      );
  }
 // Loads the current trainer's information if an access token is present.
  loadCurrentTrainer(): Observable<Trainer | null> {
    // If there is no access token, clear the trainer state and return null.
    if (!this.getAccessToken()) {
      this.trainerState.set(null);
      return of(null);
    }
    return this.http
      .get<Trainer>(
        `${this.apiUrl}/me`
      )
      .pipe(
        tap(trainer =>
          this.trainerState.set(trainer)
        ),

        catchError(() => {

          this.clearSession();

          return of(null);
        })
      );
  }

  /**
   * Logs out the current user by clearing the session and making a logout request to the server.
   * @returns An observable indicating the completion of the logout process.
   */
  logout(): Observable<void> {

    if (!this.getAccessToken()) {

      this.clearSession();

      return of(undefined);
    }

    return this.http
      .post<void>(
        `${this.apiUrl}/logout`,
        {}
      )
      .pipe(
        finalize(() =>
          this.clearSession()
        )
      );
  }
  getAccessToken(): string | null {
    return sessionStorage.getItem(
      this.tokenKey
    );
  }

  hasAccessToken(): boolean {
    return this.getAccessToken() !== null;
  }
  clearSession(): void {
    sessionStorage.removeItem(
      this.tokenKey
    );
    this.trainerState.set(null);
  }
}