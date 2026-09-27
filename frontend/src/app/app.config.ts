import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import { authInterceptor } from './core/auth/auth.interceptor';
/**
 * The application configuration for the Angular application.
 * This configuration sets up global error listeners, routing, and HTTP client with interceptors.
 * The authInterceptor is included to handle authentication by adding the Authorization header to outgoing requests.  
 */
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
