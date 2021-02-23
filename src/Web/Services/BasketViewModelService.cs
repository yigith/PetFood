using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
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

        public BasketViewModelService(IAsyncRepository<Product> productRepository, IBasketService basketService, IAsyncRepository<Basket> basketRepository)
        {
            _productRepository = productRepository;
            _basketService = basketService;
            _basketRepository = basketRepository;
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
    }
}
