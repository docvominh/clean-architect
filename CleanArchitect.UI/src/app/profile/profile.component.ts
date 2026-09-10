import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ProfileService } from './profile.service';
import { Profile } from './profile.models';
import { OrderHistoryComponent } from '../order/order-history.component';

interface AddressRow {
    readonly id: number;
    country: string;
    state?: string;
    city: string;
    street: string;
    contactPhoneNumber: string;
}

@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [FormsModule, OrderHistoryComponent],
    templateUrl: './profile.component.html',
})
export class ProfileComponent {
    private readonly profileService = inject(ProfileService);

    email = '';
    displayName = '';
    addresses: AddressRow[] = [];
    defaultAddressId: number | null = null;
    private nextAddressId = 0;

    readonly loading = signal(true);
    readonly busy = signal(false);
    readonly error = signal('');
    readonly saved = signal(false);

    constructor() {
        this.load();
    }

    addAddress(): void {
        const row: AddressRow = { id: this.nextAddressId++, country: '', state: '', city: '', street: '', contactPhoneNumber: '' };
        this.addresses.push(row);
        if (this.defaultAddressId === null) this.defaultAddressId = row.id;
    }

    removeAddress(id: number): void {
        this.addresses = this.addresses.filter(address => address.id !== id);
        if (this.defaultAddressId === id) this.defaultAddressId = this.addresses[0]?.id ?? null;
    }

    async submit(): Promise<void> {
        if (this.busy()) return;
        this.busy.set(true);
        this.error.set('');
        this.saved.set(false);
        try {
            const addresses = this.addresses.map(({ id, ...address }) => ({ ...address, isDefault: id === this.defaultAddressId }));
            const profile = await this.profileService.updateProfile({ displayName: this.displayName || undefined, addresses });
            this.applyProfile(profile);
            this.saved.set(true);
        } catch (error) {
            const body = error instanceof HttpErrorResponse ? error.error : null;
            const messages = body?.errors
                ? Object.values(body.errors)
                      .flat()
                      .filter(value => typeof value === 'string')
                : [];
            this.error.set(messages.join(' ') || 'Unable to update your profile. Please try again.');
        } finally {
            this.busy.set(false);
        }
    }

    private async load(): Promise<void> {
        this.loading.set(true);
        this.error.set('');
        try {
            this.applyProfile(await this.profileService.getProfile());
        } catch {
            this.error.set('Unable to load your profile. Please try again.');
        } finally {
            this.loading.set(false);
        }
    }

    private applyProfile(profile: Profile): void {
        this.email = profile.email ?? '';
        this.displayName = profile.displayName ?? '';
        this.defaultAddressId = null;
        this.addresses = profile.addresses.map(address => {
            const row: AddressRow = {
                id: this.nextAddressId++,
                country: address.country,
                state: address.state,
                city: address.city,
                street: address.street,
                contactPhoneNumber: address.contactPhoneNumber,
            };
            if (address.isDefault) this.defaultAddressId = row.id;
            return row;
        });
    }
}
