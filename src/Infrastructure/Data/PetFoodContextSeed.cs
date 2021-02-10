using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public static class PetFoodContextSeed
    {
        public static async Task SeedAsync(PetFoodContext petFoodContext)
        {
            if (!await petFoodContext.Categories.AnyAsync())
            {
                await petFoodContext.Categories.AddAsync(new Category() { Name = "Cat Food" });
                await petFoodContext.SaveChangesAsync();
                await petFoodContext.Categories.AddAsync(new Category() { Name = "Dog Food" });
                await petFoodContext.SaveChangesAsync();
            }
            if (!await petFoodContext.Brands.AnyAsync())
            {
                await petFoodContext.Brands.AddAsync(new Brand() { Name = "Pro Plan" });
                await petFoodContext.SaveChangesAsync();
                await petFoodContext.Brands.AddAsync(new Brand() { Name = "Royal Canin" });
                await petFoodContext.SaveChangesAsync();
                await petFoodContext.Brands.AddAsync(new Brand() { Name = "Purina" });
                await petFoodContext.SaveChangesAsync();
            }
            if (!await petFoodContext.Products.AnyAsync())
            {
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 1, Name = "Pro Plan Cat Adult Chicken & Rice Dry Cat Food - 10kg", Price = 48.40m, PictureUri = "samples/1.png" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 1, Name = "Pro Plan Light Weight Management Adult Turkey Dry Cat Food - 10kg", Price = 53.90m, PictureUri = "samples/2.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 2, Name = "Royal Canin Feline Senior Ageing 12+ Dry Cat Food - 4kg", Price = 37.99m, PictureUri = "samples/3.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 2, Name = "Royal Canin Adult Pure Feline No. 3 Lively Dry Cat Food - 300g", Price = 3.29m, PictureUri = "samples/4.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 2, Name = "Royal Canin Sensible 33 Dry Adult Cat Food - 4kg", Price = 28.99m, PictureUri = "samples/5.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 1, BrandId = 3, Name = "Purina Veterinary Diet Feline EN ST/OX Gastroenteric Dry Cat Food - 1.5Kg", Price = 16.49m, PictureUri = "samples/6.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 1, Name = "Pro Plan Medium Chicken Adult Dry Dog Food - 14kg", Price = 34.64m, PictureUri = "samples/7.png" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 1, Name = "Pro Plan Medium Chicken Puppy Dry Dog Food - 12kg", Price = 33.40m, PictureUri = "samples/8.png" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 2, Name = "Royal Canin Labrador Retriever Adult Dry Dog Food - 12kg", Price = 46.98m, PictureUri = "samples/9.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 3, Name = "Purina Veterinary Diet Canine HA Hypoallergenic Dry Dog Food - 3Kg", Price = 28.58m, PictureUri = "samples/10.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 3, Name = "Purina Veterinary Diet Canine UR Urinary Dry Dog Food - 12Kg", Price = 55.54m, PictureUri = "samples/11.jpg" });
                await petFoodContext.Products.AddAsync(new Product() { CategoryId = 2, BrandId = 1, Name = "Pro Plan Small & Mini Chicken Adult Dry Dog Food - 3kg", Price = 16.82m, PictureUri = "samples/12.png" });
                await petFoodContext.SaveChangesAsync();
            }
        }
    }
}
