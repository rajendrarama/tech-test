﻿using Order.Data;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatusAsync()
        {
            var orders = await _orderRepository.GetOrderStatusAsync();
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(OrderStatus orderStatus)
        {
            var result = await _orderRepository.UpdateOrderStatusAsync(orderStatus);
            return result;
        }

        public async Task<IEnumerable<OrderProfit>> GetOrderProfitsAsync()
        {
            var result = await _orderRepository.GetOrderProfitsAsync();

            return result;
        }

        public async Task<Guid> CreateOrderAsync(OrderDetail orderDetail)
        {
            var result = await _orderRepository.CreateOrderAsyc(orderDetail);

            return result;
        }
    }
}
