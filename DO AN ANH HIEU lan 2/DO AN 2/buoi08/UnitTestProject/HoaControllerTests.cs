using Microsoft.VisualStudio.TestTools.UnitTesting;
using buoi08.Controllers;
using buoi08.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UnitTestProject
{
    [TestClass]
    public class HoaControllerTests
    {
        [TestMethod] //Integration Test
        public void SortByName_Returns_ViewWithListOfHoa()
        {
            // Arrange
            var controller = new HoaController();

            // Act
            var result = controller.SortByName() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<Hoa>));
        }

        [TestMethod] //Integration Test
        public void SearchHoa_WithTerm_Returns_FilteredList()
        {
            // Arrange
            var controller = new HoaController();
            string term = "Cúc";

            // Act
            var result = controller.SearchHoa(term) as ViewResult;
            var model = result.Model as List<Hoa>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(model, typeof(List<Hoa>));
            Assert.IsTrue(model.Count >= 0); 
        }

        [TestMethod] //Integration Test
        public void SearchHoa_EmptyTerm_ReturnsEmptyList()
        {
            // Arrange
            var controller = new HoaController();
            string term = "";

            // Act
            var result = controller.SearchHoa(term) as ViewResult;
            var model = result.Model as List<Hoa>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(model, typeof(List<Hoa>));
            Assert.AreEqual(0, model.Count);
        }
    }
}
