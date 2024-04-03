using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;

namespace Discount.Grpc.Services;

public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
{
    private readonly IDiscountRepository _repository;

    private readonly ILogger<DiscountService> _logger;

    public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

        return new CouponModel
        {
            Id = coupon.Id ?? 0,
            ProductName = coupon.ProductName,
            Description = coupon.Description,
            Amount = coupon.Amount,
        };
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
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

        var coupon = await _repository.GetDiscount(request.Coupon.ProductName);
        return new CouponModel
        {
            Id = coupon.Id ?? 0,
            ProductName = coupon.ProductName,
            Description = coupon.Description,
            Amount = coupon.Amount,
        };
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var updated = await _repository.UpdateDiscount(new Coupon
        {
            Id = request.Coupon.Id,
            ProductName = request.Coupon.ProductName,
            Description = request.Coupon.Description,
            Amount = request.Coupon.Amount
        });
        
        if (!updated)
        {
            throw new RpcException(new Status(
                StatusCode.Unavailable, 
                $"Discount with ProductName={request.Coupon.ProductName} cannot be updated.")
            );
        }
        
        var coupon = await _repository.GetDiscount(request.Coupon.ProductName);
        return new CouponModel
        {
            Id = coupon.Id ?? 0,
            ProductName = coupon.ProductName,
            Description = coupon.Description,
            Amount = coupon.Amount,
        };
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var deleted = await _repository.DeleteDiscount(request.ProductName);

        return new DeleteDiscountResponse
        {
            Success = deleted
        };
    }
}