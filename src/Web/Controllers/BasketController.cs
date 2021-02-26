using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
        private readonly IOrderService _orderService;

        public BasketController(IBasketViewModelService basketViewModelService, SignInManager<ApplicationUser> signInManager, IBasketService basketService, IOrderService orderService)
        {
            _basketViewModelService = basketViewModelService;
            _signInManager = signInManager;
            _basketService = basketService;
            _orderService = orderService;
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

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = GetOrCreateUserId();
            return Json(await _basketViewModelService.UpdateQuantity(userId, productId, quantity));
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var vm = await CreateCheckoutViewModel();
            return View(vm);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            var newVm = await CreateCheckoutViewModel();
            if (vm.BasketItemsJson != newVm.BasketItemsJson)
            {
                ModelState.Remove("BasketItemsJson"); // renew its value and get it from newVm
                ModelState.AddModelError("BasketItemsJson", "Your basket has been updated. Please review the items before making a payment.");
            }

            if (ModelState.IsValid)
            {
                // receive the payment

                // process the order
                var address = new Address()
                {
                    City = vm.City,
                    Country = vm.Country,
                    State = vm.State,
                    Street = vm.Street,
                    ZipCode = vm.ZipCode
                };
                int orderId = await _orderService.CreateOrderAsync(newVm.BasketId, vm.FirstName, vm.LastName, address);

                // delete the basket
                await _basketService.DeleteBasketAsync(newVm.BasketId);

                // redirect to success page
                return RedirectToAction("Success", new { orderId = orderId });
            }

            vm.BasketItems = newVm.BasketItems;
            vm.PaymentTotal = newVm.PaymentTotal;
            vm.BasketItemsJson = newVm.BasketItemsJson;
            return View(vm);
        }

        public async Task<IActionResult> Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        private async Task<CheckoutViewModel> CreateCheckoutViewModel()
        {
            string userId = GetOrCreateUserId();
            var basket = await _basketViewModelService.GetBasket(userId);
            return new CheckoutViewModel()
            {
                BasketId = basket.Id,
                BasketItems = basket.Items,
                PaymentTotal = basket.Total(),
                BasketItemsJson = JsonSerializer.Serialize(basket.Items)
            };
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
