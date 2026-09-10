# Angular authentication

The React auth flow is implemented in `src/app/auth` using an injectable signal-based
`AuthService`, a functional HTTP interceptor, and a route guard. Login and registration
use the existing API DTOs. Access tokens stay in memory; refresh tokens remain in the
API's HttpOnly cookie. Reloading attempts session restoration before routing.

## Local development

1. Start the API with its HTTPS profile:
   `dotnet run --project CleanArchitect.Api --launch-profile https` (from the repository root).
2. In `CleanArchitect.UI`, run `npm start`.
3. Open `https://localhost:4200` and accept/trust the local development certificate.

API URLs use `apiBaseUrl` from `src/environments/environment.development.ts` for
local development and `environment.production.ts` for production builds. An empty
base uses the UI origin. Browser requests include credentials for refresh cookies;
configure the API CORS policy with the exact UI origin and AllowCredentials.
Trust the API's HTTPS certificate in your browser. No development proxy is used.

## Administrator bootstrap

The API's `appsettings.Development.json` supplies the demo administrator email
and password through `BootstrapAdmin:Email` and `BootstrapAdmin:Password`.
`Program.cs` calls `SetupAdminUserAsync` only when `app.Environment.IsDevelopment()`
is true. There are no fallback credentials in code. Development permits the configured
demo password; incomplete credentials fail startup, and missing credentials create roles only.

Other environments skip this bootstrap entirely, even when bootstrap credentials
are configured. Provision the `Admin` and `User` roles and administrator accounts
separately before using authentication in those environments.

This change does not remove
previously created demo accounts: if one exists outside development, remove its
administrator access or rotate its password and revoke its existing sessions.

## Usage

- `/login` and `/register` are public; `/` is the protected account page.
- Apply `canActivate: [authGuard]` to additional private routes.
- Use Angular `HttpClient` with `/api/...` URLs for authenticated API calls.
- The interceptor resolves relative `/api/` URLs against the configured base and only attaches tokens to that API, refreshes near expiry,
  and retries a 401 once. Concurrent refresh calls share one request within the tab.
- `AuthService.user()` exposes decoded name, email, roles, and expiration for display.
  JWT decoding and route guards do not replace server-side authorization.
- Logout revokes the server token and clears memory. A revocation failure is shown
  because the server cookie may still restore the session on a later reload.

Validation: `npm run build` and `npm test -- --watch=false --browsers=ChromeHeadless`.
