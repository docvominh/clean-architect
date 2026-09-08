using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

// The refresh token itself is never passed in from the API layer - the handler reads it
// straight from the request cookie via IRefreshTokenCookie.
public class RefreshTokenCommand : IRequest<AuthResponse>;
