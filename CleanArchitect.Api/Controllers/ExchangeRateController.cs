using CleanArchitect.Application.ExternalSystem;
using CleanArchitect.Application.ExternalSystem.Query;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Controllers;

[ApiController]
[Route("api/exchange-rates")]
[Produces("application/json")]
public class ExchangeRateController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<ExchangeRatesDto> GetLatest(CancellationToken cancellationToken)
    {
        return sender.Send(new GetExchangeRatesQuery(), cancellationToken);
    }
}
