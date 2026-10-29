using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Dto;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

[Authorize(Roles = nameof(Role.Admin))]
public record GetUsersQuery(int Page): IRequest<List<UserDto>>;


