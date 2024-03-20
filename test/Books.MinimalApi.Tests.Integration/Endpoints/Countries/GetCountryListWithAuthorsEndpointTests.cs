using Books.Application.Countries.GetCountryWithAuthorList;

namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class GetCountryListWithAuthorsEndpointTests : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private readonly IList<Country> _countries = [];
    public GetCountryListWithAuthorsEndpointTests(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetCountryListWithAuthors_ReturnsAllCountries_WhenCountriesExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        await SeedCountryWithData();

        // Act
        var result = await httpClient.GetAsync("/countries/authors");
        var actualCountries = await result.Content.ReadFromJsonAsync<List<CountryWithAuthorResponse>>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        actualCountries.Should().NotBeNullOrEmpty();
        actualCountries!.Count.Should().Be(3);
        actualCountries.Should().Contain(x => x.Name == "Test 1");
        actualCountries.Should().Contain(x => x.Name == "Test 2");
        actualCountries.Should().Contain(x => x.Name == "Test 3");

        var authors = actualCountries.SelectMany(X => X.AuthorsResponse).ToList();
        authors.Count.Should().Be(2);
        authors.Should().Contain(x => x.FirstName == "First Name 1" && x.LastName == "Last Name 1");
        authors.Should().Contain(x => x.FirstName == "First Name 2" && x.LastName == "Last Name 2");
    }

    [Fact]
    public async Task GetCountryListWithAuthors_ReturnsEmpty_WhenCountriesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());

        // Act
        var result = await httpClient.GetAsync("/countries/authors");
        var actualCountries = await result.Content.ReadFromJsonAsync<List<CountryWithAuthorResponse>>();

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

    private async Task SeedCountryWithData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _countries.Add(Country.Create(new Name("Test 1")));
        _countries.Add(Country.Create(new Name("Test 2")));
        _countries.Add(Country.Create(new Name("Test 3")));
        context.Set<Country>().AddRange(_countries);

        var authors = new List<Author>{
            Author.Create(
                new FirstName("First Name 1"),
                new LastName("Last Name 1"),
                _countries[0].Id),
            Author.Create(
                new FirstName("First Name 2"),
                new LastName("Last Name 2"),
                _countries[1].Id)};
        context.Set<Author>().AddRange(authors);

        await context.SaveChangesAsync();
    }
}
