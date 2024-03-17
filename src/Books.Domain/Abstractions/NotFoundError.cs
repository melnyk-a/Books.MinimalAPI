namespace Books.Domain.Abstractions;

public record NotFoundError(string Code, string Name): Error(Code, Name)
{
}