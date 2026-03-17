using Jira.Api.Resources;
using Jira.Application.ResultErrors;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Jira.Api.Extensions;

public static class ResultExtensions
{
    public static Results<Ok<TResponseModel>, BadRequest, NotFound, ProblemHttpResult> ToGetResult<TDomainModel, TResponseModel>(
        this Result<TDomainModel> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(mapper(serviceResult.Value));
        }

        if (serviceResult.HasError<EntityDoesNotExistError>())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }

    public static Results<Ok<List<TResponseModel>>, BadRequest, ProblemHttpResult> ToGetResult<TDomainModel, TResponseModel>(
        this Result<List<TDomainModel>> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(serviceResult.Value.Select(mapper).ToList());
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }

    public static Results<Ok<TResponseModel>, BadRequest, NotFound, ProblemHttpResult> ToPutResult<TDomainModel, TResponseModel>(
        this Result<TDomainModel> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(mapper(serviceResult.Value));
        }

        if (serviceResult.HasError<EntityDoesNotExistError>() || serviceResult.HasError<OperationFailedError>())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }

    public static Results<Ok, BadRequest, NotFound, ProblemHttpResult> ToOkPostResult(this Result serviceResult)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok();
        }

        if (serviceResult.HasError<EntityDoesNotExistError>() || serviceResult.HasError<OperationFailedError>())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
    }
}
