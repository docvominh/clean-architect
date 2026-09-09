using CleanArchitect.Application.StorageAggregate;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Controllers;

[ApiController]
[Route("api/storage")]
[Produces("application/json")]
public class StorageController(IStorageService storage) : ControllerBase
{
    private static readonly HashSet<string> AllowedImageContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png", "image/jpeg", "image/webp", "image/gif"
    };

    [HttpPost("upload")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UploadResponse>> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (!AllowedImageContentTypes.Contains(file.ContentType))
        {
            return BadRequest("Only PNG, JPEG, WEBP, and GIF images are supported.");
        }

        var url = await storage.UploadAsync(file.OpenReadStream(), file.FileName, file.ContentType, cancellationToken);

        return Ok(new UploadResponse(url));
    }
}

public sealed record UploadResponse(string Url);
