using CleanArchitect.Application.UserAggregate;

namespace CleanArchitect.Api.Auth;

public sealed class RefreshTokenCookie(IHttpContextAccessor accessor) : IRefreshTokenCookie
{
    private const string CookieName = "refreshToken";
    private const string CookiePath = "/api/auth";

    private HttpContext Context => accessor.HttpContext!;

    public string? Read() => Context.Request.Cookies[CookieName];

    public void Write(string token, DateTimeOffset expiresAt)
    {
        Context.Response.Cookies.Append(
            CookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = CookiePath,
                Expires = expiresAt
            }
        );
    }

    public void Clear()
    {
        Context.Response.Cookies.Delete(
            CookieName,
            new CookieOptions
            {
                Path = CookiePath
            }
        );
    }
}
