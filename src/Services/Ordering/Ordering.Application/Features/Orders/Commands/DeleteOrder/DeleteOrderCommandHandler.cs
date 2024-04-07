using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IOrderRepository _repository;

    private readonly IMapper _mapper;

    private readonly ILogger<DeleteOrderCommandHandler> _logger;

    public DeleteOrderCommandHandler(IOrderRepository repository, IMapper mapper,
        ILogger<DeleteOrderCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderToDelete = await _repository.GetByIdAsync(request.Id);

        if (orderToDelete is null)
        {
            _logger.LogError("Order not exist on database.");

            throw new NotFoundException(nameof(Order), request.Id);
        }
        
        await _repository.DeleteAsync(orderToDelete);
        
        _logger.LogInformation($"Order {orderToDelete.Id} is successfully deleted.");
    }
}