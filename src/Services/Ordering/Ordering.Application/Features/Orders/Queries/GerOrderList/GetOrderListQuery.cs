using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GerOrderList;

public class GetOrderListQuery : IRequest<List<OrderVm>>
{
    public string UserName { get; set; }

    public GetOrderListQuery(string userName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
}