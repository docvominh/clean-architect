import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { OrderRequest, OrderResponse } from './order.models';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class OrderService {
    private readonly http = inject(HttpClient);

    create(request: OrderRequest): Promise<OrderResponse> {
        return firstValueFrom(this.http.post<OrderResponse>(apiUrl('/api/orders'), request));
    }
}
