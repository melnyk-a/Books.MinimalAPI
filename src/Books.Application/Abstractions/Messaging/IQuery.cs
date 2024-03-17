using Books.Domain.Abstractions;
using MediatR;

namespace Books.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}