using System.Collections.Generic;
using System.Web;

namespace UnitTestProject
{
    public class MockHttpSession : HttpSessionStateBase
    {
        private Dictionary<string, object> sessionStorage = new Dictionary<string, object>();

        public override object this[string name]
        {
            get => sessionStorage.ContainsKey(name) ? sessionStorage[name] : null;
            set => sessionStorage[name] = value;
        }
    }
}
