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
            await _context.SaveChangesAsync();

            FillDropDownTags();
            //var pro = _context.Products.Include(c => c.ListOfIngredients).FirstOrDefault();
            //var ing = await _context.Ingredients.FirstOrDefaultAsync();
            //Ingredient ing1 = new Ingredient();
            //ing1.Name = "царевица";
            //ing1.Available = true;
            //var newpro = new Products();
            //newpro.ProductName = "bruh";
            //newpro.Price = 5;
            //newpro.ImageUrl = "https://i.imgur.com/uVue8N5.jpg";
            //newpro.ListOfIngredients = new List<Ingredient>
            //{
            //    ing1
            //};
            ////_context.Ingredients.Add(ing1);
            //////pro.ListOfIngredients.Add(ing);
            //_context.Products.Add(newpro);
            //await _context.SaveChangesAsync();

            //test code delete before realese^ !!!!!
            GenUUID();
            if(TagsDropdown == null)
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
        private void GenUUID()
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
            IEnumerable<SelectListItem> tags = _context.Products.Select(t => new SelectListItem
            {
                Value = t.Tag,
                Text = t.Tag
            }).Distinct();
            ViewBag.TagsforDropdown = tags;

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

            if (_context.Comments.Where(m => m.ProductsId == id).Any())//check if there any records in comments table
            {
                //if avarage score and add to viewbag
                var avg_score = _context.Comments
                .Where(m => m.ProductsId == id)
                .Average(m => m.Score);
                ViewBag.avg_score = "Нашите потребители средно дават на това ястие оценката: " + avg_score;
            }
            else
            {//if not tell user there is no score
                ViewBag.avg_score = "За съжаление този продукт все още няма потребителски оценки. Можете да помогнете да промените това!";
            }

            var products = await _context.Products.Include(c => c.ListOfIngredients)
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
