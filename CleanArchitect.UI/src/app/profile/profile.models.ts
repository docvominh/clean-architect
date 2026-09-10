export interface ProfileAddress {
    id: string;
    country: string;
    state?: string;
    city: string;
    street: string;
    contactPhoneNumber: string;
    isDefault: boolean;
}

export interface Profile {
    email?: string;
    displayName?: string;
    addresses: ProfileAddress[];
}

export interface UpdateProfileAddressRequest {
    country: string;
    state?: string;
    city: string;
    street: string;
    contactPhoneNumber: string;
    isDefault: boolean;
}

export interface UpdateProfileRequest {
    displayName?: string;
    addresses: UpdateProfileAddressRequest[];
}
