import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../user/auth.service';

export const adminGuard: CanActivateFn = async () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    await auth.initialize();
    if (auth.user()?.roles?.includes('Admin')) return true;
    return router.createUrlTree(['/']);
};
