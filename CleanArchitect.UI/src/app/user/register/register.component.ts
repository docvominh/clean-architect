import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth.service';
import { ShippingAddress } from '../auth.models';

interface AddressRow extends ShippingAddress {
    readonly id: number;
}

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [FormsModule, RouterLink],
    templateUrl: './register.component.html',
})
export class RegisterComponent {
    private readonly auth = inject(AuthService);
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    readonly returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    email = '';
    password = '';
    confirmPassword = '';
    displayName = '';
    addresses: AddressRow[] = [];
    private nextAddressId = 0;
    readonly busy = signal(false);
    readonly error = signal('');

    addAddress(): void {
        this.addresses.push({ id: this.nextAddressId++, country: '', state: '', city: '', street: '', contactPhoneNumber: '' });
    }

    removeAddress(id: number): void {
        this.addresses = this.addresses.filter(address => address.id !== id);
    }

    async submit(): Promise<void> {
        if (this.busy() || this.password !== this.confirmPassword) return;
        this.busy.set(true);
        this.error.set('');
        try {
            const addresses: ShippingAddress[] = this.addresses.map(({ country, state, city, street, contactPhoneNumber }) => ({
                country,
                state,
                city,
                street,
                contactPhoneNumber,
            }));
            await this.auth.register({ email: this.email, password: this.password, displayName: this.displayName, addresses });
            const destination = this.returnUrl.startsWith('/') && !this.returnUrl.startsWith('//') && !this.returnUrl.includes('\\') ? this.returnUrl : '/';
            await this.router.navigateByUrl(destination);
        } catch (error) {
            const body = error instanceof HttpErrorResponse ? error.error : null;
            const messages = body?.errors
                ? Object.values(body.errors).flat().filter(value => typeof value === 'string')
                : [];
            this.error.set(messages.join(' ') || 'Unable to create your account. Please try again.');
        } finally {
            this.busy.set(false);
        }
    }
}
