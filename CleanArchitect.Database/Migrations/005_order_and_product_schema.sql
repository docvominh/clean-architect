-- RefreshToken now inherits BaseEntity: Id becomes a Guid (was an int identity), and the
-- audit columns become CreateBy/UpdateBy (Guid) instead of CreatedBy/ModifiedBy (string).
-- No production data depends on this table yet, so it is dropped and recreated rather than altered.

DROP TABLE [RefreshTokens];

CREATE TABLE [RefreshTokens]
(
    [Id]              uniqueidentifier NOT NULL,
    [UserId]          uniqueidentifier NOT NULL,
    [Token]           nvarchar(512)    NOT NULL,
    [ExpiresAt]       datetimeoffset   NOT NULL,
    [RevokedAt]       datetimeoffset   NULL,
    [ReplacedByToken] nvarchar(512)    NULL,
    [CreateBy]        uniqueidentifier NOT NULL,
    [CreatedAt]       datetimeoffset   NOT NULL,
    [UpdateBy]        uniqueidentifier NOT NULL,
    [ModifiedAt]      datetimeoffset   NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
