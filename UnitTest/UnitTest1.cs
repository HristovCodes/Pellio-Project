using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pellio.Controllers;
using Pellio.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pellio.Data;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pellio.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
    }
}
