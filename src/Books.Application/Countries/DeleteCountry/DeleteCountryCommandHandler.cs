using Books.Application.Abstractions.Messaging;
using Books.Domain.Abstractions.Persistence;
using Books.Domain.Abstractions;
using Books.Domain;

namespace Books.Application.Countries.DeleteCountry;

internal sealed class DeleteCountryCommandHandler : ICommandHandler<DeleteCountryCommand>
{
    private readonly ICountryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCountryCommandHandler(
        ICountryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
    {
        var country = await _repository.GetByIdAsync(request.Id);

        if (country is null)
        {
            return Result.Failure<Guid>(CountryErrors.NotFound);
        }

        await _repository.DeleteAsync(country);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}