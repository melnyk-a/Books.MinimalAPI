using Books.Application.Abstractions.Messaging;

namespace Books.Application.Countries.DeleteCountry;

public sealed record DeleteCountryCommand(Guid Id) : ICommand;