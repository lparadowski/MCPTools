using Confluence.Api.Resources;
using Confluence.Application.ResultErrors;
using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Confluence.Api.Extensions;

public static class ResultExtensions
{
    public static Results<Ok<TResponseModel>, BadRequest, NotFound, ProblemHttpResult> ToGetResult<TDomainModel, TResponseModel>(
        this Result<TDomainModel> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(mapper(serviceResult.Value));
        }
        else
        {
            if (serviceResult.HasError<ValidationError>(out var validationErrors))
            {
                return CreateBadRequestResult(validationErrors);
            }
            else if (serviceResult.HasError<EntityDoesNotExistError>())
            {
                return TypedResults.NotFound();
            }
            else
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public static Results<Ok<List<TResponseModel>>, BadRequest, ProblemHttpResult> ToGetResult<TDomainModel, TResponseModel>(
        this Result<List<TDomainModel>> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(serviceResult.Value.Select(mapper).ToList());
        }
        else
        {
            return serviceResult.HasError<ValidationError>(out var validationErrors)
                ? CreateBadRequestResult(validationErrors)
                : TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public static Results<Ok<TResponseModel>, BadRequest, NotFound, ProblemHttpResult> ToPutResult<TDomainModel, TResponseModel>(
        this Result<TDomainModel> serviceResult, Func<TDomainModel, TResponseModel> mapper)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok(mapper(serviceResult.Value));
        }
        else
        {
            if (serviceResult.HasError<ValidationError>(out var validationErrors))
            {
                return CreateBadRequestResult(validationErrors);
            }
            else if (serviceResult.HasError<EntityDoesNotExistError>())
            {
                return TypedResults.NotFound();
            }
            else
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    public static Results<Ok, BadRequest, NotFound, ProblemHttpResult> ToOkPostResult(this Result serviceResult)
    {
        if (serviceResult.IsSuccess)
        {
            return TypedResults.Ok();
        }
        else
        {
            if (serviceResult.HasError<ValidationError>(out var validationErrors))
            {
                return CreateBadRequestResult(validationErrors);
            }
            else if (serviceResult.HasError<EntityDoesNotExistError>())
            {
                return TypedResults.NotFound();
            }
            else
            {
                return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }

    private static ProblemHttpResult CreateBadRequestResult(IEnumerable<ValidationError> validationErrors)
    {
        var errorsDictionary = validationErrors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.SelectMany(x => x.ErrorMessages).ToArray()
            );

        return TypedResults.Problem(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = ResponseMessages.Status400Title,
            Extensions = new Dictionary<string, object?> {
                { ResponseMessages.Status400JsonErrorsTitle, errorsDictionary }
            }
        });
    }
}
