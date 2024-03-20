using Books.Application.Countries.GetCountryList;
using Books.Domain.Abstractions;

namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class GetCountryByIdEndpointTests : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private Country? _country;
    public GetCountryByIdEndpointTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCountryById_ReturnsCountry_WhenCountryExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());

        // Act
        var result = await httpClient.GetAsync($"/countries/{_country!.Id}");
        var actualCountry = await result.Content.ReadFromJsonAsync<CountryResponse>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actualCountry.Should().NotBeNull();
        actualCountry!.Id.Should().Be(_country.Id);
        actualCountry.Name.Should().Be(_country.Name.Value);
    }

    [Fact]
    public async Task GetCountryById_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync($"/countries/{_country!.Id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCountryById_Fails_WhenCountryDoesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        var countryId = Guid.NewGuid();

        // Act
        var result = await httpClient.GetAsync($"/countries/{countryId}");
        var error = await result.Content.ReadFromJsonAsync<Error>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        error.Should().NotBeNull();
        error!.Code.Should().Be(CountryErrors.NotFound.Code);
        error.Name.Should().Be(CountryErrors.NotFound.Name);
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
