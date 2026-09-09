import { Component, inject, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ProductService } from '../product/product.service';
import { Product } from '../product/product.models';
import { CartService } from '../cart/cart.service';

@Component({
    selector: 'app-home',
    standalone: true,
    imports: [DecimalPipe],
    templateUrl: './home.component.html',
})
export class HomeComponent {
    private readonly productService = inject(ProductService);
    readonly cart = inject(CartService);
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
