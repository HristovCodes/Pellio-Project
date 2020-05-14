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
                    PercentOffCode = new PercentOffCode()
                    {
                        Code = "todd",
                        Percentage = 0,
                        Available = false
                    }
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
            }
            return RedirectToAction(nameof(Index));
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
                    PercentOffCode = new PercentOffCode()
                    {
                        Code = "todd",
                        Percentage = 0,
                        Available = false
                    }
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

            UpdateItemsCount();
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
            UpdateItemsCount();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Called from view (Order.chhtml). Calls subfunctions associated with ordering. 
        /// </summary>
        /// <returns>The cart view after ordering. (empty cart)</returns>
        [HttpPost]
        public async Task<IActionResult> OrderingMainLogicFunc(string name, string address, string rec, int phone, string mes, string code)
        {
            UseDiscountCode(code);
            SendMail(rec, mes);
            AddOrderToDb(name, address, phone, rec);
            ClearCart();
            //napravi nqkude check dali total e po malko ot 0, ako e NE pravi poruchka - Ivailo
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
            string uid = Request.Cookies["uuidc"];
            try
            {
                if (ModelState.IsValid)
                {
                    var completed = _context.MadeOrder.Where(c => c.UserId == uid).Count();
                    if (completed % 3 == 0)
                    {
                        var code = new PercentOffCode
                        {
                            Code = CodeGenerate(),
                            Percentage = 5,
                            Available = true
                        };
                        _context.PercentOffCodes.Add(code);
                        var credsfromdb = _context.EmailCredentials.FirstOrDefault();
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(credsfromdb.Email, credsfromdb.Password),
                            EnableSsl = true
                        };
                        mes = mes.TrimEnd(',');
                        client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods пможе да получи намаление с код " + code.Code + ", направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                    }
                    else
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
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string CodeGenerate()

        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
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
                .FirstAsync(m => m.UserId == uid);//finds cart
            var productList = ordersList.Products;
            _context.OrdersList.Remove(ordersList);
            var used_code = _context.PercentOffCodes.Where(n => n.Code == ordersList.PercentOffCode.Code).FirstOrDefault();
            _context.PercentOffCodes.Remove(used_code);//finds and removes code from db
            ordersList.PercentOffCode = _context.PercentOffCodes.FirstOrDefault();//replaces with empty code
            _context.Products.RemoveRange(productList);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Counts items in the users cart and makes a cookie with the count for use inside js
        /// </summary>
        private void UpdateItemsCount()
        {
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);

            if (Request.Cookies["cartitems"] == null)
            {
                Response.Cookies.Append("cartitems", "0", cookieOptionss);
            }

            string uid = Request.Cookies["uuidc"];

            var productsincart = _context.OrdersList.Include(c => c.Products).FirstOrDefault(m => m.UserId == uid);
            string count = productsincart.Products.Count().ToString();

            Response.Cookies.Delete("cartitems");
            Response.Cookies.Append("cartitems", count, cookieOptionss);
        }
    }
}
