using MediatR;
using Twon.Application.Common;
using Twon.Application.Store.Commands.CreateOrder;

namespace Twon.Application.Store.Queries.GetOrder;

public record GetOrderQuery(string UserId, string OrderId) : IRequest<BaseResult<OrderDto>>;
