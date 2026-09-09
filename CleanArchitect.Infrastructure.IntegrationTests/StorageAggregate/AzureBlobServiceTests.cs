using Azure.Storage.Blobs;

using CleanArchitect.Application.StorageAggregate;
using CleanArchitect.Infrastructure.IntegrationTests.Fixtures;
using CleanArchitect.Infrastructure.StorageAggregate;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.StorageAggregate;

public sealed class AzureBlobServiceTests(AzuriteContainerFixture fixture) : IClassFixture<AzuriteContainerFixture>
{
    [Theory]
    [InlineData("photo.png", "image/png", ".png")]
    [InlineData("archive.tar.gz", "application/gzip", ".gz")]
    [InlineData("README", "text/plain", "")]
    public async Task UploadAsync_PersistsContentAndContentTypeAndReturnsBlobUri(string fileName, string contentType, string extension)
    {
        // Arrange
        IStorageService service = new AzureBlobService(fixture.CreateClient());
        byte[] expected = [0, 1, 127, 128, 255];
        using var content = new MemoryStream(expected);

        // Act
        var result = await service.UploadAsync(content, fileName, contentType, CancellationToken.None);

        // Assert
        var uri = new BlobUriBuilder(new Uri(result));
        uri.BlobContainerName.ShouldBe("product");
        Path.GetExtension(uri.BlobName).ShouldBe(extension);
        Guid.TryParse(Path.GetFileNameWithoutExtension(uri.BlobName), out _).ShouldBeTrue();
        var blob = fixture.CreateClient().GetBlobContainerClient("product").GetBlobClient(uri.BlobName);
        result.ShouldBe(blob.Uri.ToString());
        var downloaded = await blob.DownloadContentAsync();
        downloaded.Value.Content.ToArray().ShouldBe(expected);
        downloaded.Value.Details.ContentType.ShouldBe(contentType);
    }

    [Fact]
    public async Task UploadAsync_SameFileName_PreservesBothUploads()
    {
        // Arrange
        IStorageService service = new AzureBlobService(fixture.CreateClient());
        using var first = new MemoryStream([1, 2, 3]);
        using var second = new MemoryStream([4, 5, 6]);

        // Act
        var firstUri = await service.UploadAsync(first, "photo.png", "image/png", CancellationToken.None);
        var secondUri = await service.UploadAsync(second, "photo.png", "image/png", CancellationToken.None);

        // Assert
        firstUri.ShouldNotBe(secondUri);
        var container = fixture.CreateClient().GetBlobContainerClient("product");
        var firstBlob = container.GetBlobClient(new BlobUriBuilder(new Uri(firstUri)).BlobName);
        var secondBlob = container.GetBlobClient(new BlobUriBuilder(new Uri(secondUri)).BlobName);
        (await firstBlob.DownloadContentAsync()).Value.Content.ToArray().ShouldBe(new byte[] { 1, 2, 3 });
        (await secondBlob.DownloadContentAsync()).Value.Content.ToArray().ShouldBe(new byte[] { 4, 5, 6 });
    }

    [Fact]
    public async Task UploadAsync_CancelledToken_ThrowsWithoutCreatingBlob()
    {
        // Arrange
        IStorageService service = new AzureBlobService(fixture.CreateClient());
        var container = fixture.CreateClient().GetBlobContainerClient("product");
        var before = new List<string>();

        await foreach (var blob in container.GetBlobsAsync()) before.Add(blob.Name);

        using var content = new MemoryStream([1, 2, 3]);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        // Act
        var exception = await Record.ExceptionAsync(() =>
            service.UploadAsync(content, "cancelled.png", "image/png", cancellation.Token));

        // Assert
        exception.ShouldBeAssignableTo<OperationCanceledException>();
        var after = new List<string>();

        await foreach (var blob in container.GetBlobsAsync()) after.Add(blob.Name);

        after.ShouldBe(before);
    }
}
