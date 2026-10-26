using MediatR;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;
public record GetUserQuery(int Id): IRequest<UserDto>;


