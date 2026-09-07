import { environment } from '../../environments/environment';
import { apiUrl, resolveApiRequest } from './api-url';

describe('API URL configuration', () => {
    const original = environment.apiBaseUrl;
    beforeEach(() => { environment.apiBaseUrl = 'https://localhost:7189/'; });
    afterEach(() => { environment.apiBaseUrl = original; });

    it('joins the configured base without duplicate slashes', () => {
        expect(apiUrl('/api/auth/login')).toBe('https://localhost:7189/api/auth/login');
        expect(resolveApiRequest('/api/users?skip=1')).toBe('https://localhost:7189/api/users?skip=1');
    });
    it('accepts absolute requests only inside the configured API', () => {
        expect(resolveApiRequest('https://localhost:7189/api/users')).toBe('https://localhost:7189/api/users');
        expect(resolveApiRequest('https://localhost:7189.evil.test/api/users')).toBeNull();
        expect(resolveApiRequest('https://localhost.evil.test:7189/api/users')).toBeNull();
        expect(resolveApiRequest('https://localhost:7190/api/users')).toBeNull();
        expect(resolveApiRequest('https://localhost:7189/other')).toBeNull();
    });
    it('supports same-origin deployments with an empty base', () => {
        environment.apiBaseUrl = '';
        expect(apiUrl('/api/auth/login')).toBe('/api/auth/login');
        expect(resolveApiRequest('/api/users')).toBe('/api/users');
    });
});
