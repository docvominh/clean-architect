export interface LoginRequest {
    email: string;
    password: string;
}
export interface ShippingAddress {
    country: string;
    state?: string;
    city: string;
    street: string;
    contactPhoneNumber: string;
}
export interface RegisterRequest extends LoginRequest {
    displayName?: string;
    addresses?: ShippingAddress[];
}
export interface AuthResponse {
    accessToken: string;
    expiresAt: string;
}
