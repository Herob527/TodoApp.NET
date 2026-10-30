using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Contracts.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUserMe;


[Authorize]
public record GetUserMeQuery(): IRequest<UserDto>;
