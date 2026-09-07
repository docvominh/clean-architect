import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [FormsModule, RouterLink],
    templateUrl: './login.component.html',
})
export class LoginComponent {
    private readonly auth = inject(AuthService);
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    readonly returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    email = '';
    password = '';
    readonly busy = signal(false);
    readonly error = signal('');

    async submit(): Promise<void> {
        if (this.busy()) return;
        this.busy.set(true);
        this.error.set('');
        try {
            await this.auth.login({ email: this.email, password: this.password });
            const destination = this.returnUrl.startsWith('/') && !this.returnUrl.startsWith('//') && !this.returnUrl.includes('\\') ? this.returnUrl : '/';
            await this.router.navigateByUrl(destination);
        } catch (error) {
            const body = error instanceof HttpErrorResponse ? error.error : null;
            const messages = body?.errors
                ? Object.values(body.errors).flat().filter(value => typeof value === 'string')
                : [];
            this.error.set(messages.join(' ') || (error instanceof HttpErrorResponse && error.status === 401 ? 'Incorrect email or password.' : 'Unable to sign in. Please try again.'));
        } finally {
            this.busy.set(false);
        }
    }
}
