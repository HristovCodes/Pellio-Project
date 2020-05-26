using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pellio.Controllers;
using Pellio.Data;
using System.Threading.Tasks;
using System.Linq;
using Moq;
using Pellio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pellio.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Web;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Collections.Specialized;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTest
{

    [TestClass]
    public class UnitTest1
    {
        ServiceProvider _provider;

        public void Prep_Appsettings()
        {
            var services = new ServiceCollection();
            // mock PersonSettings
            services.AddTransient<IOptions<AppSettings>>(
                 provider => Options.Create<AppSettings>(new AppSettings
                 {
                     Email_name = "name",
                     Email_pass = "pass",
                     Geocode_key = "key"
                 }));
            _provider = services.BuildServiceProvider();
        }

        #region UnitTest ProductsController_tests
        //below are ProductsController test

        [TestMethod]
        public void ProductsControllerIndexReturnsView()
        {
            // Arrange
            ProductsController controller = new ProductsController(null);

            // Act
            Task<IActionResult> result = controller.Index(null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DataForSelectingIsGivenToView()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var context = new PellioContext(options);
            Products pr = new Products();
            pr.Tag = "pizza";
            context.Products.Add(pr);
            context.SaveChanges();
            ProductsController pcr = new ProductsController(context);

            // Act
            var before = pcr.ViewBag.TagsforDropdown;
            pcr.FillDropDownTags();
            var after = pcr.ViewBag.TagsforDropdown;

            //Assert
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public void OrderReturnsView()
        {
            // Arrange
            ProductsController controller = new ProductsController(null);

            // Act
            Task<IActionResult> result = controller.Order(null);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task OrderReturnsNotFoundIfGivenNull()
        {
            // Arrange
            ProductsController controller = new ProductsController(null);

            // Act
            IActionResult action = await controller.Order(null);
            var StatusCodeResult = (IStatusCodeActionResult)action; 

            // Assert
            Assert.IsNotNull(action);
            Assert.AreEqual(404, StatusCodeResult.StatusCode);
        }

        [TestMethod]
        public async Task OrderReturnsNotFoundIfGivenUnrealId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            context.Products.Add(pr);
            context.SaveChanges();
            //setting up mock db^
            ProductsController pcr = new ProductsController(context);

            // Act
            IActionResult action = await pcr.Order(2);//id 2 does not exist
            var StatusCodeResult = (IStatusCodeActionResult)action;//cast to status code

            // Assert
            Assert.IsNotNull(action);
            Assert.AreEqual(404, StatusCodeResult.StatusCode);
        }

        [TestMethod]
        public async Task CheckIfAvarageMesgIsCorrect()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            context.Products.Add(pr);
            Comments co = new Comments();
            co.Comment = "bruh";
            co.Products = pr;
            context.Comments.Add(co);
            context.SaveChanges();
            //setting up mock db^
            ProductsController pcr = new ProductsController(context);

            //Act
            var before = pcr.ViewBag.avg_score; ;
            await pcr.Order(1);//id 1 is pizza
            var after = pcr.ViewBag.avg_score;

            // Assert
            Assert.IsNotNull(pcr);
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public async Task CommentGetsAddedToProductAndDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            context.Products.Add(pr);
            context.SaveChanges();
            Comments co = new Comments();
            co.Comment = "bruh";
            co.Products = pr;
            co.Score = 1;
            //setting up mock db^
            ProductsController pcr = new ProductsController(context);

            //Act
            var before = context.Products.Include(x => x.Comments).First().Comments.Count;
            await pcr.AddComment(1, co);
            var after = context.Products.Include(x => x.Comments).First().Comments.Count;

            //Assert
            Assert.AreNotEqual(before, after);
            Assert.AreEqual(0, before);
            Assert.AreEqual(1, after);
        }

        //here end the ProductsController test
        #endregion

        #region UnitTest OrderListController_tests
        //below OrderListController test

        [TestMethod]
        public void OrderListControllerIndexReturnsView()
        {
            // Arrange
            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(null, app_settings_options);

            // Act
            Task<IActionResult> result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        private void PercentOffCodeTextIsGenerated()
        {
            // Arrange
            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(null, app_settings_options);

            // Act
            var result = "";
            var before = result;
            result = controller.CodeGenerate();
            var after = result;

            // Assert
            Assert.IsNotNull(controller);
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public async Task DataFromDbIsPassedToViewCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act
            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;
            var model = (OrderListMadeOrder)(viewResult.Model);

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(1, model.OrdersList.Products.Count);
            Assert.AreEqual("pizza", model.OrdersList.Products.First().ProductName);
        }

        [TestMethod]
        public async Task ProductsInCartCountIsUpdatedCorrectly()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act

            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;
            var model = (OrderListMadeOrder)(viewResult.Model);
            string count = _context.OrdersList.Include(c => c.Products)
                                              .First()
                                              .Products.Count
                                              .ToString();
            controller.Request.Headers["cartitems"] = count;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(count, model.OrdersList.Products.Count.ToString());
            Assert.AreEqual(controller.Request.Headers["cartitems"].ToString(), model.OrdersList.Products.Count.ToString());
        }

        [TestMethod]
        public async Task UsingCodeFuncActuallyUsingCodeAndAddingDbConnection()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            PercentOffCode poc = new PercentOffCode();
            poc.Code = "potato";//something random to compare to
            poc.Usable = true;
            poc.Percentage = 50;//needed here?
            _context.PercentOffCodes.Add(poc);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act

            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var before = _context.OrdersList.Include(c => c.PercentOffCode)
                                                    .First()
                                                    .PercentOffCode.Code
                                                    .ToString();
            controller.UseDiscountCode("potato");
            var after = _context.OrdersList.Include(c => c.PercentOffCode)
                 .First()
                 .PercentOffCode.Code
                 .ToString();
            var viewResult = actionResultTask.Result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public async Task ProductIsAddedToCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act

            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            int before = _context.OrdersList.Include(c => c.Products)
                                              .First()
                                              .Products.Count;
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;
            int after = _context.OrdersList.Include(c => c.Products)
                                              .First()
                                              .Products.Count;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public async Task ProductIsDeletedFromCart()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act

            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            int before = _context.OrdersList.Include(c => c.Products)
                                              .First()
                                              .Products.Count;
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.DeleteProduct(2);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;
            int after = _context.OrdersList.Include(c => c.Products)
                                              .First()
                                              .Products.Count;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(before, after);
        }

        [TestMethod]
        public async Task OrderIsCompletedAndAddedToDb()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act
            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            int before = _context.MadeOrder.Count();
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.AddOrderToDb("name", "adres", 123, "email");
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            var viewResult = actionResultTask.Result as ViewResult;
            int after = _context.MadeOrder.Count();

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreNotEqual(before, after);
        }

        [TestMethod]
        public async Task CartIsCorrectlyCleared()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PellioContext>()
                .UseInMemoryDatabase(databaseName: "PellioDb")
                .Options;
            var _context = new PellioContext(options);
            Products pr = new Products();
            pr.ProductName = "pizza";
            _context.Products.Add(pr);
            PercentOffCode poc = new PercentOffCode();
            poc.Code = "potatos";
            poc.Percentage = 50;
            poc.Usable = true;
            _context.SaveChanges();
            //setting up mock db^

            Prep_Appsettings();
            IOptions<AppSettings> app_settings_options = _provider.GetService<IOptions<AppSettings>>();
            OrdersListsController controller = new OrdersListsController(_context, app_settings_options);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var uuid = Guid.NewGuid().ToString();
            CookieOptions cookieOptionss = new CookieOptions();
            cookieOptionss.Expires = DateTime.Now.AddDays(30);
            controller.Response.Cookies.Append("uuidc", uuid, cookieOptionss);
            controller.Response.Cookies.Append("cartitems", "0", cookieOptionss);
            controller.Request.Headers["uuidc"] = uuid;
            //setting up mock cookies^

            // Act
            var actionResultTask = controller.Index();
            actionResultTask.Wait();
            int before_ol_count = _context.OrdersList.Include(c => c.Products)
                                             .First()
                                             .Products.Count;
            controller.AddToCart(1);
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.UseDiscountCode("potatos");
            string before_code = _context.OrdersList.Include(c => c.PercentOffCode)
                                             .First()
                                             .PercentOffCode.Code;
            actionResultTask = controller.Index();
            actionResultTask.Wait();
            controller.ClearCart();
            string after_code = _context.OrdersList.Include(c => c.PercentOffCode)
                                             .Where(b => b.UserId == uuid)
                                             .First()
                                             .PercentOffCode.Code;
            var viewResult = actionResultTask.Result as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.AreNotEqual(before_ol_count, null);//its empty trying to get Count will throw error
            Assert.AreNotEqual(before_code, before_code);
        }

        #endregion
    }

    
}
