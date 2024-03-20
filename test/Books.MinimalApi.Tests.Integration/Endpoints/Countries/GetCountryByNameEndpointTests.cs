namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class GetCountryByNameEndpointTests : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private Country? _country;
    public GetCountryByNameEndpointTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCountryByName_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync($"/countries/{_country!.Name}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCountryByName_Fails_WithNotImplementedException()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        var name = "NotExistingName";

        // Act
        var result = await httpClient.GetAsync($"/countries/{name}");
        var error = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        error.Should().NotBeNull();
        error.Type.Should().Be("ServerError");
        error.Title.Should().Be("Server error");
        error.Detail.Should().Be("An unexpected error has occurred");
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _country = Country.Create(new Name("Test"));
        context.Set<Country>().Add(_country);
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_country is not null)
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Set<Country>().Remove(_country);
            await context.SaveChangesAsync();
        }
    }
}
