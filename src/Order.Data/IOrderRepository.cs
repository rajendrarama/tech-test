using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Data
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();

        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);

        Task<IEnumerable<OrderStatus>> GetOrderStatusAsync();

        Task<bool> UpdateOrderStatusAsync(OrderStatus orderStatus);

        Task<IEnumerable<OrderProfit>> GetOrderProfitsAsync();

        Task<Guid>CreateOrderAsyc(OrderDetail orderDetail);
    }
}
