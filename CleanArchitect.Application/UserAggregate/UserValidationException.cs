namespace CleanArchitect.Application.UserAggregate;

public sealed class UserValidationException(IReadOnlyDictionary<string, string[]> errors) : Exception("User validation failed.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}

public sealed class UserAuthenticationException() : Exception("Authentication failed.");
