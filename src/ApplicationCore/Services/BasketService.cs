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
        private readonly IAsyncRepository<BasketItem> _basketItemRepository;

        public BasketService(IAsyncRepository<Basket> basketRepository, IAsyncRepository<BasketItem> basketItemRepository)
        {
            _basketRepository = basketRepository;
            _basketItemRepository = basketItemRepository;
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

        public async Task<int> BasketItemsCount(string buyerId)
        {
            if (!await BasketExistsAsync(buyerId))
                return 0;

            var basketId = await GetBasketIdAsync(buyerId);
            var spec = new BasketItemsSpecification(basketId);
            return await _basketItemRepository.CountAsync(spec);
        }

        public async Task<int> CreateBasketAsync(string buyerId)
        {
            Basket basket = new Basket() { BuyerId = buyerId };
            basket = await _basketRepository.AddAsync(basket);
            return basket.Id;
        }

        public async Task DeleteBasketAsync(int basketId)
        {
            var basket = await _basketRepository.GetByIdAsync(basketId);
            await _basketRepository.DeleteAsync(basket);
        }

        public async Task<int> GetBasketIdAsync(string buyerId)
        {
            var basket = await _basketRepository.FirstAsync(new BasketExistsSpecification(buyerId));
            return basket.Id;
        }

        public async Task TransferBasketAsync(string anonymousId, string userId)
        {
            // get anonymous basket
            var specAnon = new BasketWithItemsSpecification(anonymousId);
            var anonBasket = await _basketRepository.FirstOrDefaultAsync(specAnon);
            if (anonBasket == null)
                return; // there is no anonymous basket to transfer from

            // get or create user basket
            var specUser = new BasketWithItemsSpecification(userId);
            var userBasket = await _basketRepository.FirstOrDefaultAsync(specUser);
            if (userBasket == null)
            {
                userBasket = new Basket() { BuyerId = userId, Items = new List<BasketItem>() };
                await _basketRepository.AddAsync(userBasket);
            }

            // transfer items
            foreach (var basketItem in anonBasket.Items)
            {
                var userBasketItem = userBasket.Items.FirstOrDefault(x => x.ProductId == basketItem.ProductId);

                if (userBasketItem != null)
                {
                    userBasketItem.Quantity += basketItem.Quantity;
                }
                else
                {
                    userBasket.Items.Add(new BasketItem()
                    {
                        ProductId = basketItem.ProductId,
                        Quantity = basketItem.Quantity,
                        UnitPrice = basketItem.UnitPrice
                    });
                }
            }
            await _basketRepository.UpdateAsync(userBasket);

            // delete anonymous basket
            await DeleteBasketAsync(anonBasket.Id);
        }
    }
}
