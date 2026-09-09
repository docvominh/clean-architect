import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, map } from 'rxjs';
import { Product, ProductRequest, ProductsResponse } from './product.models';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class ProductService {
    private readonly http = inject(HttpClient);

    list(): Promise<Product[]> {
        return firstValueFrom(
            this.http.get<ProductsResponse>(apiUrl('/api/products')).pipe(map(response => response.products)),
        );
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

    remove(id: string): Promise<void> {
        return firstValueFrom(this.http.delete<void>(apiUrl(`/api/products/${id}`)));
    }
}
