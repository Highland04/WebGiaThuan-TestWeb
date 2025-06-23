using Microsoft.VisualStudio.TestTools.UnitTesting;
using buoi08.Controllers;
using System.Web.Mvc;

namespace UnitTestProject
{
    [TestClass]
    public class MoTaControllerTests
    {
        private MoTaController controller;

        [TestInitialize]
        public void Setup()
        {
            controller = new MoTaController();
        }

        [TestMethod] //Integration Test
        public void SP000001_Returns_View()
        {
            var result = controller.SP000001() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod] //Integration Test
        public void SP000099_Returns_View()
        {
            var result = controller.SP000099() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod] //Unit Test
        public void SP000050_DoesNotExist_ShouldReturnNull()
        {
            // Controller không có SP000050 nên ta gọi thông qua Reflection để chứng minh không tồn tại
            var method = typeof(MoTaController).GetMethod("SP000050");
            Assert.IsNull(method);
        }

        [TestMethod] //Integration Test
        public void All_Existing_Actions_Return_View()
        {
            string[] actionNames = new string[]
            {
                "SP000001", "SP000002", "SP000003", "SP000004", "SP000005",
                "SP000006", "SP000007", "SP000008", "SP000009", "SP000010",
                "SP000011", "SP000012", "SP000013", "SP000014", "SP000015",
                "SP000016", "SP000017", "SP000018", "SP000019", "SP000020",
                "SP000021", "SP000022", "SP000023", "SP000024", "SP000025",
                "SP000026", "SP000027", "SP000028", "SP000029", "SP000030",
                "SP000031", "SP000032", "SP000033", "SP000034", "SP000035",
                "SP000036", "SP000037", "SP000038", "SP000039", "SP000040",
                "SP000041", "SP000042", "SP000043", "SP000044", "SP000045",
                "SP000046", "SP000047", "SP000088", "SP000089", "SP000090",
                "SP000091", "SP000092", "SP000093", "SP000094", "SP000095",
                "SP000096", "SP000097", "SP000098", "SP000099"
            };

            foreach (string action in actionNames)
            {
                var method = typeof(MoTaController).GetMethod(action);
                Assert.IsNotNull(method, $"Method {action} should exist.");

                var result = method.Invoke(controller, null) as ViewResult;
                Assert.IsNotNull(result, $"View for {action} should not be null.");
            }
        }
    }
}
