import {HttpInterceptorFn} from '@angular/common/http';
/**
 * An HTTP interceptor that adds an Authorization header with a Bearer token to outgoing requests.
 * This interceptor checks for the presence of an access token in sessionStorage and appends it to requests targeting the API.
 * If no token is found or the request is not directed to the API, the request proceeds without modification.   
 */
export const authInterceptor: HttpInterceptorFn =
  (request, next) => {
    const token =
      sessionStorage.getItem(
        'access_token'
      );
    const isApiRequest =
      request.url.startsWith('/api/');

    if (!token || !isApiRequest) {
      return next(request);
    }
    // Clone the request and set the Authorization header with the Bearer token.
    const authenticatedRequest =
      request.clone({
        setHeaders: {
          Authorization:
            `Bearer ${token}`
        }
      });

    return next(
      authenticatedRequest
    );
  };