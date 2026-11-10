using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.ApiService.Services;

namespace TodoApp.ApiService.Pipelines;

public class AuthorizationBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    CurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var httpContext = httpContextAccessor.HttpContext;

        if (requestType.GetCustomAttribute<AllowAnonymousAttribute>() != null)
        {
            return await next(cancellationToken);
        }

        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException();
        }

        var authorizeAttribute = requestType.GetCustomAttribute<AuthorizeAttribute>();
        if (authorizeAttribute is null || string.IsNullOrEmpty(authorizeAttribute.Roles))
        {
            return await next(cancellationToken);
        }

        var requiredRoles = authorizeAttribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (!await currentUser.IsInAnyRoleAsync(requiredRoles, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await next(cancellationToken);
    }
}
