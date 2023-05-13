using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Controllers
{
    public class VMLoginsController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly SingletonSum _singletonSummarySum;
        public VMLoginsController(WarehouseContext context, SingletonSum singletonSummarySum)
        {
            _context = context;
            _singletonSummarySum = singletonSummarySum;
        }

        // GET: VMLogins
        public async Task<IActionResult> Index()
        {
              return _context.VMLogin != null ? 
                          View(await _context.VMLogin.ToListAsync()) :
                          Problem("Entity set 'WarehouseContext.VMLogin'  is null.");
        }

        // GET: VMLogins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VMLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,User,Pwd")] VMLogin vMLogin)
        {
            if (ModelState.IsValid)
            {
                var users = _context.VMLogin.ToList();
                if (vMLogin.User == null || vMLogin.User.Length < 3 || vMLogin.Pwd == null || vMLogin.Pwd.Length < 3)
                {
                    ViewData["ValidateMessage"] = "Cannot create such user";
                    return View(vMLogin);
                }
                foreach (var userInUsers in users)
                {
                    if (vMLogin.User == userInUsers.User)
                    {
                        ViewData["ValidateMessage"] = "Cannot create such user";
                        return View(vMLogin);
                    }
                }
                _context.Add(vMLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login","Access");
            }
            return View(vMLogin);
        }

        // GET: VMLogins/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            if (id == null || _context.VMLogin == null)
            {
                return NotFound();
            }

            var vMLogin = await _context.VMLogin
                .FirstOrDefaultAsync(m => m.id == id);
            if (vMLogin == null)
            {
                return NotFound();
            }

            return View(vMLogin);
        }

        // POST: VMLogins/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            if (_context.VMLogin == null)
            {
                return Problem("Entity set 'WarehouseContext.VMLogin'  is null.");
            }
            var vMLogin = await _context.VMLogin.FindAsync(id);
            if (vMLogin != null)
            {
                _context.VMLogin.Remove(vMLogin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("LogOut","Home");
        }

        private bool VMLoginExists(int id)
        {
          return (_context.VMLogin?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
