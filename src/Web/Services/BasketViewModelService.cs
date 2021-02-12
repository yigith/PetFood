using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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

        public BasketViewModelService(IAsyncRepository<Product> productRepository)
        {
            _productRepository = productRepository;
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
