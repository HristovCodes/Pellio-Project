using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pellio.Data;
using Pellio.Models;
using Pellio.ViewModels;

namespace Pellio.Controllers
{
    public class ProductsController : Controller
    {
        private readonly PellioContext _context;

        public ProductsController(PellioContext context)
        {
            _context = context;
        }

        // GET: Products
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string production)
        {
            
            if (Request.Cookies["uuidc"] == null)
            {
                Generateuuidc();//on boot we check for uuidc if not found call function to generate one
            }

            if(production == null || production == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(x => x.PlaceOfProduction == production).ToListAsync());
            }
            
        }

        public async void Generateuuidc()
        {
                var uuid = Guid.NewGuid().ToString();
                CookieOptions cookieOptionss = new CookieOptions();
                cookieOptionss.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("uuidc", uuid, cookieOptionss);

                //after we generate the uuid we also add a cart to the db
                if (await _context.OrdersList
                        .Include(c => c.Products).FirstOrDefaultAsync(m => m.UserId == uuid) == null)
                {
                    _context.Add(new OrdersList
                    {
                        Products = new List<Products>(),
                        Total = 0,
                        UserId = uuid
                    });
                    await _context.SaveChangesAsync();
                }
        }

        //GET: Products/CheckAll
        public async Task<IActionResult> CheckAll()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Order/5
        public async Task<IActionResult> Order(int? id)
        {
            ViewBag.Title = "Order";
            ViewBag.Header = "Поръчайте";
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            if (products.Comments == null)
            {
                products.Comments = new List<Comments>();
            }
            foreach (var com in _context.Comments)
            {
                if (!products.Comments.Contains(com))
                {
                    if (com.ProductsId == products.Id)
                    {
                        products.Comments.Add(com);
                    }
                }
            }

            ProductComment productComment = new ProductComment()
            {
                Products = products,
                Comments = new Comments()
            };

            return View(productComment);
        }

        //POST: Products/AddComment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, [Bind("Name,Comment")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                int productid = int.Parse(HttpContext.Request.Path.ToString().Substring(21));
                comments.ProductsId = productid;
                _context.Add(comments);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(comments);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductName,Ingredients,Price,ImageUrl")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Ingredients,Price,ImageUrl")] Products products)
        {
            if (id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
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
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //POST : Products/DeleteAll
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAll()
        {
            foreach (Products item in _context.Products)
            {
                _context.Products.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //POST : Products/FillDB
        [HttpPost, ActionName("FillDB")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FillDB()
        {
            string line;

            // Read the file and display it line by line.  
            string file_name = "Danni.txt";
            string path = Path.Combine(Environment.CurrentDirectory, file_name);
            System.IO.StreamReader file =
                new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split('|');
                Products tobeadded = new Products();
                tobeadded.ProductName = values[0];
                tobeadded.Ingredients = values[1].Trim('\"');
                tobeadded.Price = Convert.ToDecimal(values[2].Trim('"'), new CultureInfo("en-US"));
                tobeadded.ImageUrl = values[3];
                _context.Products.Add(tobeadded);
                //_context.Products.Add(new Products
                //{
                //    ProductName = values[0],
                //    Ingredients = values[1].Trim('\"'),
                //    Price = Convert.ToDecimal(values[2].Trim('\"')), //Convert.ToDecimal(values[2].Trim('\"'))
                //    ImageUrl = values[3]
                //}); ;
            }
            file.Close();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
