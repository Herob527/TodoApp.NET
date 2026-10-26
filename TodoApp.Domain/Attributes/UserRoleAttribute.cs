namespace TodoApp.Domain.Attributes;

using TodoApp.Domain.Enum;
using Microsoft.AspNetCore.Authorization;

public sealed class UserRoleAttribute : AuthorizeAttribute
{
    public UserRoleAttribute(params Role[] roles)
    {
        Roles = string.Join(",", roles.Select(r => r.ToString()));
    }
}
