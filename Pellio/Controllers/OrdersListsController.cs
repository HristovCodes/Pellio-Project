namespace Pellio.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using OpenCage.Geocode;
    using Pellio.Data;
    using Pellio.Models;
    using Pellio.ViewModels;
    using ServiceStack.Text;

    /// <summary>
    /// This class controls everything that has to do with ordering.
    /// </summary>
    public class OrdersListsController : Controller
    {
        /// <summary>
        /// Reference to database.
        /// </summary>
        private readonly PellioContext _context;//db conn

        private readonly AppSettings _appSettings;//private conn to appsetttings file

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersListsController" /> class.
        /// </summary>
        /// <param name="context">Reference to database.</param>
        public OrdersListsController(PellioContext context, IOptions<AppSettings> appsettingsOptions)
        {
            _context = context;
            _appSettings = appsettingsOptions.Value;
            //gets vals from appsettings and adds them to _appsettings
        }

        /// <summary>
        /// Generated a random 8 digit discount code.
        /// </summary>
        /// <returns>Randomly generated discount code.</returns>
        public string CodeGenerate()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = (chars[random.Next(chars.Length)]);
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        // GET: OrdersLists

        /// <summary>
        /// This is the page for cart.
        /// </summary>
        /// <returns>Returns a view for cart.</returns>
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
                        Usable = false
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

        /// <summary>
        /// Check if the discount code provided is valid.
        /// </summary>
        /// <param name="code">Discount code to be checked.</param>
        /// <returns>Redirects to cart.</returns>
        public IActionResult UseDiscountCode(string code)
        {
            string uid = Request.Cookies["uuidc"];
            var code_form_db = _context.PercentOffCodes.Include(c => c.OrdersList)
                .Where(c => c.Code == code).FirstOrDefault();
            if (code_form_db != null && code_form_db.Usable)
            {
                var ol = _context.OrdersList.Include(c => c.Products).Where(u => u.UserId == uid).FirstOrDefault();
                ol.PercentOffCode = code_form_db;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        /// POST: OrdersLists/AddToCart/5
        /// <summary>
        /// Adds product to cart no redirects.
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
        /// <returns>Redirects to index page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int? id)
        {
            ProductToCart(id);
            return RedirectToAction("Index", "Products");
        }

        /// POST: OrdersLists/GoToCart/5
        /// <summary>
        /// Adds product to cart and redirects to cart.
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
        /// <returns>Redirects to cart.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GoToCart(int? id)
        {
            ProductToCart(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Adds Product object to Orderlist entry in db with specific uuidc.
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
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
                        Usable = false
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
            userorders.Products.Add(newproduct);

            UpdateItemsCount();
            _context.SaveChanges();
        }

        // POST: OrdersLists/DeleteProduct/5

        /// <summary>
        /// Deletes an ordered product from the database. Accepts ID as an argument. 
        /// </summary>
        /// <param name="id">Id of Product queried from db.</param>
        /// <returns>Redirects to cart.</returns>
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
        /// <param name="name">Name of user for delivery information.</param>
        /// <param name="address">Adress of user for delivery information.</param>
        /// <param name="rec">Email of user for delivery information.</param>
        /// <param name="phone">Phone of user for delivery information.</param>
        /// <param name="mes">Message to be sent in the mail.</param>
        /// <param name="code">Discount code that user entered.</param>
        /// <returns>The cart view after ordering (empty cart).</returns>
        [HttpPost]
        public async Task<IActionResult> OrderingMainLogicFunc(string name, string address, string rec, int phone, string code)
        {
            UseDiscountCode(code);
            SendMail(rec);
            await AddOrderToDb(name, address, phone, rec);
            await ClearCart();
            UpdateItemsCount();
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Called from view (Order.chhtml). Using Opencagedata API finds address from IP location
        /// </summary>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        /// <returns>Rough address based on IP location</returns>
        [HttpPost]
        public async Task<IActionResult> GeoLock(string lat, string lon)
        {
            var gc = new Geocoder(_appSettings.Geocode_key);//mades ne instance of geocoder with API key
            var reserveresult = gc.ReverseGeocode(double.Parse(lat, CultureInfo.InvariantCulture), double.Parse(lon, CultureInfo.InvariantCulture), "bg", false);
            //gets address from latitude and longitude conveted to double
            if(reserveresult.Status.Code == 200)
            {
                TempData["re_addres"] = reserveresult.Results[0].Formatted;
            }
            else
            {
                TempData["re_addres"] = "";
                Console.WriteLine(reserveresult.Status.Code);
                Console.WriteLine(reserveresult.Status.Message);
                Console.WriteLine("Some problem with the library. Overused key? Look^");
            }
            
            //Extracts address from returned by API data and adds it to TempData
            //TempData lives for one jump
            //When the user clicks off the Cart their address will not be saved
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Creates MadeOrder objects. Feeds it data. Saves it to DB.
        /// </summary>
        /// <param name="name">Name of user for delivery information.</param>
        /// <param name="address">Adress of user for delivery information.</param>
        /// <param name="phone">Phone of user for delivery information.</param>
        /// <param name="rec">Email of user for delivery information.</param>
        /// <returns>Returns nothing.</returns>
        public async Task AddOrderToDb(string name, string address, int phone, string rec)
        {
            string uid = Request.Cookies["uuidc"];
            MadeOrder neworder = new MadeOrder
            {
                CustomerName = name,
                CustomerAddress = address,
                CustomerPhoneNumber = phone,
                CustomerEmail = rec,
                UserId = uid,
                TimeOfOrder = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                Complete = false,
                Canceled = false
            };
            var userorders = await _context.OrdersList
                .Include(m => m.Products)
                .Include(co => co.PercentOffCode)
                .FirstOrDefaultAsync(m => m.UserId == uid);
            StringBuilder temp_product_names = new StringBuilder(string.Empty);
            foreach (var nameb in userorders.Products)
            {
                temp_product_names.Append(nameb.ProductName + ',');
            }

            neworder.Products_names = temp_product_names.ToString();
            neworder.FinalPrice = userorders.Total - (userorders.Total * (userorders.PercentOffCode.Percentage / 100));
            _context.MadeOrder.Add(neworder);
            _context.SaveChanges();
        }


        /// <summary>
        /// Generates the messege used in the email
        /// </summary>
        /// <returns>Messege used in the email</returns>
        private string GenEmailMsg()
        {
            string msgformail = "";
            string uid = Request.Cookies["uuidc"];
            var userorders = _context.OrdersList
                .Include(m => m.Products)
                .Include(co => co.PercentOffCode)
                .First(m => m.UserId == uid);
            
            if(userorders.PercentOffCode != null && userorders.PercentOffCode.Usable == true)
            {
                msgformail += "\n Вие използвахте кода " + userorders.PercentOffCode.Code + "!";
                msgformail += "\n Крайната цената се обновява " + userorders.Total + "лв - " + userorders.PercentOffCode.Percentage + "% = ";
                decimal temp_final = userorders.Total - (userorders.Total * (userorders.PercentOffCode.Percentage / 100));
                msgformail += temp_final.ToString("F") + "лв.";
                msgformail += "\n Храните които поръчахте са:";
            }
            else
            {
                msgformail += "\n Общата цена на вашата поръчка е " + userorders.Total;
                msgformail += "\n Храните които поръчахте са:";
            }
            
            foreach(var item in userorders.Products)
            {
                msgformail += "\n - " + item.ProductName + $"({item.Price})";
            }
            return msgformail;
        }

        /// <summary>
        /// Sends the email via gmail Smtp server.
        /// </summary>
        /// <param name="rec">Short for reciver.</param>
        /// <param name="mes">Short for messege.</param>
        public void SendMail(string rec)
        {
            string uid = Request.Cookies["uuidc"];
            string mes = GenEmailMsg();
            var codebruh = _context.OrdersList.Include(c => c.PercentOffCode)
                .Where(a => a.UserId == uid).First().PercentOffCode.Code;
            if (ModelState.IsValid)
            {
                var completed = _context.MadeOrder.Where(c => c.UserId == uid).Count();
                if (completed % 3 == 0)
                {
                    var code = new PercentOffCode
                    {
                        Code = CodeGenerate(),
                        Percentage = 5,
                        Usable = true
                    };
                    _context.PercentOffCodes.Add(code);
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };
                    //mes = mes.TrimEnd(',');
                    //mes = mes.Replace("&", "\n");
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods може да получи намаление с код " + code.Code + $"({code.Percentage}%), направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
                else
                {
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };

                    mes = mes.TrimEnd(',');
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
            }
        }

        /// <summary>
        /// Clears all Product items from DB table OrderList with specific UUIDC.
        /// </summary>
        /// <returns>Returns nothing.</returns>
        public async Task ClearCart()
        {
            string uid = Request.Cookies["uuidc"];
            var ordersList = await _context.OrdersList
                .Include(c => c.Products)
                .Include(co => co.PercentOffCode)
                .FirstAsync(m => m.UserId == uid);
            var productList = ordersList.Products;
            _context.OrdersList.Remove(ordersList);
            var used_code = _context.PercentOffCodes.Where(n => n.Code == ordersList.PercentOffCode.Code).FirstOrDefault();
            _context.PercentOffCodes.Remove(used_code); // finds and removes code from db
            ordersList.PercentOffCode = new PercentOffCode()
            {
                Code = "todd",
                Percentage = 0,
                Usable = false
            };// replaces with empty code
            _context.Products.RemoveRange(productList);
            UpdateItemsCount();
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Counts items in the users cart and makes a cookie with the count for use inside js.
        /// </summary>
        public void UpdateItemsCount()
        {
            CookieOptions cookieOptionss = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30)
            };

            if (Request.Cookies["cartitems"] == null)
            {
                Response.Cookies.Append("cartitems", "0", cookieOptionss);
            }

            string uid = Request.Cookies["uuidc"];
            string count = "0";

            if (_context.OrdersList.Include(c => c.Products).FirstOrDefault(m => m.UserId == uid) != null)
            {
                count = _context.OrdersList.Include(c => c.Products)
                                              .FirstOrDefault(m => m.UserId == uid).Products.Count
                                              .ToString();
            }

            Response.Cookies.Delete("cartitems");
            Response.Cookies.Append("cartitems", count, cookieOptionss);
        }
    }
}
