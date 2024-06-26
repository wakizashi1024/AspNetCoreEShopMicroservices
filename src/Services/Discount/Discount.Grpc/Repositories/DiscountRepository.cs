﻿using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly IConfiguration _configuration;

    public DiscountRepository(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<Coupon> GetDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(
            _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
            );
        
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(
            "SELECT ID, ProductName, Description, Amount FROM Coupon WHERE ProductName=@ProductName", 
            new
                {
                    ProductName = productName,
                });

        if (coupon == null)
        {
            return new Coupon
            {
                ProductName = "No Discount",
                Description = "No Discount Desc",
                Amount = 0,
            };
        }

        return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(
            _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
        );

        var affected = await connection.ExecuteAsync(
            @"INSERT INTO Coupon (ProductName, Description, Amount) 
                VALUES (@ProductName, @Description, @Amount)",
            new
            {
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
            });

        return affected != 0;
    }

    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(
            _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
        );

        var affected = await connection.ExecuteAsync(
            @"UPDATE Coupon 
                SET ProductName=@ProductName, Description=@Description, Amount=@Amount
                WHERE ID=@Id",
            new
            {
                Id = coupon.Id,
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
            });

        return affected != 0;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(
            _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
        );

        var affected = await connection.ExecuteAsync(
            @"DELETE FROM Coupon 
                WHERE ProductName=@ProductName",
            new
            {
                ProductName = productName,
            });

        return affected != 0;
    }
}