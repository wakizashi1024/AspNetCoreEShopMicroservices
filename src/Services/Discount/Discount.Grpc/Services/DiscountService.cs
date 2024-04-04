using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _repository;

    private readonly IMapper _mapper;

    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDiscountRepository repository, IMapper mapper, ILogger<DiscountService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await _repository.GetDiscount(request.ProductName);
        if (coupon == null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound, 
                $"Discount with ProductName={request.ProductName} is not found.")
            );
        }
        
        _logger.LogInformation(
            $"Discount is retrieved for ProductName: {coupon.ProductName}, Amount: {coupon.Amount}"
        );

        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request.Coupon);
        
        var inserted = await _repository.CreateDiscount(
        new Coupon
            {
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount,
            }
        );
        if (!inserted)
        {
            throw new RpcException(new Status(
                StatusCode.Unavailable, 
                $"Discount with ProductName={request.Coupon.ProductName} cannot be created.")
            );
        }
        
        _logger.LogInformation(
            $"Discount is inserted for ProductName: {request.Coupon.ProductName}, Amount: {request.Coupon.Amount}"
        );

        coupon = await _repository.GetDiscount(request.Coupon.ProductName);
        
        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = _mapper.Map<Coupon>(request.Coupon);
        var updated = await _repository.UpdateDiscount(coupon);
        
        if (!updated)
        {
            throw new RpcException(new Status(
                StatusCode.Unavailable, 
                $"Discount with ProductName={request.Coupon.ProductName} cannot be updated.")
            );
        }
        
        _logger.LogInformation(
            $"Discount is updated for ProductName: {request.Coupon.ProductName}, Amount: {request.Coupon.Amount}"
        );
        
        coupon = await _repository.GetDiscount(request.Coupon.ProductName);
        
        return _mapper.Map<CouponModel>(coupon);
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var deleted = await _repository.DeleteDiscount(request.ProductName);

        if (!deleted)
        {
            throw new RpcException(new Status(
                StatusCode.Unavailable, 
                $"Discount with ProductName={request.ProductName} cannot be deleted.")
            );
        }
        
        _logger.LogInformation(
            $"Discount is deleted for ProductName: {request.ProductName}"
        );
        
        return new DeleteDiscountResponse
        {
            Success = deleted
        };
    }
}