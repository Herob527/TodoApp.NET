using MediatR;
using TodoApp.Contracts.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;
public record GetUserQuery(string Id): IRequest<UserDto>;
