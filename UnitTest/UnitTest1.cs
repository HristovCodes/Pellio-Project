using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pellio.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pellio.Data;
using System.Linq;

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
        public void DataGivenToSelectIsCorrect()
        {
            // Arrange
            ProductsController controller = new ProductsController(null);
            //controller.FillDropDownTags();
            PellioContext _context;
            IEnumerable<SelectListItem> tags = _context.Products.Select(t => new SelectListItem
            {
                Value = t.Tag,
                Text = t.Tag
            }).Distinct();
            Assert.AreEqual(controller.ViewBag.TagsforDropdown, tags);
        }
    }
}
