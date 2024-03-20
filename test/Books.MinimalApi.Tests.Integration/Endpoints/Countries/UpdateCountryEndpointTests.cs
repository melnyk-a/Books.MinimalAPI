namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class UpdateCountryEndpointTests : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private readonly IList<Country> _countries = [];
    private Guid countryIdToUpdateId;

    public UpdateCountryEndpointTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UpdateCountry_UpdatesCountry_WhenDataIsCorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "Ukraine" },
            { ClaimTypes.Role, "admin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("Test 1 Updated");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateCountry_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var updateCountryRequest = new UpdateCountryRequest("Test 1 Updated");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateCountry_Fails_WhenRoleNotAdmin()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "Ukraine" },
            { ClaimTypes.Role, "NotAdmin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("Test 1 Updated");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCountry_Fails_WhenClaimNotUkraine()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "NotUkraine" },
            { ClaimTypes.Role, "admin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("Test 1 Updated");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCountry_Fails_WhenNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "Ukraine" },
            { ClaimTypes.Role, "admin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("Test 1 Updated");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{Guid.NewGuid()}", updateCountryRequest);
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

    [Fact]
    public async Task UpdateCountry_Fails_WhenNameExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "Ukraine" },
            { ClaimTypes.Role, "admin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("Test 2");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        problemDetails.Extensions.Should().NotBeEmpty();
        problemDetails!.Extensions["code"] = CountryErrors.AlreadyExist.Code;
        problemDetails.Extensions["name"] = CountryErrors.AlreadyExist.Name;
    }

    [Fact]
    public async Task UpdateCountry_Fails_WhenNameIsEmpty()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var claims = new Dictionary<string, object>
        {
            { "country", "Ukraine" },
            { ClaimTypes.Role, "admin" },
        };
        httpClient.SetFakeBearerToken(claims);
        var updateCountryRequest = new UpdateCountryRequest("");

        // Act
        var result = await httpClient.PutAsJsonAsync($"/countries/{countryIdToUpdateId}", updateCountryRequest);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation error");
        problemDetails.Type.Should().Be("ValidationFailure");
        problemDetails.Detail.Should().Be("One or more validation errors has occurred");
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _countries.Add(Country.Create(new Name("Test 1")));
        _countries.Add(Country.Create(new Name("Test 2")));
        context.Set<Country>().AddRange(_countries);
        countryIdToUpdateId = _countries[0].Id;
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (_countries.Any())
        {
            context.Set<Country>().RemoveRange(_countries);
            await context.SaveChangesAsync();
        }
    }
}
