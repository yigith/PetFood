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
    public class BasketService : IBasketService
    {
        private readonly IAsyncRepository<Basket> _basketRepository;

        public BasketService(IAsyncRepository<Basket> basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<int> AddItemToBasketAsync(int basketId, int productId, decimal price, int quantity = 1)
        {
            // get basket with items
            var spec = new BasketWithItemsSpecification(basketId);
            Basket basket = await _basketRepository.FirstOrDefaultAsync(spec);

            // get basket item if exists
            BasketItem basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);

            // if exists increase quantity
            if (basketItem != null)
            {
                basketItem.Quantity += quantity;
            }
            // if not exists add new basket item
            else
            {
                basketItem = new BasketItem() 
                { 
                    BasketId = basketId, 
                    ProductId = productId, 
                    Quantity = quantity, 
                    UnitPrice = price 
                };
                basket.Items.Add(basketItem);
            }

            // update basket
            await _basketRepository.UpdateAsync(basket);

            return basket.Items.Count;
        }

        public async Task<bool> BasketExistsAsync(string buyerId)
        {
            return await _basketRepository.CountAsync(new BasketExistsSpecification(buyerId)) > 0;
        }

        public async Task<int> CreateBasketAsync(string buyerId)
        {
            Basket basket = new Basket() { BuyerId = buyerId };
            basket = await _basketRepository.AddAsync(basket);
            return basket.Id;
        }

        public async Task<int> GetBasketIdAsync(string buyerId)
        {
            var basket = await _basketRepository.FirstAsync(new BasketExistsSpecification(buyerId));
            return basket.Id;
        }
    }
}
