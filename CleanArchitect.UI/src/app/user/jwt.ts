export interface JwtClaims {
    name?: string;
    email?: string;
    roles: string[];
    exp?: number;
}

const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

export function decodeJwt(token: string): JwtClaims | null {
    const parts = token.split('.');

    if (parts.length !== 3) {
        return null;
    }

    try {
        const payload = JSON.parse(base64UrlDecode(parts[1])) as Record<string, unknown>;
        const rawRole = payload[ROLE_CLAIM];

        return {
            name: typeof payload[NAME_CLAIM] === 'string' ? payload[NAME_CLAIM] : undefined,
            email: typeof payload[EMAIL_CLAIM] === 'string' ? payload[EMAIL_CLAIM] : undefined,
            roles: Array.isArray(rawRole) ? rawRole.filter((r): r is string => typeof r === 'string') : typeof rawRole === 'string' ? [rawRole] : [],
            exp: typeof payload['exp'] === 'number' ? payload['exp'] : undefined,
        };
    } catch {
        return null;
    }
}

function base64UrlDecode(value: string): string {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
    const padded = normalized.padEnd(normalized.length + ((4 - (normalized.length % 4)) % 4), '=');

    return decodeURIComponent(
        atob(padded)
            .split('')
            .map(c => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
            .join(''),
    );
}
