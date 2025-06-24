using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using buoi08.Controllers;
using buoi08.ADMIN;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace UnitTestProject
{
    public class TestableAdminController : AdminController
    {
        public void SetContext(Doanltweb3Entities2 context)
        {
            typeof(AdminController)
                .GetField("db", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this, context);
        }
    }

    public static class MockDbSetHelper
    {
        public static Mock<DbSet<T>> CreateMockSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            return mockSet;
        }
    }

    [TestClass] //giả lập dữ liệu controller
    public class AdminControllerTests
    {
        private TestableAdminController CreateControllerWithMockContext(
            List<Hoa> hoas = null,
            List<NhaCungCap> nccs = null,
            List<LoaiHoa> loais = null)
        {
            if (hoas == null) hoas = new List<Hoa>();
            if (nccs == null) nccs = new List<NhaCungCap> { new NhaCungCap { MaNCC = 1, TenNCC = "NCC A" } };
            if (loais == null) loais = new List<LoaiHoa> { new LoaiHoa { MaLoai = 1, TenLoai = "Loại A" } };

            var mockHoaSet = MockDbSetHelper.CreateMockSet(hoas);
            var mockNccSet = MockDbSetHelper.CreateMockSet(nccs);
            var mockLoaiSet = MockDbSetHelper.CreateMockSet(loais);

            var mockContext = new Mock<Doanltweb3Entities2>();
            mockContext.Setup(m => m.Hoas).Returns(mockHoaSet.Object);
            mockContext.Setup(m => m.NhaCungCaps).Returns(mockNccSet.Object);
            mockContext.Setup(m => m.LoaiHoas).Returns(mockLoaiSet.Object);

            var controller = new TestableAdminController();
            controller.SetContext(mockContext.Object);
            return controller;
        }

        [TestMethod] //Unit Test
        public void Create_Post_ValidModel_RedirectsToIndex()
        {
            var hoas = new List<Hoa>();
            var controller = CreateControllerWithMockContext(hoas);

            var newHoa = new Hoa
            {
                MaHoa = 3,
                TenHoa = "Hoa Hồng",
                GiaBan = 100,
                MoTa = "Hoa đẹp",
                Anh = "rose.jpg",
                SoLuongTon = 5,
                MaNCC = 1,
                MaLoai = 1
            };

            var result = controller.Create(newHoa) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod] //Unit Test
        public void Create_Post_InvalidModel_ReturnsView()
        {
            var hoa = new Hoa { MaHoa = 2, TenHoa = "" }; // Tên hoa thiếu

            var controller = CreateControllerWithMockContext();
            controller.ModelState.AddModelError("TenHoa", "Tên hoa là bắt buộc");

            var result = controller.Create(hoa) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(hoa, result.Model);
        }

        [TestMethod] //Unit Test
        public void Edit_Post_ValidModel_RedirectsToIndex()
        {
            var hoaList = new List<Hoa>
            {
                new Hoa
                {
                    MaHoa = 1,
                    TenHoa = "Hoa Cũ",
                    GiaBan = 50,
                    MoTa = "Cũ",
                    Anh = "old.jpg",
                    SoLuongTon = 10,
                    MaNCC = 1,
                    MaLoai = 1
                }
            };

            var mockSet = MockDbSetHelper.CreateMockSet(hoaList);

            var mockContext = new Mock<Doanltweb3Entities2>();
            mockContext.Setup(m => m.Hoas).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChanges()).Verifiable();

            var controller = new TestableAdminController();
            controller.SetContext(mockContext.Object);

            var updatedHoa = new Hoa
            {
                MaHoa = 1,
                TenHoa = "Hoa Mới",
                GiaBan = 100,
                MoTa = "Mô tả mới",
                Anh = "hoa_moi.jpg",
                SoLuongTon = 20,
                MaNCC = 1,
                MaLoai = 1
            };

            var result = controller.Edit(updatedHoa) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
            mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [TestMethod] //Unit Test
        public void Edit_Post_InvalidModel_ReturnsView()
        {
            var hoa = new Hoa
            {
                MaHoa = 1,
                TenHoa = "", // Thiếu tên
                GiaBan = 100,
                MoTa = "Mô tả",
                Anh = "hoa.jpg",
                SoLuongTon = 10,
                MaNCC = 1,
                MaLoai = 1
            };

            var controller = CreateControllerWithMockContext();
            controller.ModelState.AddModelError("TenHoa", "Tên hoa là bắt buộc");

            var result = controller.Edit(hoa) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(hoa, result.Model);
        }

        [TestMethod]
        public void DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            var hoaToDelete = new Hoa { MaHoa = 5, TenHoa = "Hoa Xoá" };

            var hoaList = new List<Hoa> { hoaToDelete };

            var mockSet = new Mock<DbSet<Hoa>>();
            mockSet.Setup(m => m.Find(5)).Returns(hoaToDelete);
            mockSet.Setup(m => m.Remove(It.IsAny<Hoa>())).Callback<Hoa>(h => hoaList.Remove(h));

            var mockContext = new Mock<Doanltweb3Entities2>();
            mockContext.Setup(m => m.Hoas).Returns(mockSet.Object);

            var controller = new TestableAdminController();
            controller.SetContext(mockContext.Object);

            var result = controller.DeleteConfirmed(5) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }


        [TestMethod] //Unit Test
        public void DeleteConfirmed_InvalidId_ReturnsRedirectToIndex()
        {
            var mockSet = new Mock<DbSet<Hoa>>();
            mockSet.Setup(m => m.Find(999)).Returns((Hoa)null); // Không có hoa với ID = 999

            var mockContext = new Mock<Doanltweb3Entities2>();
            mockContext.Setup(m => m.Hoas).Returns(mockSet.Object);

            var controller = new TestableAdminController();
            controller.SetContext(mockContext.Object);

            var result = controller.DeleteConfirmed(999) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
