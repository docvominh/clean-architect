// See https://aka.ms/new-console-template for more information

using CleanArchitect.Database;

using Microsoft.Extensions.Configuration;

Console.WriteLine("Migration started");

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", true)
    .AddJsonFile($"appsettings.{environment}.json", true)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AzureSql");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(
        "Missing ConnectionStrings:AzureSql. Set it in appsettings.json/appsettings.{Environment}.json or the ConnectionStrings__AzureSql environment variable.");
    Console.ResetColor();

    return -1;
}

var result = DatabaseMigrator.Migrate(connectionString);

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();

return 0;
