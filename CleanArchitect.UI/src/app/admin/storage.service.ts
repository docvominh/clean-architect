import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { apiUrl } from '../user/api-url';

@Injectable({ providedIn: 'root' })
export class StorageService {
    private readonly http = inject(HttpClient);

    async upload(file: File): Promise<string> {
        const formData = new FormData();
        formData.append('file', file);
        const response = await firstValueFrom(this.http.post<{ url: string }>(apiUrl('/api/storage/upload'), formData));
        return response.url;
    }
}
