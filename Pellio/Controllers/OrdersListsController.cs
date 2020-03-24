using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pellio.Data;
using Pellio.Models;

namespace Pellio.Controllers
{
    public class OrdersListsController : Controller
    {
        private readonly PellioContext _context;

        public OrdersListsController(PellioContext context)
        {
            _context = context;
        }

        // GET: OrdersLists
        public async Task<IActionResult> Index()
        {
            string uid = Request.Cookies["uuidc"];
            return View(await _context.OrdersList
                .Include(c => c.Products).FirstOrDefaultAsync(m => m.UserId == uid));
        }
        //POST: OrdersLists/AddToCart/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int? id)
        {
            string uid = Request.Cookies["uuidc"];
            var userorders = await _context.OrdersList
            .FirstOrDefaultAsync(m => m.UserId == uid);
            if (userorders == null)
            {
                userorders = new OrdersList
                {
                    Total = 0,
                    UserId = uid
                };
                _context.OrdersList.Add(userorders);
            }
            if (userorders.Products == null)
            {
                userorders.Products = new List<Products>();
            }
            var product = _context.Products.Find(id);
            var newproduct = new Products
            {
                ProductName = product.ProductName,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Ingredients = "dont show"
            };

            userorders.Products.Add(newproduct);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //pm;sdfmk;lds
        public async Task<IActionResult> DeleteProduct(int? id)
        {
             var removed = _context.Products
            .First(m => m.Id == id);
            _context.Products
           .Remove(removed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //sendmail
        [HttpPost]
        public async Task<IActionResult> SendMail(string rec, string mes)
        {
            var credsfromdb = _context.EmailCredentials.Find(1);
            try
            {
                if (ModelState.IsValid)
                {
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {

                        Credentials = new NetworkCredential(credsfromdb.Email, credsfromdb.Password),
                        EnableSsl = true
                    };
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes);
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: OrdersLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersList = await _context.OrdersList
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersList == null)
            {
                return NotFound();
            }

            return View(ordersList);
        }

        // GET: OrdersLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrdersLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Total,UserId")] OrdersList ordersList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordersList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ordersList);
        }

        // GET: OrdersLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersList = await _context.OrdersList.FindAsync(id);
            if (ordersList == null)
            {
                return NotFound();
            }
            return View(ordersList);
        }

        // POST: OrdersLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Total,UserId")] OrdersList ordersList)
        {
            if (id != ordersList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordersList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersListExists(ordersList.Id))
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
            return View(ordersList);
        }

        // GET: OrdersLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersList = await _context.OrdersList
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersList == null)
            {
                return NotFound();
            }

            return View(ordersList);
        }

        // POST: OrdersLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var ordersList = await _context.OrdersList
                .Include(c => c.Products)
                .FirstAsync(m => m.Id == id);
            var productList = ordersList.Products;
            _context.OrdersList.Remove(ordersList);
            _context.Products.RemoveRange(productList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersListExists(int id)
        {
            return _context.OrdersList.Any(e => e.Id == id);
        }
    }
}
