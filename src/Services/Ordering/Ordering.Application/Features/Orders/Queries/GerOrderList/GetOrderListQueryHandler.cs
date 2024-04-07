using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GerOrderList;

public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, List<OrderVm>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;

    public GetOrderListQueryHandler(IOrderRepository repository, IMapper mapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<OrderVm>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var orderList = await _repository.GetOrdersByUserName(request.UserName);

        return _mapper.Map<List<OrderVm>>(orderList);
    }
}