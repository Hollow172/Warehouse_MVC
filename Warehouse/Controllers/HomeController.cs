using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Warehouse.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Warehouse.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Warehouse.Controllers
{
    [Authorize] 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WarehouseContext _context;
        private readonly SingletonSum _singletonSummarySum;
        private readonly SingletonCurrentlyLoggedInUser _singleton;

        public HomeController(ILogger<HomeController> logger, WarehouseContext context, SingletonSum singletonSummarySum, SingletonCurrentlyLoggedInUser singleton)
        {
            _logger = logger;
            _context = context;
            _singletonSummarySum = singletonSummarySum;
            _singleton = singleton;
        }

        public IActionResult Index()
        {
            ViewBag.uID = _singleton.getUserID();
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}