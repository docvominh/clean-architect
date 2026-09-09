import { Injectable, computed, signal } from '@angular/core';
import { Product } from '../product/product.models';
import { CartItem } from './cart.models';

const STORAGE_KEY = 'cart';

@Injectable({ providedIn: 'root' })
export class CartService {
    readonly items = signal<CartItem[]>(this.restore());
    readonly count = computed(() => this.items().length);
    readonly total = computed(() => this.items().reduce((sum, item) => sum + item.unitPrice * item.quantity, 0));

    add(product: Product): void {
        const items = this.items();
        const existing = items.find(item => item.productId === product.id);
        if (existing) {
            this.persist(items.map(item => (item.productId === product.id ? { ...item, quantity: item.quantity + 1 } : item)));
        } else {
            const unitPrice = product.priceDiscount ?? product.price;
            this.persist([...items, { productId: product.id, name: product.name, imageUrl: product.imageUrl, unitPrice, quantity: 1 }]);
        }
    }

    increase(productId: string): void {
        this.persist(this.items().map(item => (item.productId === productId ? { ...item, quantity: item.quantity + 1 } : item)));
    }

    decrease(productId: string): void {
        const items = this.items();
        const existing = items.find(item => item.productId === productId);
        if (existing && existing.quantity <= 1) {
            this.remove(productId);
            return;
        }
        this.persist(items.map(item => (item.productId === productId ? { ...item, quantity: item.quantity - 1 } : item)));
    }

    setQuantity(productId: string, quantity: number): void {
        if (quantity < 1) {
            this.remove(productId);
            return;
        }
        this.persist(this.items().map(item => (item.productId === productId ? { ...item, quantity } : item)));
    }

    remove(productId: string): void {
        this.persist(this.items().filter(item => item.productId !== productId));
    }

    clear(): void {
        this.persist([]);
    }

    private persist(items: CartItem[]): void {
        this.items.set(items);
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
        } catch {
            // Ignore storage failures (e.g. private browsing quota); cart still works for this session.
        }
    }

    private restore(): CartItem[] {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            return raw ? (JSON.parse(raw) as CartItem[]) : [];
        } catch {
            return [];
        }
    }
}
