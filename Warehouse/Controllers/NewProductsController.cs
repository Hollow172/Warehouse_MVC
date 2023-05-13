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
    public class NewProductsController : Controller
    {
        private readonly WarehouseContext _context;

        private readonly SingletonSum _singletonSummarySum;
        private readonly SingletonCurrentlyLoggedInUser _singleton;

        public NewProductsController(WarehouseContext context,  SingletonSum singletonSummarySum, SingletonCurrentlyLoggedInUser singleton)
        {
            _context = context;
            _singletonSummarySum = singletonSummarySum;
            _singleton = singleton;
        }

        // GET: NewProducts
        public async Task<IActionResult> Index()
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();

            var warehouseContext = _context.NewProduct.Include(n => n.category);
            return View(await warehouseContext.ToListAsync());
        }

        // GET: NewProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NewProduct == null)
            {
                return NotFound();
            }

            var newProduct = await _context.NewProduct
                .Include(n => n.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (newProduct == null)
            {
                return NotFound();
            }

            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();

            return View(newProduct);
        }

        // GET: NewProducts/Create
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_context.Category, "id", "name");
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();

            return View();
        }

        // POST: NewProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,code,name,description,categoryId,quantity,measure,price,Version")] NewProduct newProduct)
        {
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();
            if (ModelState.IsValid)
            {
                var list = _context.NewProduct.Where(p => p.code == newProduct.code).ToList();
                if (list.Count > 0)
                {
                    ViewData["ValidateMessage"] = "Such code is already assigned to another product";
                    ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", newProduct.categoryId);
                    return View(newProduct);
                }

                _context.Add(newProduct);
                await _context.SaveChangesAsync();
                _singletonSummarySum.setSums(_context);
                return RedirectToAction(nameof(Index));
            }
            ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", newProduct.categoryId);
            return View(newProduct);
        }

        // GET: NewProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NewProduct == null)
            {
                return NotFound();
            }

            var newProduct = await _context.NewProduct.FindAsync(id);
            if (newProduct == null)
            {
                return NotFound();
            }
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewBag.uID = _singleton.getUserID();

            ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", newProduct.categoryId);
            return View(newProduct);
        }

        // POST: NewProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("id,code,name,description,categoryId,quantity,measure,price,Version")] NewProduct newProduct)
        public async Task<IActionResult> Edit(int? id, byte[] version)
        {

            ViewBag.uID = _singleton.getUserID();

            if (id == null)
            {
                return NotFound();
            }

            var productToUpdate = await _context.NewProduct
                .FirstOrDefaultAsync(p => p.id == id);

            if (productToUpdate == null)
            {
                NewProduct deletedProduct = new NewProduct();
                await TryUpdateModelAsync(deletedProduct);
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to save changes. The product was deleted by another user."
                );
                _singletonSummarySum.setSums(_context);
                ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", deletedProduct.categoryId);
                return View(deletedProduct);
            }

            _context.Entry(productToUpdate).Property("Version").OriginalValue = version;

            if (await TryUpdateModelAsync<NewProduct>(
                productToUpdate,
                "",
                p => p.code, p => p.name, p => p.description, p => p.quantity, p => p.price, p => p.measure, p=>p.categoryId
            ))
            {
                try
                {
                    var list = _context.NewProduct.Where(p => p.id != productToUpdate.id).Where(p => p.code == productToUpdate.code).ToList();
                    if (list.Count > 0)
                    {
                        ViewData["ValidateMessage"] = "Such code is already assigned to another product";
                        ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", productToUpdate.categoryId);
                        ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                        ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                        ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                        return View(productToUpdate);
                    }

                    await _context.SaveChangesAsync();
                    ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", productToUpdate.categoryId);
                    _singletonSummarySum.setSums(_context);
                    ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                    ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                    ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (NewProduct)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The product was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (NewProduct)databaseEntry.ToObject();

                        if (databaseValues.code != clientValues.code)
                        {
                            ModelState.AddModelError("Code", $"Current value: {databaseValues.code}");
                        }

                        if (databaseValues.name != clientValues.name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.name}");
                        }

                        if (databaseValues.description != clientValues.description)
                        {
                            ModelState.AddModelError("Description", $"Current value: {databaseValues.description}");
                        }

                        if (databaseValues.categoryId != clientValues.categoryId)
                        {
                            var c = await _context.Category.FirstAsync(cat => cat.id == databaseValues.categoryId);
                            ModelState.AddModelError("CategoryId", $"Current value: {c.name}");
                        }

                        if (databaseValues.quantity != clientValues.quantity)
                        {
                            ModelState.AddModelError("Quantity", $"Current value: {databaseValues.quantity}");
                        }

                        if (databaseValues.measure != clientValues.measure)
                        {
                            ModelState.AddModelError("Measure", $"Current value: {databaseValues.measure}");
                        }

                        if (databaseValues.price != clientValues.price)
                        {
                            ModelState.AddModelError("Price", $"Current value: {databaseValues.price}");
                        }


                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");

                        productToUpdate.Version = databaseValues.Version;
                        _singletonSummarySum.setSums(_context);
                        ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                        ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                        ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                        ModelState.Remove("Version");
                    }
                }
            }

            _singletonSummarySum.setSums(_context);
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            ViewData["categoryId"] = new SelectList(_context.Category, "id", "name", productToUpdate.categoryId);
            return View(productToUpdate);
        }

        // GET: NewProducts/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {

            ViewBag.uID = _singleton.getUserID();

            if (id == null || _context.NewProduct == null)
            {
                return NotFound();
            }

            var newProduct = await _context.NewProduct
                .Include(n => n.category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.id == id);
            if (newProduct == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    _singletonSummarySum.setSums(_context);
                    ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                    ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                    ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                    return RedirectToAction(nameof(Index));
                }
                _singletonSummarySum.setSums(_context);
                ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            _singletonSummarySum.setSums(_context);
            ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
            ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
            ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
            return View(newProduct);
        }

        // POST: NewProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(NewProduct product)
        {
            try
            {
                if (await _context.NewProduct.AnyAsync(m => m.id == product.id))
                {
                    _context.NewProduct.Remove(product);
                    _singletonSummarySum.setSums(_context);
                    await _context.SaveChangesAsync();
                }
                ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.wSum = _singletonSummarySum.getWarehouseSum();
                ViewBag.cSum = _singletonSummarySum.getCategoriesSum();
                ViewBag.cIDs = _singletonSummarySum.getCategoriesIDs();
                _singletonSummarySum.setSums(_context);
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = product.id });
            }
        }

        private bool NewProductExists(int id)
        {
          return (_context.NewProduct?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
