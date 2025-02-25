using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsql3s2a.Data;
using Tsql3s2a.Models;
using Tsql3s2a.Models.ViewModels;

namespace Tsql3s2a.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly TsqlContext _context;

        public SuppliersController(TsqlContext context)
        {
            _context = context;
        }

        //LISTA SUPPLIERA
        public async Task<IActionResult> Index()
        {
            var s = await _context.Suppliers.ToListAsync();
            return View(s);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Supplierid", "Companyname", "Contactname", "Contacttitle", "Address", "City", "Region", "Postalcode", "Country", "Phone", "Fax")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var s = await _context.Suppliers.FirstOrDefaultAsync(s => s.Supplierid == id);
            return View(s);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Supplierid", "Companyname", "Contactname", "Contacttitle", "Address", "City", "Region", "Postalcode", "Country", "Phone", "Fax")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var s = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Supplierid == id);

            if (s == null)
            {
                return NotFound();
            }

            var model = new SupplierViewModel
            {
                Supplier = s,
                Products = s.Products.ToList()
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var s = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Supplierid == id);

            if (s != null)
            {
                _context.Suppliers.Remove(s);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}
