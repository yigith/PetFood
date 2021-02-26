using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IAsyncRepository<Order> _orderRepository;

        public OrderService(IAsyncRepository<Basket> basketRepository, IAsyncRepository<Product> productRepository, IAsyncRepository<Order> orderRepository)
        {
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<int> CreateOrderAsync(int basketId, string firstName, string lastName, Address address)
        {
            // get the basket with items
            var specBasket = new BasketWithItemsSpecification(basketId);
            var basket = await _basketRepository.FirstOrDefaultAsync(specBasket);

            // get the list of products in basket
            var specProducts = new ProductsSpecification(basket.Items.Select(x => x.ProductId).ToArray());
            var products = await _productRepository.ListAsync(specProducts);

            // create order
            var order = new Order()
            {
                BuyerId = basket.BuyerId,
                FirstName = firstName,
                LastName = lastName,
                ShipToAddress = address,
                OrderItems = basket.Items.Select(x => {
                    var product = products.First(p => p.Id == x.ProductId);
                    return new OrderItem()
                    {
                        ProductId = x.ProductId,
                        ProductName = product.Name,
                        PictureUri = product.PictureUri,
                        UnitPrice = x.UnitPrice,
                        Units = x.Quantity
                    };
                }).ToList()
            };

            // save order
            order = await _orderRepository.AddAsync(order);
            return order.Id;
        }
    }
}
