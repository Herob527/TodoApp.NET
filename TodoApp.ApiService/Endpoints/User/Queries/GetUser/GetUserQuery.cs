using MediatR;
using TodoApp.Domain.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;
public record GetUserQuery(string Id): IRequest<UserDto>;
