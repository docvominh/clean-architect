import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { filter } from 'rxjs';
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
    private readonly elementRef = inject(ElementRef<HTMLElement>);
    readonly menuOpen = signal(false);
    readonly userMenuOpen = signal(false);
    readonly busy = signal(false);

    constructor() {
        this.router.events
            .pipe(
                filter(event => event instanceof NavigationEnd),
                takeUntilDestroyed(),
            )
            .subscribe(() => this.closeMenus());
    }

    @HostListener('document:click', ['$event'])
    onDocumentClick(event: MouseEvent): void {
        if (!this.elementRef.nativeElement.contains(event.target as Node)) this.closeMenus();
    }

    closeMenus(): void {
        this.menuOpen.set(false);
        this.userMenuOpen.set(false);
    }

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
            this.closeMenus();
            await this.router.navigateByUrl('/login');
        }
    }
}
