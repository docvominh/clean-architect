import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
    selector: 'app-account-page',
    standalone: true,
    template: `<main class="auth-card">
        <p class="eyebrow">CLEAN ARCHITECT</p>
        <h1>Welcome, {{ auth.user()?.name || 'there' }}</h1>
        <p>{{ auth.user()?.email }}</p>
        <p>Roles: {{ auth.user()?.roles?.join(', ') || 'None' }}</p>
        <button (click)="logout()" [disabled]="busy()">Sign out</button>
        @if (error()) {
            <p class="error" role="alert">{{ error() }}</p>
        }
    </main>`,
})
export class AccountPageComponent {
    readonly auth = inject(AuthService);
    private readonly router = inject(Router);
    readonly busy = signal(false);
    readonly error = signal('');
    async logout(): Promise<void> {
        this.busy.set(true);
        try {
            await this.auth.logout();
            await this.router.navigateByUrl('/login');
        } catch {
            this.error.set('Signed out locally, but the server could not revoke your session. Reconnect and sign out again.');
        } finally {
            this.busy.set(false);
        }
    }
}
