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
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly SingletonSum _singletonSummarySum;
        private readonly SingletonCurrentlyLoggedInUser _singleton;


        public CategoriesController(WarehouseContext context, SingletonSum singletonSummarySum, SingletonCurrentlyLoggedInUser singleton)
        {
            _context = context;
            _singletonSummarySum = singletonSummarySum;
            _singleton = singleton;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();

            return _context.Category != null ? 
                          View(await _context.Category.ToListAsync()) :
                          Problem("Entity set 'WarehouseContext.Category'  is null.");
        }

    }
}
