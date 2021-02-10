using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IAsyncRepository<Category> _categoryRepository;
        private readonly IAsyncRepository<Brand> _brandRepository;

        public HomeIndexViewModelService(IAsyncRepository<Product> productRepository, IAsyncRepository<Category> categoryRepository, IAsyncRepository<Brand> brandRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
        }

        public async Task<List<SelectListItem>> GetBrandListItems()
        {
            return (await _brandRepository.ListAllAsync()).Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
        }

        public async Task<List<SelectListItem>> GetCategoryListItems()
        {
            return (await _categoryRepository.ListAllAsync()).Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
        }

        public async Task<HomeIndexViewModel> GetHomeIndexViewModel(int? categoryId, int? brandId)
        {
            var spec = new ProductFilterPaginatedSpecification(categoryId, brandId);
            var products = await _productRepository.ListAsync(spec);

            return new HomeIndexViewModel()
            {
                Products = products.Select(x => new ProductViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    PictureUri = x.PictureUri
                }).ToList(),
                Categories = await GetCategoryListItems(),
                Brands = await GetBrandListItems()
            };
        }
    }
}
