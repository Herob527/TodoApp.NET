using MediatR;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;
public record GetUsersQuery(int Page): IRequest<List<UserDto>>;


