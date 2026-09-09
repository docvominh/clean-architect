import { Component, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from './order.service';
import { OrderResponse } from './order.models';

@Component({
    selector: 'app-order-history',
    standalone: true,
    imports: [DecimalPipe, DatePipe, RouterLink],
    templateUrl: './order-history.component.html',
})
export class OrderHistoryComponent {
    private readonly orderService = inject(OrderService);
    readonly orders = signal<OrderResponse[]>([]);
    readonly loading = signal(true);
    readonly error = signal('');

    constructor() {
        this.load();
    }

    private async load(): Promise<void> {
        this.loading.set(true);
        this.error.set('');
        try {
            this.orders.set(await this.orderService.getMine());
        } catch {
            this.error.set('Unable to load your orders. Please try again.');
        } finally {
            this.loading.set(false);
        }
    }
}
