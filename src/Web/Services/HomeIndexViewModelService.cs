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
    public class HomeIndexViewModelService : IHomeIndexViewModelService
    {
        private readonly IAsyncRepository<Product> _productRepository;

        public HomeIndexViewModelService(IAsyncRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<HomeIndexViewModel> GetHomeIndexViewModel()
        {
            var products = await _productRepository.ListAllAsync();

            return new HomeIndexViewModel()
            {
                Products = products.Select(x => new ProductViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PictureUri = x.PictureUri
                }).ToList()
            };
        }
    }
}
