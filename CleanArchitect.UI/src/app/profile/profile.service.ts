import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Profile, UpdateProfileRequest } from './profile.models';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class ProfileService {
    private readonly http = inject(HttpClient);

    getProfile(): Promise<Profile> {
        return firstValueFrom(this.http.get<Profile>(apiUrl('/api/auth/profile')));
    }

    updateProfile(request: UpdateProfileRequest): Promise<Profile> {
        return firstValueFrom(this.http.put<Profile>(apiUrl('/api/auth/profile'), request));
    }
}
