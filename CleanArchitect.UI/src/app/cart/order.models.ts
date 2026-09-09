export interface OrderItemRequest {
    productId: string;
    quantity: number;
}

export interface OrderRequest {
    country: string;
    state?: string;
    city: string;
    street: string;
    contactPhoneNumber: string;
    items: OrderItemRequest[];
}

export interface OrderProductResponse {
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
}

export interface OrderResponse {
    id: string;
    status: string;
    totalAmount: number;
    products: OrderProductResponse[];
}
