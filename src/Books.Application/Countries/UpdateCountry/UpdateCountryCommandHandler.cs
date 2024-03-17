using Books.Application.Abstractions.Messaging;
using Books.Domain.Abstractions.Persistence;
using Books.Domain.Abstractions;
using Books.Domain;

namespace Books.Application.Countries.UpdateCountry;

internal sealed class UpdateCountryCommandHandler : ICommandHandler<UpdateCountryCommand, Guid>
{
    private readonly ICountryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCountryCommandHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _repository.GetByIdAsync(request.CountryId);

        if (country is null)
        {
            return Result.Failure<Guid>(CountryErrors.NotFound);
        }

        country.UpdateName(new Name(request.CountryName));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return country.Id;
    }
}