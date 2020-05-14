using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pellio.Controllers;
using Pellio.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pellio.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace UnitTest
{
   
    [TestClass]
    public class UnitTest1
    {
       

        [TestMethod]
        public void IndexReturnsView()
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
    }
}
