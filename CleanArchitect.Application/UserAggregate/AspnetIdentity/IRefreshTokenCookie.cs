namespace CleanArchitect.Application.UserAggregate.AspnetIdentity;

public interface IRefreshTokenCookie
{
    string? Read();

    void Write(string token, DateTimeOffset expiresAt);

    void Clear();
}
