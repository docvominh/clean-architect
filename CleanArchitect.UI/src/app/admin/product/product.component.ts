import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ProductService } from '../product.service';

@Component({
    selector: 'app-product',
    standalone: true,
    imports: [FormsModule, RouterLink],
    templateUrl: './product.component.html',
})
export class ProductComponent {
    private readonly productService = inject(ProductService);
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    readonly productId = this.route.snapshot.paramMap.get('id');
    readonly isEdit = this.productId !== null;

    name = '';
    manufacturer = '';
    price: number | null = null;
    priceDiscount: number | null = null;
    imageUrl = '';
    description = '';

    readonly loading = signal(this.isEdit);
    readonly busy = signal(false);
    readonly error = signal('');

    constructor() {
        if (this.productId) this.load(this.productId);
    }

    private async load(id: string): Promise<void> {
        try {
            const product = await this.productService.getById(id);
            this.name = product.name;
            this.manufacturer = product.manufacturer;
            this.price = product.price;
            this.priceDiscount = product.priceDiscount ?? null;
            this.imageUrl = product.imageUrl ?? '';
            this.description = product.description ?? '';
        } catch {
            this.error.set('Unable to load this product.');
        } finally {
            this.loading.set(false);
        }
    }

    async submit(): Promise<void> {
        if (this.busy()) return;
        this.busy.set(true);
        this.error.set('');
        try {
            const request = {
                name: this.name,
                manufacturer: this.manufacturer,
                price: this.price ?? 0,
                priceDiscount: this.priceDiscount ?? undefined,
                imageUrl: this.imageUrl || undefined,
                description: this.description || undefined,
            };
            if (this.productId) await this.productService.update(this.productId, request);
            else await this.productService.create(request);
            await this.router.navigateByUrl('/admin/product');
        } catch (error) {
            const body = error instanceof HttpErrorResponse ? error.error : null;
            const messages = body?.errors
                ? Object.values(body.errors).flat().filter(value => typeof value === 'string')
                : [];
            this.error.set(messages.join(' ') || 'Unable to save this product. Please try again.');
        } finally {
            this.busy.set(false);
        }
    }
}
