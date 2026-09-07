-- Custom table (not part of ASP.NET Core Identity) used for JWT refresh token storage/rotation

CREATE TABLE [RefreshTokens]
(
    [Id]              int IDENTITY (1,1) NOT NULL,
    [UserId]          uniqueidentifier   NOT NULL,
    [Token]           nvarchar(512)      NOT NULL,
    [ExpiresAt]       datetimeoffset     NOT NULL,
    [RevokedAt]       datetimeoffset     NULL,
    [ReplacedByToken] nvarchar(512)      NULL,
    [CreatedBy]       nvarchar(50)       NULL,
    [CreatedAt]       datetimeoffset     NOT NULL,
    [ModifiedBy]      nvarchar(50)       NULL,
    [ModifiedAt]      datetimeoffset     NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);