using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AddToBasket(ProductViewModel productDetails)
        {
            // todo: sepet yoksa oluştur

            // todo: oluşan sepeti ilgili ürünü ekle

            return Json(new { BasketItemCount = 0 });
        }
    }
}
