import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const userGuard: CanActivateFn = async () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    await auth.initialize();
    if (auth.user()?.roles?.includes('User')) return true;
    return router.createUrlTree(['/']);
};
