using Books.Application.Countries.GetCountryList;
using Books.Domain;
using Books.Infrastructure;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Net;

namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class GetCountryListEndpointTest : IAsyncLifetime
{
    private readonly TestApiFactory _factory;
    private readonly IList<Country> _countries = [];
    public GetCountryListEndpointTest(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCountryList_ReturnsAllCountries_WhenCountriesExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        await SeedCountries();

        // Act
        var result = await httpClient.GetAsync("/countries");
        var actualCountries = await result.Content.ReadFromJsonAsync<List<CountryResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actualCountries.Should().NotBeNullOrEmpty();
        actualCountries!.Should().NotBeEmpty();
        actualCountries.Count.Should().Be(2);
        actualCountries.Should().Contain(x => x.Name == "Test 1");
        actualCountries.Should().Contain(x => x.Name == "Test 2");
    }

    [Fact]
    public async Task GetCountryList_ReturnsAllCountriesWithName_WhenNameIsPassed()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        await SeedCountries();

        // Act
        var result = await httpClient.GetAsync($"/countries?name={Uri.EscapeDataString("Test 1")}");
        var actualCountries = await result.Content.ReadFromJsonAsync<List<CountryResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actualCountries.Should().NotBeNullOrEmpty();
        actualCountries!.Should().NotBeEmpty();
        actualCountries.Count.Should().Be(1);
        actualCountries.Should().Contain(x => x.Name == "Test 1");
    }

    [Fact]
    public async Task GetCountryList_ReturnsEmpty_WhenCountriesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());

        // Act
        var result = await httpClient.GetAsync("/countries");
        var actualCountries = await result.Content.ReadFromJsonAsync<List<CountryResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actualCountries.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCountryList_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync("/countries");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_countries.Any())
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Set<Country>().RemoveRange(_countries);
            await context.SaveChangesAsync();
        }
    }

    public async Task SeedCountries()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _countries.Add(Country.Create(new Name("Test 1")));
        _countries.Add(Country.Create(new Name("Test 2")));
        context.Set<Country>().AddRange(_countries);
        await context.SaveChangesAsync();
    }
}
