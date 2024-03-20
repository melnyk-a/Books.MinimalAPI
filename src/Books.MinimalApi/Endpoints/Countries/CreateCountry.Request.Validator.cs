using FluentValidation;

namespace Books.MinimalApi.Endpoints.Countries;

public class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
{
    public CreateCountryRequestValidator()
    {
        RuleFor(book => book.Name).NotEmpty();
    }
}
