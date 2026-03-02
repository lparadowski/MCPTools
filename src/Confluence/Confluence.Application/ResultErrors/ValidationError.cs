using FluentResults;

namespace Confluence.Application.ResultErrors;

public class ValidationError(string propertyName, IEnumerable<string> errorMessages) : Error()
{
    public string PropertyName { get; } = propertyName;
    public IEnumerable<string> ErrorMessages { get; } = errorMessages;
}
