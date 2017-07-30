using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkObservability;
using NetworkObservabilityCore;

namespace UnitTestProject1
{
    [TestClass]
    public class ArcTest
    {
        private Arc arc;

        [TestInitialize]
        public void Initialization()
        {
            arc = new Arc();
        }

        [TestMethod]
        public void TestChangeName()
        {
            arc.Name = "Test";
            Assert.AreEqual("Test", arc.Name);
        }

        [TestMethod]
        public void TestDefaultNodePointsIsNull()
        {
            arc
        }

    }
}
