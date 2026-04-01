using MediatR;
using Twon.Application.Common;
using Twon.Application.Store.Commands.CreateOrder;
using Twon.Application.Store.Managers;

namespace Twon.Application.Store.Queries.GetOrder;

public class GetOrderHandler(StoreManager manager)
    : IRequestHandler<GetOrderQuery, BaseResult<OrderDto>>
{
    public async Task<BaseResult<OrderDto>> Handle(
        GetOrderQuery request, CancellationToken cancellationToken)
        => await manager.GetOrderAsync(request.UserId, request.OrderId);
}
