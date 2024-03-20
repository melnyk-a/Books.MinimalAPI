namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class DeleteCountryEndpointTests : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private Guid countryIdToDelete;

    public DeleteCountryEndpointTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeleteCountry_DeletesCountry_WhenDataIsCorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
            {
                { "country", "Ukraine" },
                { ClaimTypes.Role, "admin" },
            };
        httpClient.SetFakeBearerToken(claims);

        // Act
        var result = await httpClient.DeleteAsync($"/countries/{countryIdToDelete}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteCountry_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var result = await httpClient.DeleteAsync($"/countries/{countryIdToDelete}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCountry_Fails_WhenRoleNotAdmin()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
            {
                { "country", "Ukraine" },
                { ClaimTypes.Role, "NotAdmin" },
            };
        httpClient.SetFakeBearerToken(claims);

        // Act
        var result = await httpClient.DeleteAsync($"/countries/{countryIdToDelete}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCountry_Fails_WhenClaimNotUkraine()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
            {
                { "country", "NotUkraine" },
                { ClaimTypes.Role, "admin" },
            };
        httpClient.SetFakeBearerToken(claims);

        // Act
        var result = await httpClient.DeleteAsync($"/countries/{countryIdToDelete}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteCountry_Fails_WhenNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
            {
                { "country", "Ukraine" },
                { ClaimTypes.Role, "admin" },
            };
        httpClient.SetFakeBearerToken(claims);

        // Act
        var result = await httpClient.DeleteAsync($"/countries/{Guid.NewGuid()}");
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        problemDetails.Should().NotBeNull();
        problemDetails.Extensions.Should().NotBeEmpty();
        problemDetails!.Extensions["code"] = CountryErrors.NotFound.Code;
        problemDetails.Extensions["name"] = CountryErrors.NotFound.Name;

        _factory.NotFoundFilterLogger.Verify(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (v, t) => v.ToString() == $"Resource {result.RequestMessage.RequestUri.AbsolutePath} was not found"),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(1));
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var country = Country.Create(new Name("Test"));
        context.Set<Country>().Add(country);
        countryIdToDelete = country.Id;
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var countryToDelete = await context.Set<Country>().FindAsync(countryIdToDelete);
        if (countryToDelete is not null)
        {
            context.Set<Country>().Remove(countryToDelete);
            await context.SaveChangesAsync();
        }
    }
}
