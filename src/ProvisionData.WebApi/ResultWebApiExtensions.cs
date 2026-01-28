// ProvisionData.Common
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProvisionData;

public static class ResultWebApiExtensions
{
    public static IResult ToApiResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => Results.NotFound(CreateProblemDetails(result.Error, 404)),
            ValidationError => Results.BadRequest(CreateProblemDetails(result.Error, 400)),
            ConflictError => Results.Conflict(CreateProblemDetails(result.Error, 409)),
            UnauthorizedError => CreateUnauthorizedResult(result.Error),
            _ => Results.BadRequest(CreateProblemDetails(result.Error, 400))
        };
    }

    public static IResult ToApiResult(this Result result, Object? successValue = null)
    {
        if (result.IsSuccess)
            return successValue is not null
                ? Results.Ok(successValue)
                : Results.Ok();

        return result.Error switch
        {
            NotFoundError => Results.NotFound(CreateProblemDetails(result.Error, 404)),
            ValidationError => Results.BadRequest(CreateProblemDetails(result.Error, 400)),
            ConflictError => Results.Conflict(CreateProblemDetails(result.Error, 409)),
            _ => Results.BadRequest(CreateProblemDetails(result.Error, 400))
        };
    }

    public static IResult ToCreatedResult<T>(
        this Result<T> result,
        String location)
    {
        if (result.IsSuccess)
            return Results.Created(location, result.Value);

        return result.ToApiResult();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "ProblemDetails is a well-known type that will be preserved by the trimmer")]
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling", Justification = "ProblemDetails is a simple DTO that doesn't require runtime code generation")]
    private static IResult CreateUnauthorizedResult(Error error)
        => Results.Json(
            CreateProblemDetails(error, 401),
            statusCode: 401);

    private static ProblemDetails CreateProblemDetails(Error error, Int32 statusCode)
        => new()
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Description,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
}
