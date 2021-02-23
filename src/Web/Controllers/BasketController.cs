using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBasketService _basketService;

        public BasketController(IBasketViewModelService basketViewModelService, SignInManager<ApplicationUser> signInManager, IBasketService basketService)
        {
            _basketViewModelService = basketViewModelService;
            _signInManager = signInManager;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = GetOrCreateUserId();
            return View(await _basketViewModelService.GetBasket(userId));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBasket(int id, int quantity = 1)
        {
            // get product details
            var product = await _basketViewModelService.GetProductDetails(id);

            // get/create user id
            string userId = GetOrCreateUserId();

            // create basket if not exists and get basket id
            int basketId;
            if (!await _basketService.BasketExistsAsync(userId))
                basketId = await _basketService.CreateBasketAsync(userId);
            else
                basketId = await _basketService.GetBasketIdAsync(userId);

            // add item to the basket
            int count = await _basketService.AddItemToBasketAsync(basketId, product.Id, product.Price, quantity);

            return Json(new { BasketItemCount = count });
        }

        private string GetOrCreateUserId()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                if (Request.Cookies[Constants.BASKET_COOKIENAME] != null)
                {
                    return Request.Cookies[Constants.BASKET_COOKIENAME];
                }
                else
                {
                    var guid = Guid.NewGuid().ToString();
                    var cookieOptions = new CookieOptions();
                    cookieOptions.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Append(Constants.BASKET_COOKIENAME, guid, cookieOptions);
                    return guid;
                }
            }
        }
    }
}
