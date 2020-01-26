using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _5Semester.Models;
using Microsoft.AspNetCore.Http;

namespace _5Semester.Controllers
{
    public class ProductsController : Controller
    {
        private readonly _5SemesterContext _context;

        public ProductsController(_5SemesterContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                return View(await _context.Product.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin") {

                return View();
            }
            return RedirectToAction("Index", "Home");
    }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescription")] Product product)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
                return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescription")] Product product)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string status = HttpContext.Session.GetString("sessionStatus");
            if (status == "Admin")
            {
                var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "Home");
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
