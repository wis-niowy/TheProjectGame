using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TheProjectGame.GameArea;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class PointTests
    {
        [TestMethod]
        public void NewPoint()
        {
            var point = new Point(0,0);
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
        }
        [TestMethod]
        public void NewCustomPoint()
        {
            var point = new Point(1, 2);
            Assert.AreEqual(1, point.X);
            Assert.AreEqual(2, point.Y);
        }
    }
}
