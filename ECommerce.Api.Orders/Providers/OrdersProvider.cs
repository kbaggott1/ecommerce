using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly OrdersDbContext dbContext;
        private readonly ILogger<OrdersProvider> logger;
        private readonly IMapper mapper;

        public OrdersProvider(OrdersDbContext dbContext, ILogger<OrdersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Orders.Any())
            {
                List<Db.OrderItem> order1 = new List<Db.OrderItem>()
                {
                    new Db.OrderItem() { Id = 1, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 12},
                    new Db.OrderItem() { Id = 2,  OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 5 }
                };

                List<Db.OrderItem> order2 = new List<Db.OrderItem>()
                {
                    new Db.OrderItem() { Id = 3, OrderId = 2, ProductId = 1, Quantity = 3, UnitPrice = 5 }
                };

                List<Db.OrderItem> order3 = new List<Db.OrderItem>()
                {
                    new Db.OrderItem() { Id = 4, OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 25 }
                };

                List<Db.OrderItem> order4 = new List<Db.OrderItem>()
                {
                    new Db.OrderItem() { Id = 5, OrderId = 4, ProductId = 2, Quantity = 2, UnitPrice = 12 }
                };

                foreach (Db.OrderItem item in order1)
                {
                    dbContext.Add(item);
                }
                foreach (Db.OrderItem item in order2)
                {
                    dbContext.Add(item);
                }
                foreach (Db.OrderItem item in order3)
                {
                    dbContext.Add(item);
                }
                foreach (Db.OrderItem item in order4)
                {
                    dbContext.Add(item);
                }

                dbContext.Orders.Add(new Db.Order() { Id = 1, CustomerId = 1, OrderDate = DateTime.Now, Items = order1 });
                dbContext.Orders.Add(new Db.Order() { Id = 2, CustomerId = 1, OrderDate = DateTime.Now, Items = order2 });
                dbContext.Orders.Add(new Db.Order() { Id = 3, CustomerId = 2, OrderDate = DateTime.Now, Items = order3 });
                dbContext.Orders.Add(new Db.Order() { Id = 4, CustomerId = 3, OrderDate = DateTime.Now, Items = order4 });

                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await dbContext.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> Orders, string ErrorMessage)> GetOrdersAsync()
        {
            try
            {
                var orders = await dbContext.Orders.ToListAsync();
                if (orders != null && orders.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
