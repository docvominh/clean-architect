import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, map } from 'rxjs';
import { OrderRequest, OrderResponse, OrdersResponse } from './order.models';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class OrderService {
    private readonly http = inject(HttpClient);

    create(request: OrderRequest): Promise<OrderResponse> {
        return firstValueFrom(this.http.post<OrderResponse>(apiUrl('/api/orders'), request));
    }

    getMine(): Promise<OrderResponse[]> {
        return firstValueFrom(
            this.http.get<OrdersResponse>(apiUrl('/api/orders/mine')).pipe(map(response => response.orders)),
        );
    }

    getAll(): Promise<OrderResponse[]> {
        return firstValueFrom(this.http.get<OrdersResponse>(apiUrl('/api/orders')).pipe(map(response => response.orders)));
    }

    updateStatus(orderId: string, status: string): Promise<OrderResponse> {
        return firstValueFrom(this.http.put<OrderResponse>(apiUrl(`/api/orders/${orderId}/status`), { status }));
    }
}
