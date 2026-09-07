namespace CleanArchitect.Application.UserAggregate;

public interface IRefreshTokenCookie
{
    string? Read();
    void Write(string token, DateTimeOffset expiresAt);
    void Clear();
}
