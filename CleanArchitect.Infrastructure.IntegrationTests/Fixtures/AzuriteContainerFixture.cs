using Azure.Storage.Blobs;

using Testcontainers.Azurite;

namespace CleanArchitect.Infrastructure.IntegrationTests.Fixtures;

public sealed class AzuriteContainerFixture : IAsyncLifetime
{
    private readonly AzuriteContainer container =
        new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest")
            .WithCommand("--skipApiVersionCheck")
            .Build();

    public async Task InitializeAsync()
    {
        await container.StartAsync();
        await CreateClient().GetBlobContainerClient("product").CreateAsync();
    }

    public async Task DisposeAsync()
    {
        await container.DisposeAsync();
    }

    public BlobServiceClient CreateClient()
    {
        return new BlobServiceClient(container.GetConnectionString());
    }
}
