using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tsql3s2a.Data;
using Tsql3s2a.Models;
using Tsql3s2a.Models.ViewModels;

namespace Tsql3s2a.Controllers
{
    public class ProductsController : Controller
    {
        private TsqlContext _context;

        public ProductsController(TsqlContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 10)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Productname.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();

            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var model = new ProductListViewModel
            {
                Products = products,
                SearchTerm = searchTerm,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems/(double)pageSize)
            };

            return View(model);

        }


        public async Task<IActionResult> Create()
        {
            var model = new ProuctViewModel
            {
                Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Categoryid.ToString(), Text = c.Categoryname }).ToListAsync(),
                Suppliers = await _context.Suppliers.Select(c => new SelectListItem { Value = c.Supplierid.ToString(), Text = c.Companyname }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProuctViewModel model)
        {
            if (ModelState.IsValid)
            {
                var p = new Product {
                    Productname=model.Productname,
                    Supplierid=model.Supplierid,
                    Categoryid=model.Categoryid,
                    Unitprice=model.Unitprice,
                    Discontinued = model.Discontinued
                };

                _context.Products.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            model.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Categoryid.ToString(), Text = c.Categoryname }).ToListAsync();
            model.Suppliers = await _context.Suppliers.Select(c => new SelectListItem { Value = c.Supplierid.ToString(), Text = c.Companyname }).ToListAsync();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return NotFound();

            var model = new ProuctViewModel
            {
                Productid = p.Productid,
                Productname = p.Productname,
                Supplierid = p.Supplierid,
                Categoryid = p.Categoryid,
                Unitprice = p.Unitprice,
                Discontinued = p.Discontinued,

                Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Categoryid.ToString(), Text = c.Categoryname }).ToListAsync(),
                Suppliers = await _context.Suppliers.Select(c => new SelectListItem { Value = c.Supplierid.ToString(), Text = c.Companyname }).ToListAsync()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ProuctViewModel model)
        {
            if (id != model.Productid) return NotFound();

            if (ModelState.IsValid)
            {
                var p = await _context.Products.FindAsync(id);

                if (p == null) return NotFound();

                p.Productname = model.Productname;
                p.Supplierid = model.Supplierid;
                p.Categoryid = model.Categoryid;
                p.Unitprice = model.Unitprice;
                p.Discontinued = model.Discontinued;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            model.Categories = await _context.Categories.Select(c => new SelectListItem { Value = c.Categoryid.ToString(), Text = c.Categoryname }).ToListAsync();
            model.Suppliers = await _context.Suppliers.Select(c => new SelectListItem { Value = c.Supplierid.ToString(), Text = c.Companyname }).ToListAsync();
            return View(model);
        }




    }
}
