using MediatR;
using Twon.Application.Common;
using Twon.Application.Payment.Managers;

namespace Twon.Application.Payment.Queries.GetPendingOrders;

public class GetPendingOrdersHandler(PaymentManager manager)
    : IRequestHandler<GetPendingOrdersQuery, BaseResult<List<PendingOrderDto>>>
{
    public async Task<BaseResult<List<PendingOrderDto>>> Handle(
        GetPendingOrdersQuery request, CancellationToken cancellationToken)
        => await manager.GetPendingOrdersAsync();
}
