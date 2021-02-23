using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Web.ViewComponents
{
    public class BasketItemCountViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public BasketItemCountViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // get logged in user id
            string userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // if not logged in, get anonymous user id from cookies
            if (userId == null)
                userId = Request.Cookies[Constants.BASKET_COOKIENAME];

            // if user id not exists, return 0 (it means there is no basket)
            if (userId == null)
                return Content("0");

            // if userId exists, get basket items count
            int count = await _basketService.BasketItemsCount(userId);
            return Content(count.ToString());
        }
    }
}
