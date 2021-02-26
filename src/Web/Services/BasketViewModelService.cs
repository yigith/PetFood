using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IAsyncRepository<Product> _productRepository;
        private readonly IBasketService _basketService;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public BasketViewModelService(IAsyncRepository<Product> productRepository, IBasketService basketService, IAsyncRepository<Basket> basketRepository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _basketService = basketService;
            _basketRepository = basketRepository;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<BasketViewModel> GetBasket(string userId)
        {
            var vm = new BasketViewModel()
            {
                BuyerId = userId,
                Items = new List<BasketItemViewModel>(),
            };

            // sepet yoksa boş bir listeli sepet döndür
            if (!await _basketService.BasketExistsAsync(userId))
                return vm;

            var basket = await _basketRepository.FirstOrDefaultAsync(new BasketWithItemsSpecification(userId));
            var productIds = basket.Items.Select(x => x.ProductId).ToArray();
            var basketProducts = await _productRepository.ListAsync(new ProductsSpecification(productIds));

            vm.Id = basket.Id;
            vm.Items = basket.Items.Select(x =>
            {
                var product = basketProducts.First(p => p.Id == x.ProductId);
                return new BasketItemViewModel()
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    ProductName = product.Name,
                    PictureUri = product.PictureUri
                };
            }).ToList();

            return vm;
        }

        public async Task<ProductViewModel> GetProductDetails(int productId)
        {
            Product product = await _productRepository.GetByIdAsync(productId);

            return new ProductViewModel()
            {
                Id = product.Id,
                Name = product.Name,
                PictureUri = product.PictureUri,
                Price = product.Price
            };
        }

        public async Task TransferBasketAsync(string userName)
        {
            var anonymousId = _httpContextAccessor.HttpContext.Request.Cookies[Constants.BASKET_COOKIENAME];
            if (anonymousId == null) return;

            // find user by username (=email)
            var user = await _userManager.FindByNameAsync(userName);
            await _basketService.TransferBasketAsync(anonymousId, user.Id);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(Constants.BASKET_COOKIENAME);
        }

        public async Task<BasketUpdateQuantityViewModel> UpdateQuantity(string userId, int productId, int quantity)
        {
            var basket = await _basketRepository.FirstOrDefaultAsync(new BasketWithItemsSpecification(userId));
            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            basketItem.Quantity = quantity;

            if (quantity == 0)
                basket.Items.Remove(basketItem);
            
            await _basketRepository.UpdateAsync(basket);
            return new BasketUpdateQuantityViewModel()
            {
                ProductPrice = string.Format("${0:0.00}", basketItem.UnitPrice * basketItem.Quantity),
                TotalPrice = string.Format("${0:0.00}", basket.Items.Sum(x => x.UnitPrice * x.Quantity)),
                BasketItemsCount = basket.Items.Count
            };
        }
    }
}
