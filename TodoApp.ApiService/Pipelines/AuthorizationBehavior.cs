using System.Reflection;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService.Pipelines;

public class AuthorizationBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
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

        var user = await userManager.GetUserAsync(httpContext.User) ?? throw new UnauthorizedAccessException();

        var roles = await userManager.GetRolesAsync(user);
        var claims = await userManager.GetClaimsAsync(user);

        var userRoles = roles.ToList();
        foreach (var claim in claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role))
        {
            if (!userRoles.Contains(claim.Value))
            {
                userRoles.Add(claim.Value);
            }
        }

        if (httpContext.User != null)
        {
            foreach (var claim in httpContext.User.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role))
            {
                if (!userRoles.Contains(claim.Value))
                {
                    userRoles.Add(claim.Value);
                }
            }
        }

        var authorizeAttribute = requestType.GetCustomAttribute<AuthorizeAttribute>();
        if (authorizeAttribute != null)
        {
            if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
            {
                var requiredRoles = authorizeAttribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (!requiredRoles.Any(userRoles.Contains))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return await next(cancellationToken);
        }

        return await next(cancellationToken);
    }
}
