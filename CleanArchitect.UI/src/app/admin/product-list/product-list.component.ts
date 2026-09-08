import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../product.service';
import { Product } from '../product.models';

@Component({
    selector: 'app-product-list',
    standalone: true,
    imports: [RouterLink, DecimalPipe],
    templateUrl: './product-list.component.html',
})
export class ProductListComponent {
    private readonly productService = inject(ProductService);
    readonly products = signal<Product[]>([]);
    readonly loading = signal(true);
    readonly error = signal('');

    constructor() {
        this.load();
    }

    private async load(): Promise<void> {
        this.loading.set(true);
        this.error.set('');
        try {
            this.products.set(await this.productService.list());
        } catch {
            this.error.set('Unable to load products. Please try again.');
        } finally {
            this.loading.set(false);
        }
    }
}
