using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;
public record GetUsersQuery(int Page): IRequest;


