using FluentResults;

namespace AzureDevOps.Application.ResultErrors;

public class EntityDoesNotExistError : Error
{
    public EntityDoesNotExistError() : base("The requested entity does not exist.") { }
}
