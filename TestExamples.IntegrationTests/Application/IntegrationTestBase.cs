using Microsoft.Extensions.DependencyInjection;

namespace TestExamples.IntegrationTests.Application;

public abstract class IntegrationTestBase
{
    protected ExampleWebApplicationFactory WebsiteFactory { get; private set; }
    protected AsyncServiceScope Scope { get; private set; }
    protected IServiceProvider ServiceProvider => Scope.ServiceProvider;

    protected virtual ExampleWebApplicationFactory CreateApplicationFactory()
    {
        return new ExampleWebApplicationFactory();
    }

    [SetUp]
    public virtual void Setup()
    {
        WebsiteFactory = CreateApplicationFactory();
        Scope = WebsiteFactory.Services.GetRequiredService<IServiceScopeFactory>().CreateAsyncScope();
    }

    [TearDown]
    public virtual void TearDown()
    {
        Scope.Dispose();
        WebsiteFactory.Dispose();
    }

    protected virtual HttpClient Client => WebsiteFactory.CreateClient();

    protected virtual T GetRequiredService<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();
}