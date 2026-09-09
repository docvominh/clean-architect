# Repository integration tests

Storage tests use a separate disposable Azurite Testcontainer with dynamically assigned ports. The fixture creates the
`product` blob container, and tests call `AzureBlobService` through `IStorageService`. They verify uploaded bytes,
content types, filename extensions, distinct blobs for repeated filenames, and cancellation without a new blob. The
container is disposed after the test class; no development storage connection or volume is used.

Run only storage tests:

```powershell
dotnet test CleanArchitect.Infrastructure.IntegrationTests/CleanArchitect.Infrastructure.IntegrationTests.csproj --filter FullyQualifiedName~AzureBlobServiceTests
```

Requires Docker with Linux containers. Run from the solution directory:

```powershell
dotnet test CleanArchitect.Infrastructure.IntegrationTests/CleanArchitect.Infrastructure.IntegrationTests.csproj
```

The xUnit collection shares one disposable SQL Server 2022 Testcontainer, using a dynamically assigned port. The fixture
runs the real DbUp migrations in `CleanArchitect.Database` against a dedicated container database. It never reads the
application's development connection string. The first run downloads the container image.

Tests run serially within the collection and delete test data before each test. Verification uses fresh contexts so
tracked entities cannot hide persistence failures. Testcontainers disposes the container after the suite.

Coverage includes every implemented operation on `ProductRepository`, `UserRepository`, and `RefreshTokenRepository`,
plus SQL Server foreign keys, unique token values, and the filtered default-address index. `UserRepository.Add`
participates in the shared unit of work, so its tests explicitly save the context as the registration flow does.

Use xUnit for discovery and fixtures, and Shouldly for all assertions, including asynchronous exceptions and collection
checks. Keep both successful and rejected/missing-record scenarios. These tests use real repositories and SQL Server, so
they do not need Moq. Audit-update verification uses a persisted historical timestamp to avoid sleeps or
clock-resolution assumptions.

`OrderRepository` and `IOrderRepository` currently have no methods; add tests when their behavior is implemented.
