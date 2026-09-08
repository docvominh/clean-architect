-- Business/domain schema, on top of the ASP.NET Identity tables from 001_init_schema.sql.
-- Consolidated into one file since this database has not been deployed anywhere yet.

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

-- Business "User" profile, separate from AspNetUsers (the ASP.NET Identity table).
-- Shares its primary key with AspNetUsers (one-to-one) so Domain/Application never
-- need to reference the Identity model directly.
CREATE TABLE [Users]
(
    [Id]          uniqueidentifier NOT NULL,
    [DisplayName] nvarchar(256)    NULL,
    [CreateBy]    uniqueidentifier NOT NULL,
    [CreatedAt]   datetimeoffset   NOT NULL,
    [UpdateBy]    uniqueidentifier NOT NULL,
    [ModifiedAt]  datetimeoffset   NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_AspNetUsers_Id] FOREIGN KEY ([Id]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

-- Saved shipping addresses, belonging to the Users aggregate. At most one default per user.
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
    CONSTRAINT [FK_UserAddresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_UserAddresses_UserId] ON [UserAddresses] ([UserId]);
CREATE UNIQUE INDEX [IX_UserAddresses_UserId_IsDefault] ON [UserAddresses] ([UserId]) WHERE [IsDefault] = 1;

CREATE TABLE [Products]
(
    [Id]            uniqueidentifier NOT NULL,
    [ImageUrl]      nvarchar(2048)   NULL,
    [Name]          nvarchar(256)    NOT NULL,
    [Manufacturer]  nvarchar(256)    NOT NULL,
    [Description]   nvarchar(max)    NULL,
    [Price]         decimal(18, 2)   NOT NULL,
    [PriceDiscount] decimal(18, 2)   NULL,
    [CreateBy]      uniqueidentifier NOT NULL,
    [CreatedAt]     datetimeoffset   NOT NULL,
    [UpdateBy]      uniqueidentifier NOT NULL,
    [ModifiedAt]    datetimeoffset   NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

-- Shipping address is flattened directly onto Orders (no separate value-object table).
CREATE TABLE [Orders]
(
    [Id]                         uniqueidentifier NOT NULL,
    [ShippingCountry]            nvarchar(100)    NOT NULL,
    [ShippingState]              nvarchar(100)    NULL,
    [ShippingCity]               nvarchar(100)    NOT NULL,
    [ShippingStreet]             nvarchar(256)    NOT NULL,
    [ShippingContactPhoneNumber] nvarchar(32)     NOT NULL,
    [Status]                     nvarchar(32)     NOT NULL,
    [TotalAmount]                decimal(18, 2)   NOT NULL,
    [CreateBy]                   uniqueidentifier NOT NULL,
    [CreatedAt]                  datetimeoffset   NOT NULL,
    [UpdateBy]                   uniqueidentifier NOT NULL,
    [ModifiedAt]                 datetimeoffset   NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [OrderProducts]
(
    [Id]         uniqueidentifier NOT NULL,
    [OrderId]    uniqueidentifier NOT NULL,
    [ProductId]  uniqueidentifier NOT NULL,
    [Quantity]   int              NOT NULL,
    [UnitPrice]  decimal(18, 2)   NOT NULL,
    [CreateBy]   uniqueidentifier NOT NULL,
    [CreatedAt]  datetimeoffset   NOT NULL,
    [UpdateBy]   uniqueidentifier NOT NULL,
    [ModifiedAt] datetimeoffset   NOT NULL,
    CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderProducts_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);

CREATE INDEX [IX_OrderProducts_OrderId_ProductId] ON [OrderProducts] ([OrderId], [ProductId]);
CREATE INDEX [IX_OrderProducts_ProductId] ON [OrderProducts] ([ProductId]);
