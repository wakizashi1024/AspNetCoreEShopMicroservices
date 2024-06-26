﻿using System.Net;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountRepository _repository;

    public DiscountController(IDiscountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    [Route("{productName}", Name = "GetDiscount")]
    [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> GetDiscount(string productName)
    {
        var coupon = await _repository.GetDiscount(productName);

        return Ok(coupon);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Coupon), (int)HttpStatusCode.Created)]
    public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
    {
        await _repository.CreateDiscount(coupon);

        return CreatedAtRoute("GetDiscount", 
            new { productName = coupon.ProductName }, 
            await _repository.GetDiscount(coupon.ProductName)
            );
    }

    [HttpPut]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
    {
        return Ok(await _repository.UpdateDiscount(coupon));
    }

    [HttpDelete]
    [Route("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteDiscount(string productName)
    {
        return Ok(await _repository.DeleteDiscount(productName));
    }
}