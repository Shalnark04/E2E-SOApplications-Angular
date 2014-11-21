using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [TestMethod]
        public void test_dirty_set()
        {
            TestClass objTest = new TestClass();
            Assert.IsFalse(objTest.IsDirty, "Object should be clean & pure as the driven snow.");

            objTest.DirtyProp = "test value";
            Assert.IsTrue(objTest.IsDirty, "Object should be dirty.");
        }

        //[TestMethod]
        //public void test_property_change_single_subscription()
        //{
        //    TestClass objTest = new TestClass();
        //    int changeCounter = 0;
        //    PropertyChangedEventHandler handler1 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });
        //    PropertyChangedEventHandler handler2 = new PropertyChangedEventHandler((s, e) => { changeCounter++; });

        //    objTest.PropertyChanged += handler1;
        //    objTest.PropertyChanged += handler1;

        //    // TEST UNFINISHED
        //}
    }
}
