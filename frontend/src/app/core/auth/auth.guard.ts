import { inject } from '@angular/core';

import {CanActivateFn,Router} from '@angular/router';

import {map} from 'rxjs';

import {AuthService} from './auth.service';

/**
 * A guard that checks if the user is authenticated before allowing access to a route.
 * @returns A boolean indicating whether the user is authenticated. If not authenticated, it redirects to the login page.
 */
export const authGuard: CanActivateFn =
  () => {
    const authService = inject(AuthService);

    const router = inject(Router);

    if (!authService.hasAccessToken()) {
      return router.createUrlTree([
        '/login'
      ]);
    }
    return authService
      .loadCurrentTrainer()
      .pipe(
        map(trainer => {
          if (trainer) {
            return true;
          }
          return router.createUrlTree([
            '/login'
          ]);

        })

      );
  };