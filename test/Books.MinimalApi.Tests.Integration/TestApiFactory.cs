using Microsoft.AspNetCore.Mvc.Testing;
using Books.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.TestHost;
using WebMotions.Fake.Authentication.JwtBearer;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Books.MinimalApi.EndpointFilters;

namespace Books.MinimalApi.Tests.Integration;
public class TestApiFactory : WebApplicationFactory<IApiMarker>
{
    private readonly string db_name = $"books_{Guid.NewGuid()}";

    public Mock<ILogger<LogNotFoundResponseFilter>> NotFoundFilterLogger { get; } = new();
    public TestApiFactory()
    {
        // Don't need for now, as migration is done in main
        // MigrateAndSeed();
    }

    private void MigrateAndSeed()
    {
        using var scope = Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var mockLoggerProvider = new Mock<ILoggerProvider>();
        mockLoggerProvider.Setup(p => p.CreateLogger(
            It.IsAny<string>())).Returns<string>(category =>
        {
            if (category == typeof(LogNotFoundResponseFilter).FullName)
            {
                return NotFoundFilterLogger.Object;
            }
            return NullLoggerProvider.Instance.CreateLogger(category);
        });

        builder.ConfigureLogging(o =>
        {
            o.ClearProviders();
            o.AddProvider(mockLoggerProvider.Object);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"DataSource=file:{db_name}?mode=memory&cache=shared");
            });
            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
            .AddFakeJwtBearer();
        });
    }
}
