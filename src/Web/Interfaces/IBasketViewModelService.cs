using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IBasketViewModelService
    {
        Task<BasketViewModel> GetBasket(string userId);
        Task<ProductViewModel> GetProductDetails(int productId);
        Task<BasketUpdateQuantityViewModel> UpdateQuantity(string userId, int productId, int quantity);
        Task TransferBasketAsync(string userName);
    }
}
