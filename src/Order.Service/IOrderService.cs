using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);

        Task<IEnumerable<OrderStatus>> GetOrderStatusAsync();

        Task<bool> UpdateOrderStatusAsync(OrderStatus orderStatus);

        Task<IEnumerable<OrderProfit>> GetOrderProfitsAsync();

        Task<Guid> CreateOrderAsync(OrderDetail orderDetail);
    }
}
