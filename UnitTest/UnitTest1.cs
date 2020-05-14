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

namespace UnitTest
{

    [TestClass]
    public class UnitTest1
    {

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
            OrdersListsController controller = new OrdersListsController(null);

            // Act
            Task<IActionResult> result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        private void PercentOffCodeTextIsGenerated()
        {
            // Arrange
            OrdersListsController controller = new OrdersListsController(null);

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

            OrdersListsController controller = new OrdersListsController(_context);
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

        #endregion
    }

    
}
