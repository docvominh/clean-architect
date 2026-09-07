import { apiUrl } from './api-url';
import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { authInterceptor } from './auth.interceptor';

describe('Angular authentication', () => {
    let auth: AuthService;
    let http: HttpTestingController;
    const response = () => ({
        accessToken: 'header.' + btoa(JSON.stringify({ exp: Math.floor(Date.now() / 1000) + 3600 })) + '.signature',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
    });

    beforeEach(() => {
        TestBed.configureTestingModule({ providers: [provideHttpClient(withInterceptors([authInterceptor])), provideHttpClientTesting()] });
        auth = TestBed.inject(AuthService);
        http = TestBed.inject(HttpTestingController);
    });
    afterEach(() => http.verify());

    it('restores a session from the cookie and shares concurrent refreshes', async () => {
        const initialized = auth.initialize();
        const refresh = auth.refresh();
        const request = http.expectOne(apiUrl('/api/auth/refresh'));
        expect(request.request.withCredentials).toBeTrue();
        expect(request.request.headers.has('Authorization')).toBeFalse();
        request.flush(response());
        await Promise.all([initialized, refresh]);
        expect(auth.isAuthenticated()).toBeTrue();
        expect(auth.isInitializing()).toBeFalse();
    });

    it('finishes initialization when there is no refresh cookie', async () => {
        const initialized = auth.initialize();
        http.expectOne(apiUrl('/api/auth/refresh')).flush(null, { status: 401, statusText: 'Unauthorized' });
        await initialized;
        expect(auth.getAccessToken()).toBeNull();
        expect(auth.isInitializing()).toBeFalse();
    });

    it('clears local state even if revocation fails', async () => {
        const initialized = auth.initialize();
        http.expectOne(apiUrl('/api/auth/refresh')).flush(response());
        await initialized;
        const logout = auth.logout();
        const rejected = expectAsync(logout).toBeRejected();
        await Promise.resolve();
        await Promise.resolve();
        const request = http.expectOne(apiUrl('/api/auth/revoke'));
        expect(request.request.headers.get('Authorization')).toContain('Bearer ');
        expect(request.request.withCredentials).toBeTrue();
        request.flush(null, { status: 500, statusText: 'Server error' });
        await rejected;
        expect(auth.getAccessToken()).toBeNull();
    });

    it('does not attach credentials to a third-party request', () => {
        TestBed.inject(HttpClient).get('https://example.com/data').subscribe();
        const request = http.expectOne('https://example.com/data');
        expect(request.request.headers.has('Authorization')).toBeFalse();
        expect(request.request.withCredentials).toBeFalse();
        request.flush({});
    });
});
