import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
    selector: 'app-account-page',
    standalone: true,
    template: `<section class="section">
        <div class="container">
            <div class="columns is-centered">
                <div class="column is-half">
                    <div class="box">
                        <p class="heading">Clean Architect</p>
                        <h1 class="title">Welcome, {{ auth.user()?.name || 'there' }}</h1>
                        <p class="subtitle">{{ auth.user()?.email }}</p>
                        <div class="tags">
                            @for (role of auth.user()?.roles ?? []; track role) {
                                <span class="tag is-info">{{ role }}</span>
                            } @empty {
                                <span class="tag">No roles</span>
                            }
                        </div>
                        <div class="field">
                            <div class="control">
                                <button type="button" class="button is-danger" [disabled]="busy()" (click)="logout()">Sign out</button>
                            </div>
                        </div>
                        @if (error()) {
                            <p class="help is-danger" role="alert">{{ error() }}</p>
                        }
                    </div>
                </div>
            </div>
        </div>
    </section>`,
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
