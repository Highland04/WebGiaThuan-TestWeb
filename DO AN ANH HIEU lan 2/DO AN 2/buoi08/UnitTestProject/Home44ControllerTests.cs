using Microsoft.VisualStudio.TestTools.UnitTesting;
using buoi08.Controllers;
using buoi08.Models;
using System.Web.Mvc;
using Moq;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UnitTestProject
{
    [TestClass]
    public class Home44ControllerTests
    {
        private Home44Controller controller;
        private MockHttpSession session;

        [TestInitialize]
        public void Setup()
        {
            controller = new Home44Controller();
            session = new MockHttpSession();
        }
        public class ThanhVien
        {
            public string Tendangnhap { get; set; }
            public string Matkhau { get; set; }
        }

        [TestMethod] //Integration Test
        public void Login_WithValidCredentials_ReturnsRedirectToIndex()
        {
            // Arrange
            var controller = new Home44Controller();
            var context = new Mock<HttpContextBase>();
            var session = new MockHttpSession();

            var form = new NameValueCollection
    {
        { "username", "nguyenvana@example.com" },
        { "password", "password123" }
    };

            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Form).Returns(form);

            context.Setup(c => c.Session).Returns(session);
            context.Setup(c => c.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            // Act
            var result = controller.Login() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod] //Integration Test
        public void Login_WithInvalidCredentials_ReturnsViewWithMessage()
        {
            var request = new Mock<HttpRequestBase>();
            var form = new NameValueCollection
            {
                { "username", "invaliduser" },
                { "password", "wrongpass" }
            };
            request.Setup(r => r.Form).Returns(form);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);
            context.Setup(c => c.Request).Returns(request.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            var result = controller.Login() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Thông tin sai!", controller.ViewBag.Message);
        }

        [TestMethod] //Integration Test
        public void Register_WithValidData_ReturnsSuccess()
        {
            var form = new FormCollection
            {
                { "hoten", "Test User" },
                { "date", "2000-01-01" },
                { "gender", "Nam" },
                { "dienthoai", "0123456789" },
                { "email", $"test{System.Guid.NewGuid()}@example.com" },
                { "diachi", "HCM" },
                { "psw", "123456" },
                { "vaitro", "user" }
            };

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);
            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            var result = controller.Register(form) as ViewResult;

            Assert.IsNotNull(result);
            StringAssert.Contains(controller.ViewBag.Message.ToString(), "Đăng ký thành công");
        }

        [TestMethod] //Integration Test
        public void Register_WithMissingFields_ReturnsErrorMessage()
        {
            // Arrange: thiếu Email và Date
            var form = new FormCollection
    {
        { "hoten", "Thiếu Email" },
        { "gender", "Nam" },
        { "dienthoai", "0123456789" },
        { "diachi", "HCM" },
        { "psw", "123456" },
        { "vaitro", "user" }
        // thiếu email + date
    };

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(new MockHttpSession());

            var controller = new Home44Controller();
            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            // Act
            var result = controller.Register(form) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            StringAssert.Contains(controller.ViewBag.Message, "Có lỗi xảy ra");
        }

        [TestMethod] //Integration Test
        public void ThanhToan_WithItemsInCart_ReturnsViewModel()
        {
            var cart = new Cart();
            cart.AddItem(new CartItem
            {
                ProductId = 1,
                ProductName = "Hoa Hồng",
                Price = 100000,
                Quantity = 2
            });
            session["Cart"] = cart;

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Session).Returns(session);
            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            var result = controller.ThanhToan() as ViewResult;

            Assert.IsNotNull(result);
            var model = result.Model as ThanhToanViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(200000, model.TongTien);
            Assert.AreEqual(1, model.CartItems.Count);
        }

        [TestMethod] //Integration Test
        public void ThanhToan_WithEmptyCart_RedirectsToIndex()
        {
            // Arrange
            var controller = new Home44Controller();
            var context = new Mock<HttpContextBase>();
            var session = new MockHttpSession(); // Cart chưa được thêm gì
            context.Setup(c => c.Session).Returns(session);
            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            // Act
            var result = controller.ThanhToan() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
