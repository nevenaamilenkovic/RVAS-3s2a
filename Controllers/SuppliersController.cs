using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsql3s2a.Data;
using Tsql3s2a.Models;
namespace Tsql3s2a.Controllers
{
    public class SuppliersController : Controller
    {
        //instanciramo kontekst
        private readonly TsqlContext _context;

        //DI konstruktor
        public SuppliersController(TsqlContext context)
        {
            _context = context;
        }

        //lista suppliera (SELECT * FROM Production.Suppliers)
        public async Task<IActionResult> Index()
        {
            var s = await _context.Suppliers.ToListAsync();
            return View (s);
        }

        //vracamo formu korisniku
        public IActionResult Create()
        {
            return View();
        }

        //post metoda koja obradjuje podatke sa forme i smesta ih u bazu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Supplierid","Companyname","Contactname","Contacttitle","Address","City","Region","Postalcode","Country","Phone","Fax")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}
