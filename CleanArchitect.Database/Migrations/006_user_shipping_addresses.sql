-- Saved shipping addresses for AspNetUsers, used to auto-fill shipping address at order time.
-- A new table rather than editing 001_init_schema.sql, since that migration has already run
-- wherever the project has been deployed and DbUp tracks scripts as run-once by filename.

CREATE TABLE [UserAddresses]
(
    [Id]                 uniqueidentifier NOT NULL,
    [UserId]             uniqueidentifier NOT NULL,
    [IsDefault]          bit              NOT NULL,
    [Country]            nvarchar(100)    NOT NULL,
    [State]              nvarchar(100)    NULL,
    [City]               nvarchar(100)    NOT NULL,
    [Street]             nvarchar(256)    NOT NULL,
    [ContactPhoneNumber] nvarchar(32)     NOT NULL,
    [CreateBy]           uniqueidentifier NOT NULL,
    [CreatedAt]          datetimeoffset   NOT NULL,
    [UpdateBy]           uniqueidentifier NOT NULL,
    [ModifiedAt]         datetimeoffset   NOT NULL,
    CONSTRAINT [PK_UserAddresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserAddresses_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_UserAddresses_UserId] ON [UserAddresses] ([UserId]);

-- At most one default address per user
CREATE UNIQUE INDEX [IX_UserAddresses_UserId_IsDefault] ON [UserAddresses] ([UserId]) WHERE [IsDefault] = 1;
