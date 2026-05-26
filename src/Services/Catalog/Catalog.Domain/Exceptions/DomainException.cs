namespace Catalog.Domain.Exceptions;

public class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}