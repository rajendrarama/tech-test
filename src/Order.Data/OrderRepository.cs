using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;

        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var orderEntities = await _orderContext.Order
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var orders = orderEntities.Select(x => new OrderSummary
            {
                Id = new Guid(x.Id),
                ResellerId = new Guid(x.ResellerId),
                CustomerId = new Guid(x.CustomerId),
                StatusId = new Guid(x.StatusId),
                StatusName = x.Status.Name,
                ItemCount = x.Items.Count,
                TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = x.CreatedDate
            });

            return orders;
        }        

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var orderIdBytes = orderId.ToByteArray();

            var order = await _orderContext.Order.SingleOrDefaultAsync(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes );
            if (order == null)
            {
                return null;
            }

            var orderDetail = new OrderDetail
            {
                Id = new Guid(order.Id),
                ResellerId = new Guid(order.ResellerId),
                CustomerId = new Guid(order.CustomerId),
                StatusId = new Guid(order.StatusId),
                StatusName = order.Status.Name,
                CreatedDate = order.CreatedDate,
                TotalCost = order.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value,
                TotalPrice = order.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value,
                Items = order.Items.Select(x => new Model.OrderItem
                {
                    Id = new Guid(x.Id),
                    OrderId = new Guid(x.OrderId),
                    ServiceId = new Guid(x.ServiceId),
                    ServiceName = x.Service.Name,
                    ProductId = new Guid(x.ProductId),
                    ProductName = x.Product.Name,
                    UnitCost = x.Product.UnitCost,
                    UnitPrice = x.Product.UnitPrice,
                    TotalCost = x.Product.UnitCost * x.Quantity.Value,
                    TotalPrice = x.Product.UnitPrice * x.Quantity.Value,
                    Quantity = x.Quantity.Value
                })
            };

            return orderDetail;
        }

        public async Task<IEnumerable<OrderProfit>> GetOrderProfitsAsync()
        {
            var orderProfits = await _orderContext.Order
                .GroupBy(o => new
                {
                    Month = o.CreatedDate.Month,
                    Year = o.CreatedDate.Year,
                    Profit = o.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value - o.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value
                })
                .Select(op => new OrderProfit
                {
                    Month = op.Key.Month,
                    Year = op.Key.Year,
                    Profit = op.Key.Profit
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync();
            return orderProfits;
        }
        public async Task<IEnumerable<Model.OrderStatus>> GetOrderStatusAsync()
        {
            var orderEntities = await _orderContext.Order
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var orders = orderEntities.Select(x => new Model.OrderStatus
            {
                Id = new Guid(x.Id),
                StatusName = x.Status.Name
            });

            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(Model.OrderStatus orderStatus)
        {
            var orderIdBytes = orderStatus.Id.ToByteArray();
            var entity = await _orderContext.Order.FirstOrDefaultAsync(order => order.Id == orderIdBytes);

            if(entity is null)
            {
                return false;
            }
            entity.Status.Name = orderStatus.StatusName;

            var count = await _orderContext.SaveChangesAsync();

            return count > 0;
        }

        public async Task<Guid> CreateOrderAsyc(OrderDetail orderDetail)
        {
            var order = new Entities.Order
            {
                ResellerId = orderDetail.ResellerId.ToByteArray(),
                CustomerId = orderDetail.CustomerId.ToByteArray(),
                StatusId = orderDetail.StatusId.ToByteArray(),
                CreatedDate = orderDetail.CreatedDate
            };

            order.Status.Id = orderDetail.StatusId.ToByteArray();
            order.Status.Name = orderDetail.StatusName;

            foreach(var ordDetail in orderDetail.Items)
            {
                order.Items.Add(new Entities.OrderItem
                {
                    ProductId = ordDetail.ProductId.ToByteArray(),
                    ServiceId = ordDetail.ServiceId.ToByteArray(),
                    Quantity = ordDetail.Quantity
                });
            }
            _orderContext.Add(order);
            await _orderContext.SaveChangesAsync();

            return new Guid(order.Id);
        }
    }
}
