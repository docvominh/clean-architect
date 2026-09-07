export interface LoginRequest {
    email: string;
    password: string;
}
export interface RegisterRequest extends LoginRequest {
    displayName?: string;
}
export interface AuthResponse {
    accessToken: string;
    expiresAt: string;
}
