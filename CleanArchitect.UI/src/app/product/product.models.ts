export interface Product {
    id: string;
    name: string;
    manufacturer: string;
    imageUrl?: string;
    description?: string;
    price: number;
    priceDiscount?: number;
}

export interface ProductRequest {
    name: string;
    manufacturer: string;
    imageUrl?: string;
    description?: string;
    price: number;
    priceDiscount?: number;
}
