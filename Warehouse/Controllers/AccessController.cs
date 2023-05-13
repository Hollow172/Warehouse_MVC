using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Warehouse.Models;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;

namespace Warehouse.Controllers
{
    public class AccessController : Controller
    {

        private readonly SingletonCurrentlyLoggedInUser _singleton;
        private readonly SingletonSum _singletonSummarySum;
        private readonly WarehouseContext _context;

        public AccessController(SingletonCurrentlyLoggedInUser singleton, SingletonSum singletonSummarySum, WarehouseContext context)
        {
            _singleton = singleton;
            _singletonSummarySum = singletonSummarySum;
            _context = context;
        }

        public IActionResult Login()
        {
            _singletonSummarySum.setSums(_context);
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            var listOfUsers = _context.VMLogin.Where(p => p.User == modelLogin.User && p.Pwd == modelLogin.Pwd).ToList();

            if (listOfUsers.Count > 0)
            {

                List<Claim> claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, modelLogin.User)
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = false
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);

                _singleton.setLogin(modelLogin);
                _singleton.setID(listOfUsers.First().id);
                var idtmp = _singleton.getUserID();
                _singletonSummarySum.setSums(_context);
                return RedirectToAction("Index", "Home");
            }

            ViewData["ValidateMessage"] = "user not found";
            return View();
        }

        public IActionResult CreateUser()
        {
            if (TempData.ContainsKey("unSuccessMessage"))
            {
                ViewData["ValidateMessage"] = TempData["unSuccessMessage"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(VMLogin newUser)
        {
            if (ReadingUsers.AddUser(newUser, _context))
            {
                ViewData["ValidateMessage"] = "successfuly created new user";
                return View();
            }
            TempData["unSuccessMessage"] = "cannot create such user";
            return RedirectToAction("CreateUser");
        }

        public IActionResult DeleteUser()
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(VMLogin login)
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            var currentUser = _singleton.getLogin();
            ReadingUsers.DeleteUser(currentUser);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");

        }

        public IActionResult GoToMainPage()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult GoToCreatingUser()
        {
            return RedirectToAction("CreateUser", "Access");
        }

        public IActionResult GoToDeletingUser()
        {
            return RedirectToAction("DeleteUser", "Access");
        }

    }
}
