using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using CleanArchitect.Application.StorageAggregate;

namespace CleanArchitect.Infrastructure.StorageAggregate;

public sealed class AzureBlobService(BlobServiceClient blobServiceClient) : IStorageService
{
    public const string ContainerName = "product";

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var container = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blob = container.GetBlobClient($"{Guid.NewGuid()}{Path.GetExtension(fileName)}");
        await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        return blob.Uri.ToString();
    }
}
