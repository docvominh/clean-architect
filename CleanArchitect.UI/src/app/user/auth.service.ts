import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpBackend, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom, timeout } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest } from './auth.models';
import { decodeJwt } from './jwt';
import { apiUrl } from './api-url';

@Injectable({ providedIn: 'root' })
export class AuthService {
    // Bypass the auth interceptor to prevent recursive refresh requests.
    private readonly http = new HttpClient(inject(HttpBackend));
    private readonly session = signal<AuthResponse | null>(null);
    readonly user = computed(() => decodeJwt(this.session()?.accessToken ?? ''));
    readonly isAuthenticated = computed(() => this.user() !== null);
    readonly isInitializing = signal(true);
    private initialization?: Promise<void>;
    private refreshing?: Promise<void>;
    private generation = 0;

    getAccessToken(): string | null {
        return this.session()?.accessToken ?? null;
    }

    initialize(): Promise<void> {
        return (this.initialization ??= this.refresh()
            .catch(() => {})
            .finally(() => this.isInitializing.set(false)));
    }

    private async authenticate(path: string, request: LoginRequest | RegisterRequest): Promise<void> {
        await this.initialize();
        const generation = ++this.generation;
        const response = await firstValueFrom(
            this.http.post<AuthResponse>(apiUrl('/api/auth/' + path), request, { withCredentials: true }).pipe(timeout(15000)),
        );
        if (generation === this.generation) this.accept(response);
    }

    login(request: LoginRequest): Promise<void> {
        return this.authenticate('login', request);
    }
    register(request: RegisterRequest): Promise<void> {
        return this.authenticate('register', request);
    }

    private accept(response: AuthResponse): void {
        const expires = Date.parse(response.expiresAt);
        if ((decodeJwt(response.accessToken)?.exp ?? 0) * 1000 <= Date.now() || !Number.isFinite(expires) || expires <= Date.now()) {
            throw new Error('The server returned an invalid authentication response.');
        }
        this.session.set(response);
    }

    refresh(): Promise<void> {
        if (!this.refreshing) {
            const generation = this.generation;
            this.refreshing = firstValueFrom(this.http.post<AuthResponse>(apiUrl('/api/auth/refresh'), {}, { withCredentials: true }).pipe(timeout(15000)))
                .then(response => {
                    if (generation === this.generation) this.accept(response);
                })
                .catch(error => {
                    if (generation === this.generation && error instanceof HttpErrorResponse && error.status === 401) {
                        this.session.set(null);
                    }
                    throw error;
                })
                .finally(() => {
                    this.refreshing = undefined;
                });
        }
        return this.refreshing;
    }

    async getValidAccessToken(): Promise<string | null> {
        await this.initialize();
        const session = this.session();
        if (session && Date.parse(session.expiresAt) <= Date.now() + 30000) await this.refresh();
        return this.getAccessToken();
    }

    async logout(): Promise<void> {
        try {
            const token = await this.getValidAccessToken();
            await firstValueFrom(
                this.http
                    .post<void>(
                        apiUrl('/api/auth/revoke'),
                        {},
                        {
                            withCredentials: true,
                            headers: token ? { Authorization: 'Bearer ' + token } : {},
                        },
                    )
                    .pipe(timeout(15000)),
            );
        } finally {
            ++this.generation;
            this.session.set(null);
        }
    }
}
