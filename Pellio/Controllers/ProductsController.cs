﻿namespace Pellio.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Pellio.Data;
    using Pellio.Models;
    using Pellio.ViewModels;

    /// <summary>
    /// This class controls everything that has to do with products like displaying them and commenting on them.
    /// </summary>
    public class ProductsController : Controller
    {
        /// <summary>
        /// Reference to database.
        /// </summary>
        private readonly PellioContext _context;

        /// <summary>
        /// Please add documentation. Thank you.
        /// </summary>
        private readonly IWebHostEnvironment env;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController" /> class.
        /// </summary>
        /// <param name="context">Reference to database.</param>
        public ProductsController(PellioContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fills buttons' text with categories from database.
        /// </summary>
        public void FillDropDownTags()
        {
            var tags = _context.Products.Select(t => t.Tag).Distinct();
            ViewBag.TagsforDropdown = tags;
        }

        /// POST: Products/AddComment/5
        /// <summary>
        /// Adds a comment to the database that is associated with a certain product.
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
        /// <param name="comments">Comment to be added to database.</param>
        /// <returns>Redirects to same page after adding a comment.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, [Bind("Name,Comment,Score")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                comments.ProductsId = id;
                _context.Add(comments);
                await _context.SaveChangesAsync();

                return RedirectToAction("Order", new { id = id });
            }

            return View(comments);
        }

        /// GET: Products/Order/5
        /// <summary>
        /// Returns a view of the chosen products (by id) with additional information about it and comments.
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
        /// <returns>View with both Product data and comments for Product.</returns>
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

            // check if any comments connected to product
            if (!products.Comments.Any())
            { // if not tell user there is no score
                ViewBag.avg_score = "За съжаление този продукт все още няма потребителски оценки. Можете да помогнете да промените това!";
            }
            else
            {
                // else avarage score and add to viewbag
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

        /// POST : Products/FillDB
        /// <summary>
        /// Reads a txt with testing data. For testing pursposes on multiple devices.
        /// </summary>
        /// <returns>Redirects to index page.</returns>
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
            todd.Usable = false;
            _context.Add(todd);
            _context.SaveChanges();
            var code = new PercentOffCode();
            code.Code = "bruh";
            code.Percentage = 50m;
            code.Usable = true;
            _context.Add(code);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// GET: Products
        /// <summary>
        /// Acts as a Main function. Makes call to uuidc create function.
        /// </summary>
        /// <param name="categories">Categories for text in buttons for sorting.</param>
        /// <returns>Displays all products from db.</returns>
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string TagsDropdown)
        {
            FillDropDownTags();
            
            string uid = Request.Cookies["uuidc"];

            if (_context.OrdersList
                    .Include(c => c.Products).FirstOrDefault(m => m.UserId == uid) == null)
            {
                _context.OrdersList.Add(new OrdersList
                {
                    Products = new List<Products>(),
                    Total = 0,
                    UserId = uid,
                    PercentOffCode = new PercentOffCode()
                    {
                        Code = "todd",
                        Percentage = 0,
                        Usable = false
                    }
                });
                _context.SaveChanges();
            }

            if (TagsDropdown == null || TagsDropdown == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(p => p.Tag == TagsDropdown).ToListAsync());
            }
        }
    }
}
