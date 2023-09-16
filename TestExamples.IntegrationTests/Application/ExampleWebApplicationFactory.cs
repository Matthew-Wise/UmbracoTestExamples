using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestExamples.IntegrationTests.Application.Examine;
using TestingExamples.Web;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Examine;

namespace TestExamples.IntegrationTests.Application;

public class ExampleWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string _inMemoryConnectionString = "Data Source=IntegrationTests;Mode=Memory;Cache=Shared";
    private readonly SqliteConnection _imConnection;

    public ExampleWebApplicationFactory()
    {
        // Shared in-memory databases get destroyed when the last connection is closed.
        // Keeping a connection open while this web application is used, ensures that the database does not get destroyed in the middle of a test.
        _imConnection = new SqliteConnection(_inMemoryConnectionString);
        _imConnection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "integration.settings.json");
        builder.ConfigureAppConfiguration(conf =>
        {
            conf.AddJsonFile(configPath);
            conf.AddInMemoryCollection(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("ConnectionStrings:umbracoDbDSN", _inMemoryConnectionString),
                new KeyValuePair<string, string>("ConnectionStrings:umbracoDbDSN_ProviderName", "Microsoft.Data.Sqlite")
            }!);
        });

        builder.ConfigureServices(ConfigureServices);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var sd = services.First(s => s.ImplementationType == typeof(RebuildOnStartupHandler));
        services.Remove(sd);
        services.AddUnique<INotificationHandler<UmbracoRequestBeginNotification>, CustomRebuildOnStartupHandler>(ServiceLifetime.Transient);
        services.AddSingleton<CustomRebuildOnStartupHandlerState>();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // When this application factory is disposed, close the connection to the in-memory database
        //    This will destroy the in-memory database
        _imConnection.Close();
        _imConnection.Dispose();
    }
}