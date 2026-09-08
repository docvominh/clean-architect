import { environment } from '../../environments/environment';

export function apiUrl(path: string): string {
    return environment.apiBaseUrl.replace(/\/+$/, '') + path;
}

export function resolveApiRequest(url: string): string | null {
    const candidate = url.startsWith('/api/') ? apiUrl(url) : url;
    const base = new URL(apiUrl('/api/'), window.location.origin);
    let target: URL;
    try {
        target = new URL(candidate, window.location.origin);
    } catch {
        return null;
    }
    return target.origin === base.origin && target.pathname.startsWith(base.pathname) ? candidate : null;
}
