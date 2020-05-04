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
        /// <summary>
        /// Acts as a Main function. Makes call to uuidc create function.
        /// </summary>
        /// <returns>Displays all products from db.</returns>
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string TagsDropdown)
        {
            var creds = new EmailCredentials();
            creds.Email = "fokenlasersights@gmail.com";
            creds.Password = "***REMOVED***";
            _context.Add(creds);
            //var ccode = new PercentOffCode();
            //ccode.Code = "WORK";
            //ccode.Percentage = 0.5m;
            //ccode.Available = true;
            //_context.Add(ccode);
            await _context.SaveChangesAsync();
            //debug^

            FillDropDownTags();
            GenUUIDC();
            if (TagsDropdown == null || TagsDropdown == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(p => p.Tag == TagsDropdown).ToListAsync());
            }

        }

        /// <summary>
        /// Generates UUID. Generates new cart entry in db with said UUID.
        /// </summary>
        private void GenUUIDC()
        {
            if (Request.Cookies["uuidc"] == null)
            {
                var uuid = Guid.NewGuid().ToString();
                CookieOptions cookieOptionss = new CookieOptions();
                cookieOptionss.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("uuidc", uuid, cookieOptionss);

                if (_context.OrdersList
                    .Include(c => c.Products).FirstOrDefault(m => m.UserId == uuid) == null)
                {
                    _context.Add(new OrdersList
                    {
                        Products = new List<Products>(),
                        Total = 0,
                        UserId = uuid
                    });
                    _context.SaveChanges();
                }

            }
        }

        public void FillDropDownTags()
        {
            var tags = _context.Products.Select(t => t.Tag).Distinct();
            ViewBag.TagsforDropdown = tags;
        }

        //GET: Products/CheckAll
        //Returns a view with all the products the in database.
        public async Task<IActionResult> CheckAll()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Order/5
        /// <summary>
        /// Returns a view of the chosen products (by id) with additional information about it and comments.
        /// </summary>
        /// <returns>View with both Product data and comments for Product</returns>
        public async Task<IActionResult> Order(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.Include(co => co.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            if (products.Comments == null)//check if it is null
            {
                products.Comments = new List<Comments>();
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
            }

            if (!products.Comments.Any())//check if any comments connected to product
            {//if not tell user there is no score
                ViewBag.avg_score = "За съжаление този продукт все още няма потребителски оценки. Можете да помогнете да промените това!";
            }
            else
            {
                //if avarage score and add to viewbag
                var avg_score = products.Comments.Average(sc => sc.Score);
                var rounded = Math.Round(avg_score, 2);
                ViewBag.avg_score = "Нашите потребители средно дават на това ястие оценката: " + rounded;
            }

            ProductComment productComment = new ProductComment()
            {
                Products = products,
                Comments = new Comments()
            };

            return View(productComment);
        }

        //POST: Products/AddComment/5
        //Adds a comment to the database that is associated with a certain product.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, [Bind("Name,Comment,Score")] Comments comments)
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

        //POST : Products/FillDB
        //Reads a txt with testing data. For testing pursposes on multiple devices.
        [HttpPost, ActionName("FillDB")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FillDB()
        {
            string line;
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
                tobeadded.Tag = values[4];
                _context.Products.Add(tobeadded);
            }
            file.Close();
            var todd = new PercentOffCode();
            todd.Code = "todd";
            todd.Percentage = 0m;
            todd.Available = false;
            _context.Add(todd);
            _context.SaveChanges();
            var code = new PercentOffCode();
            code.Code = "bruh";
            code.Percentage = 50m;
            code.Available = true;
            _context.Add(code);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Checks if a product exists. Accepts ID as an argument.
        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
