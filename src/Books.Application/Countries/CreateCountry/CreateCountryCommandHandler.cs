using Books.Application.Abstractions.Messaging;
using Books.Domain;
using Books.Domain.Abstractions;
using Books.Domain.Abstractions.Persistence;

namespace Books.Application.Countries.CreateCountry;

internal sealed class CreateCountryCommandHandler : ICommandHandler<CreateCountryCommand, Guid>
{
    private readonly ICountryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCountryCommandHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Guid>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = Country.Create(new Name(request.CountryName));
        _repository.Add(country);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return country.Id;
    }
}
