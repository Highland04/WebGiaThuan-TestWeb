using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using buoi08.Controllers;
using buoi08.Models;
using System.Collections.Generic;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class CartControllerTests
    {
        private CartController controller;
        private Mock<HttpContextBase> mockHttpContext;
        private MockHttpSession mockSession;

        [TestInitialize]
        public void Setup()
        {
            controller = new CartController();
            mockHttpContext = new Mock<HttpContextBase>();
            mockSession = new MockHttpSession();

            var requestContext = new RequestContext(mockHttpContext.Object, new RouteData());
            mockHttpContext.Setup(ctx => ctx.Session).Returns(mockSession);
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object, new RouteData(), controller);
        }

        [TestMethod] //Integration Test
        public void AddToCart_ValidId_AddsItemToCart()
        {
            int id = 1;
            int quantity = 2;

            mockSession["Cart"] = new Cart();

            var flower = new Hoa
            {
                mahoa = id,
                tenhoa = "Hoa Hồng",
                giaban = 10000,
                anhbia = "rose.jpg",
                mota = "Hoa tươi",
                maloai = 1
            };

            var cart = mockSession["Cart"] as Cart;
            cart.AddItem(new CartItem
            {
                ProductId = flower.mahoa,
                ProductName = flower.tenhoa,
                Price = flower.giaban,
                Quantity = quantity
            });

            var result = controller.Cart() as ViewResult;

            var model = result.Model as Cart;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Items.Count);
            Assert.AreEqual("Hoa Hồng", model.Items[0].ProductName);
        }

        [TestMethod] //Integration Test
        public void RemoveFromCart_ValidId_RemovesItem()
        {
            var cart = new Cart();
            cart.AddItem(new CartItem { ProductId = 1, ProductName = "Hoa Hồng", Price = 10000, Quantity = 1 });
            mockSession["Cart"] = cart;

            var result = controller.RemoveFromCart(1) as RedirectToRouteResult;

            var updatedCart = mockSession["Cart"] as Cart;
            Assert.AreEqual(0, updatedCart.Items.Count);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod] //Integration Test
        public void ThanhToan_WithCartItems_ReturnsThanhToanView()
        {
            var cart = new Cart();
            cart.AddItem(new CartItem { ProductId = 1, ProductName = "Hoa Hồng", Price = 10000, Quantity = 1 });
            mockSession["Cart"] = cart;

            var result = controller.ThanhToan(true) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ThanhToan", result.ViewName);
            var model = result.Model as ThanhToanViewModel;
            Assert.AreEqual(1, model.CartItems.Count);
            Assert.AreEqual(10000, model.TongTien);
        }

        [TestMethod] //Integration Test
        public void ThanhToan_WithEmptyCart_ReturnsRedirectToIndex()
        {
            mockSession["Cart"] = new Cart();

            var result = controller.ThanhToan(true) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod] //Integration Test
        public void Cart_ReturnsCurrentCartView()
        {
            var cart = new Cart();
            cart.AddItem(new CartItem { ProductId = 5, ProductName = "Hoa Ly", Price = 20000, Quantity = 3 });
            mockSession["Cart"] = cart;

            var result = controller.Cart() as ViewResult;

            var model = result.Model as Cart;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Items.Count);
            Assert.AreEqual("Hoa Ly", model.Items[0].ProductName);
        }

        [TestMethod] //Integration Test
        public void Index_ReturnsCartItemsList()
        {
            var cart = new Cart();
            cart.AddItem(new CartItem { ProductId = 3, ProductName = "Hoa Mai", Price = 30000, Quantity = 2 });
            mockSession["Cart"] = cart;

            var result = controller.Index() as ViewResult;

            var model = result.Model as List<CartItem>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Hoa Mai", model[0].ProductName);
        }
    }
}
