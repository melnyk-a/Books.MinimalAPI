using Books.Domain;
using Books.Infrastructure;
using Books.MinimalApi.Endpoints.Countries;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Books.MinimalApi.Tests.Integration.Endpoints.Countries;

[Collection("Database collection")]
public class CreateCountryEndpointTest : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    private Guid? createdCountryId;
    private Guid? existingCountryId;
    public CreateCountryEndpointTest(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCountry_CreatesCountry_WhenDataIsCorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        var country = GenerateCountry();

        // Act
        var result = await httpClient.PostAsJsonAsync("/countries", country);
        var createdCountryId = await result.Content.ReadFromJsonAsync<Guid>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCountryId.Should().NotBeEmpty();
        result.Headers.Location.AbsolutePath.Should().Be($"/countries/{createdCountryId}");

        // Clean up
        this.createdCountryId = createdCountryId;
    }

    [Fact]
    public async Task CreateCountry_Fails_WhenUnauthorized()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var country = GenerateCountry();

        // Act
        var result = await httpClient.PostAsJsonAsync("/countries", country);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateCountry_Fails_WhenNameIsEmpty()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        var country = GenerateCountry("");

        // Act
        var result = await httpClient.PostAsJsonAsync("/countries", country);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation error");
        problemDetails.Type.Should().Be("ValidationFailure");
        problemDetails.Detail.Should().Be("One or more validation errors has occurred");
    }

    [Fact]
    public async Task CreateCountry_Fails_WhenNameExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        httpClient.SetFakeBearerToken(new Dictionary<string, object>());
        var country = GenerateCountry("Test");

        // Act
        var result = await httpClient.PostAsJsonAsync("/countries", country);
        var problemDetails = await result.Content.ReadFromJsonAsync<ProblemDetails>();

        // Assert
        using var scope = new AssertionScope();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails.Should().NotBeNull();
        problemDetails.Extensions.Should().NotBeEmpty();
        problemDetails!.Extensions["code"] = CountryErrors.AlreadyExist.Code;
        problemDetails.Extensions["name"] = CountryErrors.AlreadyExist.Name;
    }

    private static CreateCountryRequest GenerateCountry(string name = "Test country")
    {
        return new CreateCountryRequest(name);
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var country = Country.Create(new Name("Test"));
        context.Set<Country>().Add(country);
        existingCountryId = country.Id;
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var createdCountry = await context.Set<Country>().FindAsync(existingCountryId);
        context.Set<Country>().Remove(createdCountry!);
        if (createdCountryId is not null)
        {
            createdCountry = await context.Set<Country>().FindAsync(createdCountryId);
            context.Set<Country>().Remove(createdCountry!);
        }
        await context.SaveChangesAsync();
    }
}
