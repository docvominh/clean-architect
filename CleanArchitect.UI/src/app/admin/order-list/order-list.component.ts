import { Component, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe, SlicePipe } from '@angular/common';
import { OrderService } from '../../order/order.service';
import { OrderResponse } from '../../order/order.models';

@Component({
    selector: 'app-admin-order-list',
    standalone: true,
    imports: [DecimalPipe, DatePipe, SlicePipe],
    templateUrl: './order-list.component.html',
})
export class AdminOrderListComponent {
    private readonly orderService = inject(OrderService);
    readonly orders = signal<OrderResponse[]>([]);
    readonly loading = signal(true);
    readonly error = signal('');
    readonly statuses = ['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'];

    constructor() {
        this.load();
    }

    private async load(): Promise<void> {
        this.loading.set(true);
        this.error.set('');
        try {
            this.orders.set(await this.orderService.getAll());
        } catch {
            this.error.set('Unable to load orders. Please try again.');
        } finally {
            this.loading.set(false);
        }
    }

    async changeStatus(order: OrderResponse, status: string): Promise<void> {
        this.error.set('');
        try {
            const updated = await this.orderService.updateStatus(order.id, status);
            this.orders.set(this.orders().map(o => (o.id === updated.id ? updated : o)));
        } catch {
            this.error.set(`Unable to update status for order "${order.id}". Please try again.`);
        }
    }
}
