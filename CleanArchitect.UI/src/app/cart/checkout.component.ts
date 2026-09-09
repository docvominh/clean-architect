import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { CartService } from './cart.service';
import { OrderService } from './order.service';
import { OrderResponse } from './order.models';

@Component({
    selector: 'app-checkout',
    standalone: true,
    imports: [DecimalPipe, RouterLink, FormsModule],
    templateUrl: './checkout.component.html',
})
export class CheckoutComponent {
    readonly cart = inject(CartService);
    private readonly orderService = inject(OrderService);

    country = '';
    state = '';
    city = '';
    street = '';
    contactPhoneNumber = '';

    readonly busy = signal(false);
    readonly error = signal('');
    readonly placedOrder = signal<OrderResponse | null>(null);

    async submit(): Promise<void> {
        if (this.busy() || this.cart.items().length === 0) return;
        this.busy.set(true);
        this.error.set('');
        try {
            const order = await this.orderService.create({
                country: this.country,
                state: this.state || undefined,
                city: this.city,
                street: this.street,
                contactPhoneNumber: this.contactPhoneNumber,
                items: this.cart.items().map(item => ({ productId: item.productId, quantity: item.quantity })),
            });
            this.placedOrder.set(order);
            this.cart.clear();
        } catch (error) {
            const body = error instanceof HttpErrorResponse ? error.error : null;
            const messages = body?.errors
                ? Object.values(body.errors)
                      .flat()
                      .filter(value => typeof value === 'string')
                : [];
            this.error.set(messages.join(' ') || 'Unable to place this order. Please try again.');
        } finally {
            this.busy.set(false);
        }
    }
}
