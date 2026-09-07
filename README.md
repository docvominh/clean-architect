# Clean Architect

## Local HTTPS setup

The Angular UI uses HTTPS so the browser can send the API's secure HttpOnly refresh
cookie. Use the .NET SDK's development certificate tool; `mkcert` is not required.

### 1. Generate and trust the certificate

Run these commands in PowerShell from the repository root:

```powershell
cd CleanArchitect.UI
New-Item -ItemType Directory -Force .cert
dotnet dev-certs https --trust --format PEM --no-password --export-path .cert/localhost.pem
```

Accept the Windows certificate trust prompt. This exports the certificate to
`.cert/localhost.pem` and the private key to `.cert/localhost.key`.

Angular is configured to use these filenames directly. The `.cert` directory is
excluded from Git; keep these development certificates and private keys local.


### 2. Start the API

In a separate terminal, from the repository root:

```powershell
dotnet run --project CleanArchitect.Api --launch-profile https
```

The API listens on `https://localhost:7189`. Ensure its database and connection
settings are configured before using authentication.

### 3. Start the Angular UI

From `CleanArchitect.UI`:

```powershell
npm ci
npm start
```

Open **https://localhost:4200**. API requests go directly to the API URL in
`CleanArchitect.UI/src/environments/environment.development.ts`.
Angular selects the development environment for `npm start` and the production
environment for `npm run build`. Update `apiBaseUrl` in the corresponding file.
The API must allow the UI origin with credentials in its CORS policy; local HTTPS
is configured as `https://localhost:4200`. Restart the API after changing CORS.

If the browser still reports an untrusted certificate, restart it after accepting
the trust prompt. See [Angular authentication setup](CleanArchitect.UI/AUTH.md)
for the login, registration, refresh, and logout flow, and the
[.NET certificate documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs)
for certificate export details.
