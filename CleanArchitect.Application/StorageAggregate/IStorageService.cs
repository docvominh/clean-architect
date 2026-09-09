namespace CleanArchitect.Application.StorageAggregate;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken);
}
