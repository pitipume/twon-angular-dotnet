using MediatR;
using Twon.Application.Common;
using Twon.Application.Store.Managers;

namespace Twon.Application.Store.Commands.CreateOrder;

public class CreateOrderHandler(StoreManager manager)
    : IRequestHandler<CreateOrderCommand, BaseResult<OrderDto>>
{
    public async Task<BaseResult<OrderDto>> Handle(
        CreateOrderCommand request, CancellationToken cancellationToken)
        => await manager.CreateOrderAsync(request.UserId, request.ProductIds);
}
