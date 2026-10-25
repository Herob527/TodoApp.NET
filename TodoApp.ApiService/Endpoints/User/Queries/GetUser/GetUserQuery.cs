using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;
public record GetUserQuery(int Id): IRequest;


