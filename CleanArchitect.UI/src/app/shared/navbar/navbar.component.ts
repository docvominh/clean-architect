import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../user/auth.service';
import { CartService } from '../../cart/cart.service';

@Component({
    selector: 'app-navbar',
    standalone: true,
    imports: [RouterLink],
    templateUrl: './navbar.component.html',
})
export class NavbarComponent {
    readonly auth = inject(AuthService);
    readonly cart = inject(CartService);
    private readonly router = inject(Router);
    readonly menuOpen = signal(false);
    readonly userMenuOpen = signal(false);
    readonly busy = signal(false);

    toggleMenu(): void {
        this.menuOpen.update(open => !open);
    }

    toggleUserMenu(): void {
        this.userMenuOpen.update(open => !open);
    }

    async logout(): Promise<void> {
        if (this.busy()) return;
        this.busy.set(true);
        try {
            await this.auth.logout();
        } finally {
            this.busy.set(false);
            this.menuOpen.set(false);
            this.userMenuOpen.set(false);
            await this.router.navigateByUrl('/login');
        }
    }
}
