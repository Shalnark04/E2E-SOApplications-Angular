using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Common.Tests
{
    [TestClass]
    public class ObjectBaseTests
    {

        [TestMethod]
        public void test_clean_property_change()
        {
            TestClass objTest = new TestClass();
            bool propertyChanged = false;

            objTest.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "CleanProp")
                    propertyChanged = true;
            };

            objTest.CleanProp = "test value";

            Assert.IsTrue(propertyChanged, "The property should have triggered a change notification");
        }
    }
}
