using Books.Domain.Abstractions;

namespace Books.Domain;

public static class CountryErrors
{
    public static NotFoundError NotFound = new(
        "Country.NotFound",
        "The country with the specified identifier was not found");
}
