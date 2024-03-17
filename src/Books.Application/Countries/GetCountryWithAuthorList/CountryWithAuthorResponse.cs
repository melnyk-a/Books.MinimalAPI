namespace Books.Application.Countries.GetCountryWithAuthorList;

public sealed class CountryWithAuthorResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public IEnumerable<AuthorResponse> AuthorsResponse { get; init; }
}
