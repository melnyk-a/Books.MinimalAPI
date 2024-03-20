using FluentValidation;

namespace Books.MinimalApi.Endpoints.Countries;

public class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
{
    public UpdateCountryRequestValidator()
    {
        RuleFor(book => book.Name).NotEmpty();
    }
}
