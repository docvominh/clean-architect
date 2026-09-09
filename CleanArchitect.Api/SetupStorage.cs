using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using CleanArchitect.Infrastructure.StorageAggregate;

namespace CleanArchitect.Api;

public static class SetupStorage
{
    public static async Task SetupStorageAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        var blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
        var container = blobServiceClient.GetBlobContainerClient(AzureBlobService.ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);
    }
}
