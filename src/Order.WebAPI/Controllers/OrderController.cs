using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Order.Service;
using System;
using System.Threading.Tasks;
using System.Linq;
using Order.Model;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OrderService.WebAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetOrdersAsync();

            if (orders is null || orders.Any())
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(OrderDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                return Ok(order);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{status}")]
        [ProducesResponseType(typeof(IEnumerable<OrderStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderStatus()
        {
            var orders = await _orderService.GetOrderStatusAsync();
            if(orders is null || orders.Any())
            {
                return NotFound();
            }
            return Ok(orders);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(OrderStatus  orderStatus)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderStatus);

            return success ? (IActionResult)NoContent() : NotFound();
        }

        [HttpGet("{profits}")]
        [ProducesResponseType(typeof(IEnumerable<OrderProfit>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrderProfits()
        {
            var orderProfits = await _orderService.GetOrderProfitsAsync();

            if(orderProfits == null || !orderProfits.Any())
            {
                return NoContent();
            }

            return Ok(orderProfits);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder(OrderDetail order)
        {
            var id = await _orderService.CreateOrderAsync(order);

            return CreatedAtAction(nameof(GetOrderById), new { id }, id);
        }

    }
}
