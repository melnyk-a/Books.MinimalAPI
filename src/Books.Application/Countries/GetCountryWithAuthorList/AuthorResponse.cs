namespace Books.Application.Countries.GetCountryWithAuthorList;

public sealed class AuthorResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
}