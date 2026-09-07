import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = async (_route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);
    await auth.initialize();
    try {
        const token = await auth.getValidAccessToken();
        if (token && auth.user() && (auth.user()!.exp ?? 0) * 1000 > Date.now()) return true;
    } catch {
        /* Let the user sign in again. */
    }
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
