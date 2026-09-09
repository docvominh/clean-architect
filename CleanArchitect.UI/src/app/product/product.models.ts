export interface Product {
    id: string;
    name: string;
    manufacturer: string;
    imageUrl?: string;
    description?: string;
    price: number;
    priceDiscount?: number;
}

export interface ProductsResponse {
    products: Product[];
}

export interface ProductRequest {
    name: string;
    manufacturer: string;
    imageUrl?: string;
    description?: string;
    price: number;
    priceDiscount?: number;
}
