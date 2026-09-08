import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Product, ProductRequest } from './product.models';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class ProductService {
    private readonly http = inject(HttpClient);

    list(): Promise<Product[]> {
        return firstValueFrom(this.http.get<Product[]>(apiUrl('/api/products')));
    }

    getById(id: string): Promise<Product> {
        return firstValueFrom(this.http.get<Product>(apiUrl(`/api/products/${id}`)));
    }

    create(request: ProductRequest): Promise<Product> {
        return firstValueFrom(this.http.post<Product>(apiUrl('/api/products'), request));
    }

    update(id: string, request: ProductRequest): Promise<Product> {
        return firstValueFrom(this.http.put<Product>(apiUrl(`/api/products/${id}`), request));
    }
}
