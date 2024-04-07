using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Models;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder;

public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
{
    private readonly IOrderRepository _repository;

    private readonly IMapper _mapper;

    private readonly IEmailService _emailService;

    private readonly ILogger<CheckoutOrderCommandHandler> _logger;

    public CheckoutOrderCommandHandler(IOrderRepository repository, IMapper mapper, IEmailService emailService,
        ILogger<CheckoutOrderCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = _mapper.Map<Order>(request);
        var newOrder = await _repository.AddAsync(orderEntity);
        
        _logger.LogInformation($"Order {newOrder.Id} is successfully created.");

        await this.SendMail(newOrder);
        
        return newOrder.Id;
    }

    private async Task SendMail(Order order)
    {
        var email = new Email
        {
            To = "wakizashi1024@outlook.com",
            Body = $"Order was created.",
            Subject = $"Order was created",
        };
        
        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception e)
        {
            _logger.LogError($"$Order {order.Id} failed due to an error with the mail service: {e.Message}");
        }
    }
}