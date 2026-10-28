using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

[Authorize]
public record GetUsersQuery(int Page): IRequest<List<UserDto>>;


