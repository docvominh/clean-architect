using System.Collections.Concurrent;
using System.Diagnostics;

const int concurrency = 100;
var duration = TimeSpan.FromSeconds(30);
var baseUrl = Environment.GetEnvironmentVariable("LOADTEST_BASE_URL") ?? "https://localhost:7189";
var currencies = new[] { "GBP", "EUR" };

// Local dev cert isn't trusted by the tooling; only skip validation for localhost targets.
var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
using var httpClient = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };

var latenciesMs = new ConcurrentBag<double>();
var statusCodes = new ConcurrentDictionary<string, int>();
var successCount = 0;
var failCount = 0;

Console.WriteLine($"Load testing {baseUrl}/api/exchange-rates with {concurrency} concurrent clients for {duration}...");

var stopwatch = Stopwatch.StartNew();

await Parallel.ForEachAsync(GenerateWorkItems(duration), new ParallelOptions { MaxDegreeOfParallelism = concurrency }, async (_, ct) =>
{
    var targetCurrency = currencies[Random.Shared.Next(currencies.Length)];
    var requestStopwatch = Stopwatch.StartNew();

    try
    {
        using var response = await httpClient.GetAsync($"/api/exchange-rates?targetCurrency={targetCurrency}", ct);
        requestStopwatch.Stop();
        latenciesMs.Add(requestStopwatch.Elapsed.TotalMilliseconds);
        statusCodes.AddOrUpdate(((int)response.StatusCode).ToString(), 1, (_, count) => count + 1);

        if (response.IsSuccessStatusCode)
        {
            Interlocked.Increment(ref successCount);
        }
        else
        {
            Interlocked.Increment(ref failCount);
        }
    }
    catch (Exception ex)
    {
        requestStopwatch.Stop();
        latenciesMs.Add(requestStopwatch.Elapsed.TotalMilliseconds);
        statusCodes.AddOrUpdate(ex.GetType().Name, 1, (_, count) => count + 1);
        Interlocked.Increment(ref failCount);
    }
});

stopwatch.Stop();

PrintReport();

return;

static async IAsyncEnumerable<int> GenerateWorkItems(TimeSpan runFor)
{
    var deadline = DateTime.UtcNow + runFor;
    while (DateTime.UtcNow < deadline)
    {
        yield return 0;
        await Task.Yield();
    }
}

void PrintReport()
{
    var sorted = latenciesMs.OrderBy(x => x).ToArray();
    var total = sorted.Length;

    Console.WriteLine();
    Console.WriteLine("==================== Load Test Report ====================");
    Console.WriteLine($"Duration:     {stopwatch.Elapsed}");
    Console.WriteLine($"Concurrency:  {concurrency}");
    Console.WriteLine();
    Console.WriteLine($"Requests:     total = {total}, ok = {successCount}, fail = {failCount}");
    Console.WriteLine($"RPS:          {total / stopwatch.Elapsed.TotalSeconds:F2} req/s");
    Console.WriteLine();

    if (total > 0)
    {
        Console.WriteLine("Latency (ms):");
        Console.WriteLine($"  min = {sorted[0]:F2}, mean = {sorted.Average():F2}, max = {sorted[^1]:F2}");
        Console.WriteLine($"  p50 = {Percentile(sorted, 50):F2}, p75 = {Percentile(sorted, 75):F2}, p95 = {Percentile(sorted, 95):F2}, p99 = {Percentile(sorted, 99):F2}");
        Console.WriteLine();
    }

    Console.WriteLine("Status codes:");
    foreach (var (code, count) in statusCodes.OrderBy(kv => kv.Key))
    {
        Console.WriteLine($"  {code}: {count}");
    }

    Console.WriteLine("============================================================");
}

static double Percentile(double[] sortedValues, int percentile)
{
    var index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Length) - 1;
    return sortedValues[Math.Clamp(index, 0, sortedValues.Length - 1)];
}
