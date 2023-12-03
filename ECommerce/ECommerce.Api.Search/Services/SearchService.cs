using ECommerce.Api.Search.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomerService customerService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomerService customerService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var orderResult = await ordersService.GetOrdersAsync(customerId);
            var productResult = await productsService.GetProductsAsync();
            var customerResult = await customerService.GetCustomerAsync(customerId);

            if (orderResult.IsSuccess)
            {

                foreach (var order in orderResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productResult.IsSuccess ?
                            productResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name :
                            "Product information is not available";
                    }
                }

                var result = new
                {
                    Customer = customerResult.IsSuccess ? 
                                customerResult.Customer :
                                new {Name = "Customer information is not available"},
                    Orders = orderResult.Orders,
                };
                return (true, result);
            }
            return (false, null);
        }
    }
}
