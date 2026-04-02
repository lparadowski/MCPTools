using FluentResults;

namespace AzureDevOps.Application.ResultErrors;

public class OperationFailedError : Error
{
    public OperationFailedError() : base("The operation failed.") { }
}
