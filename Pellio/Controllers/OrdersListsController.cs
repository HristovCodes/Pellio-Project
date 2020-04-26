using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Pellio.ViewModels;

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
            OrderListCleanUp();

            string uid = Request.Cookies["uuidc"];

            var cart = await _context.OrdersList
                .Include(c => c.Products).FirstOrDefaultAsync(m => m.UserId == uid);

            if (cart == null)
            {
                cart = new OrdersList
                {
                    Total = 0,
                    UserId = uid,
                    TimeMade = DateTime.Now.ToString("MM/dd/yyyy"),
                    Products = new List<Products>()
                };
                _context.Add(cart);
                await _context.SaveChangesAsync();
            }


            OrderListMadeOrder combo = new OrderListMadeOrder
            {
                OrdersList = cart,
                MadeOrder = _context.MadeOrder.Where(mo => mo.UserId == uid).ToList()
            };

            return View(combo);
        }

        private void OrderListCleanUp()
        {
            string curr_time = DateTime.Now.ToString("MM/dd/yyyy");
            var entries = _context.OrdersList.Include(c => c.Products).ToList();
            foreach (var entry in entries)
            {
                DateTime parsed_now = DateTime.ParseExact(curr_time, "MM/dd/yyyy", null);
                DateTime parsed_entry = DateTime.ParseExact(entry.TimeMade, "MM/dd/yyyy", null);
                if ((parsed_entry.Date - parsed_now.Date).Days >= 31)
                {
                    _context.OrdersList.Remove(entry);
                }
            }
            _context.SaveChanges();
        }

        //POST: OrdersLists/AddToCart/5
        /// <summary>
        /// Adds Product object to Orderlist entry in db with specific uuidc
        /// </summary>
        /// <param name="id">id of Product queried from db</param>
        /// <returns></returns>
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
                    UserId = uid,
                    TimeMade = DateTime.Now.ToString("MM/dd/yyyy"),
                    Products = new List<Products>()
                };
                _context.OrdersList.Add(userorders);
            }
            else if (userorders.Products == null)
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
            userorders.Total += product.Price;
            userorders.Products.Add(newproduct);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: OrdersLists/DeleteProduct/5
        /// <summary>
        /// Deletes an ordered product from the database. Accepts ID as an argument. 
        /// </summary>
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            var removed = _context.Products
           .First(m => m.Id == id);
            _context.Products
           .Remove(removed);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Called from view (Order.chhtml). Calls subfunctions associated with ordering. 
        /// </summary>
        /// <returns>The cart view after ordering. (empty cart)</returns>
        [HttpPost]
        public async Task<IActionResult> OrderingMainLogicFunc(string name, string address, string rec, int phone, string mes)
        {
            await SendMail(rec, mes);
            await AddOrderToDb(name, address, phone, rec);
            await ClearCart();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Creates MadeOrder objects. feeds it data. Saves it to DB.
        /// </summary>
        async public Task AddOrderToDb(string name, string address, int phone, string rec)
        {
            try
            {
                string uid = Request.Cookies["uuidc"];
                MadeOrder neworder = new MadeOrder();
                neworder.CustomerName = name;
                neworder.CustomerAddress = address;
                neworder.CustomerPhoneNumber = phone;
                neworder.CustomerEmail = rec;
                neworder.UserId = uid;
                neworder.TimeOfOrder = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                neworder.Complete = false;
                neworder.Canceled = false;
                var userorders = await _context.OrdersList
                    .Include(m => m.Products)
                    .FirstOrDefaultAsync(m => m.UserId == uid);
                string temp_product_names = "";
                foreach (var nameb in userorders.Products)
                {
                    temp_product_names += nameb.ProductName;
                    temp_product_names += ',';
                }
                neworder.Products_names = temp_product_names;
                //supposted to be a many-to-many????
                //"works" fix^
                neworder.FinalPrice = userorders.Total;
                _context.MadeOrder.Add(neworder);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Sends the email via gmail smtp server
        /// </summary>
        /// <param name="rec">short for reciver</param>
        /// <param name="mes">short for messege</param>
        async public Task SendMail(string rec, string mes)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var credsfromdb = _context.EmailCredentials.FirstOrDefault();
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {

                        Credentials = new NetworkCredential(credsfromdb.Email, credsfromdb.Password),
                        EnableSsl = true
                    };
                    mes = mes.TrimEnd(',');
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Clears all Product items from DB table OrderList with specific UUIDC
        /// </summary>
        private async Task ClearCart()
        {
            string uid = Request.Cookies["uuidc"];
            var ordersList = await _context.OrdersList
                .Include(c => c.Products)
                .FirstAsync(m => m.UserId == uid);
            var productList = ordersList.Products;
            _context.OrdersList.Remove(ordersList);
            _context.Products.RemoveRange(productList);
            await _context.SaveChangesAsync();
        }
    }
}
