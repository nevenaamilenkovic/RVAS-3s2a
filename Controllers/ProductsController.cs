using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Numerics;
using System;
using System.Runtime.Intrinsics.Arm;
using System.Xml.Linq;
using Tsql3s2a.Data;
using Tsql3s2a.Models;
using Tsql3s2a.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //nismo stigli na terminu 3
        //action name je opciona stvar sad, ja ga dodajem iz navike
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //metoda prima id proizvoda koji se brise
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            //pre nego pocnemo sa brisanjem potrebno je da simuliramo sql join tabele orderDetails i orders
            var p = await _context.Products
                .Include(p=>p.OrderDetails)
                .ThenInclude(od=>od.Order)
                .FirstOrDefaultAsync(p=>p.Productid == id);

            //ukoliko proizvod sa tim id-jem ne postoji vracamo gresku
            if (p == null) return NotFound();

            //Dalje proveravamo da li proizvod ima povezane narudzbine
            //p.OrderDetails je lista objekata koja predstavlja sve stavke narudzbine povezane sa nekim proizvodom
            //Any() je Linq metoda koja proverava da li kolekcija OrderDetails sadrzi bilo kakve elemente, odnosno da li je povezana sa proizvodom koji zelimo da brisemo, ako jeste uslov je true i vracaju se testavke narudzbine
            if (p.OrderDetails.Any())
            {
                //Ukoliko ima povezanih narudzbina, dalje moramo da koristimo ViewData objekat da bismo prosledili podatke iz kontrolera u View
                //Stavljamo proizvod p odnosno instancu klase Product u ViewData tako da ga mozemo koristiti u Viewu
                ViewData["Product"] = p;
                //Ovde isto radimo sto i sa proizvodom, samo sto prosledjujemo samo detalje o orderu na osnovu svih OrderDetailsa
                ViewData["Orders"] = p.OrderDetails.Select(od => od.Order).ToList();
                //Ukoliko je proizvod povezan sa nekim narudzbimana vracamo view pod nazivom “DeleteRestricted”
                return View("DeleteRestricted");
            }


            //Naravo ako proizvod nema povezane narudzbine mozemo slobodno da ga obrisemo iz tabele i osvezimo stranicu sa listom proizvoda
            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Da bi mogli odmah da brisemo proizvode sa indeksa moramo da dodamo opciju za Delete u Products/index
        //Pravimo formu koja salje post zahtev kontroleru za brisanje proizvoda u viewu deleteRestricted


    }
}
