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
            //OrderListCleanUp();

            string uid = Request.Cookies["uuidc"];

            var cart = await _context.OrdersList
                .Include(c => c.Products).Include(co => co.PercentOffCode).FirstOrDefaultAsync(m => m.UserId == uid);

            if (cart == null)
            {
                cart = new OrdersList
                {
                    Total = 0,
                    UserId = uid,
                    TimeMade = DateTime.Now.ToString("MM/dd/yyyy"),
                    Products = new List<Products>(),
                    PercentOffCode = _context.PercentOffCodes.FirstOrDefault()
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

        public async Task<IActionResult> UseDiscountCode(string code)
        {
            string uid = Request.Cookies["uuidc"];
            var code_form_db = _context.PercentOffCodes.Include(c => c.OrdersList)
                .Where(c => c.Code == code).FirstOrDefault();
            if (code_form_db != null && code_form_db.Available == true)
            {
                var ol = _context.OrdersList.Include(c => c.Products).Where(u => u.UserId == uid).FirstOrDefault();
                ol.PercentOffCode = code_form_db;
                _context.SaveChanges();
                //var oldos = _context.OrdersList.Include(c => c.Products).Where(u => u.UserId == uid).FirstOrDefault();
                TempData["used_code"] =
                    $"Поздравления! Вие използвахте кода {code_form_db.Code} които ви дава {String.Format("{0:0}", code_form_db.Percentage)}% отстъпка.";
            }
            else
            {
                TempData["used_code"] = "Съжелява ме. но този код не е истински или е вече използван";
            }
            //var ccode = new PercentOffCode();
            //ccode.Code = "bruh";
            //ccode.Percentage = 0.5m;
            //ccode.Available = true;
            //_context.Add(ccode);
            //_context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private void OrderListCleanUp()
        {
            var entries = _context.OrdersList.Include(c => c.Products).ToList();
            if (entries.Any())
            {
                string curr_time = DateTime.Now.ToString("MM/dd/yyyy");
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
        }

        //POST: OrdersLists/AddToCart/5
        /// <summary>
        /// Adds product to cart no redirects
        /// </summary>
        /// <param name="id">id of Product queried from db</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int? id)
        {
            ProductToCart(id);
            return RedirectToAction("Index", "Products");
        }

        //POST: OrdersLists/GoToCart/5
        /// <summary>
        /// Adds product to cart and redirects to cart
        /// </summary>
        /// <param name="id">id of Product queried from db</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoToCart(int? id)
        {
            ProductToCart(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Adds Product object to Orderlist entry in db with specific uuidc
        /// </summary>
        public void ProductToCart(int? id)
        {
            string uid = Request.Cookies["uuidc"];
            var userorders = _context.OrdersList
            .FirstOrDefault(m => m.UserId == uid);

            if (userorders == null)
            {
                userorders = new OrdersList
                {
                    Total = 0,
                    UserId = uid,
                    TimeMade = DateTime.Now.ToString("MM/dd/yyyy"),
                    Products = new List<Products>(),
                    PercentOffCode = _context.PercentOffCodes.FirstOrDefault()
                };
                _context.OrdersList.Add(userorders);
            }
            else if (userorders.Products == null)
            {
                userorders.Products = new List<Products>();
            }

            var pr = _context.Products.Find(id);

            var newproduct = new Products
            {
                ProductName = pr.ProductName,
                Price = pr.Price,
                ImageUrl = pr.ImageUrl,
                Ingredients = "dont show"
            };
            userorders.Total += pr.Price;
            //if(userorders.PercentOffCode != null && userorders.PercentOffCode.Available == true)
            //{
            //    userorders.Total = userorders.Total - (userorders.Total * (userorders.PercentOffCode.Percentage / 100));
            //}
            userorders.Products.Add(newproduct);

            _context.SaveChanges();
        }

        // POST: OrdersLists/DeleteProduct/5
        /// <summary>
        /// Deletes an ordered product from the database. Accepts ID as an argument. 
        /// </summary>
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            var removed = _context.Products
           .First(m => m.Id == id);

            string uid = Request.Cookies["uuidc"];
            var userorders = await _context.OrdersList
            .FirstOrDefaultAsync(m => m.UserId == uid);

            userorders.Total -= removed.Price;

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
                    .Include(co => co.PercentOffCode)
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
                neworder.FinalPrice = userorders.Total - (userorders.Total * (userorders.PercentOffCode.Percentage / 100));
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
                .Include(co => co.PercentOffCode)
                .FirstAsync(m => m.UserId == uid);
            var productList = ordersList.Products;
            _context.OrdersList.Remove(ordersList);
            var used_code = _context.PercentOffCodes.Where(n => n.Code == ordersList.PercentOffCode.Code).FirstOrDefault().Available = false;
            //used_code.Available = false;
            ordersList.PercentOffCode = _context.PercentOffCodes.FirstOrDefault();
            _context.Products.RemoveRange(productList);
            await _context.SaveChangesAsync();
        }
    }
}
