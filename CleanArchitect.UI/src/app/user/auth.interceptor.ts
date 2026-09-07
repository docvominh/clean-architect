import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { resolveApiRequest } from './api-url';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
    // Never forward credentials to third-party URLs.
    const url = resolveApiRequest(request.url);
    if (url === null) return next(request);
    const auth = inject(AuthService);
    const send = (token: string | null) =>
        next(
            request.clone({
                url,
                withCredentials: true,
                setHeaders: token ? { Authorization: 'Bearer ' + token } : {},
            }),
        );
    return from(auth.getValidAccessToken()).pipe(
        switchMap(token =>
            send(token).pipe(
                catchError(error => {
                    if (!(error instanceof HttpErrorResponse) || error.status !== 401 || !token) {
                        return throwError(() => error);
                    }
                    // A concurrent request may already have refreshed the session.
                    const refresh = auth.getAccessToken() !== token ? Promise.resolve() : auth.refresh();
                    return from(refresh).pipe(switchMap(() => send(auth.getAccessToken())));
                }),
            ),
        ),
    );
};
