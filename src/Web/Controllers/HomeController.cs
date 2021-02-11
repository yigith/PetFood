using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeIndexViewModelService _homeIndexViewModelService;

        public HomeController(ILogger<HomeController> logger, IHomeIndexViewModelService homeIndexViewModelService)
        {
            _logger = logger;
            _homeIndexViewModelService = homeIndexViewModelService;
        }

        public async Task<IActionResult> Index(int? categoryId, int? brandId, int pageId = 1)
        {
            return View(await _homeIndexViewModelService.GetHomeIndexViewModel(categoryId, brandId, pageId));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
