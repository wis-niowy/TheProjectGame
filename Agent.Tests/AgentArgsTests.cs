using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Tests
{
    public partial class PlayerTests
    {

        [TestMethod]
        public void CorrectArgumentsLine1()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "red", "member", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsTrue(validateResult);
        }

        [TestMethod]
        public void CorrectArgumentsLine2()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "red", "leader", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsTrue(validateResult);
        }

        [TestMethod]
        public void CorrectArgumentsLine3()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "blue", "leader", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsTrue(validateResult);
        }

        [TestMethod]
        public void CorrectArgumentsLine4()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "red", "member", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsTrue(validateResult);
        }

        [TestMethod]
        public void InsufficientArgsNumber()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "red", "member"};

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsFalse(validateResult);
        }

        [TestMethod]
        public void PortNumberLessThanZero()
        {
            string[] args = new string[] { "192.168.0.0", "-3", "red", "member", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsFalse(validateResult);
        }

        [TestMethod]
        public void IncorrectTeamColorFormat()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "redd", "member", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsFalse(validateResult);
        }

        [TestMethod]
        public void IncorrectRoleFormat()
        {
            string[] args = new string[] { "192.168.0.0", "5678", "red", "meber", "settings.xml" };

            var validateResult = MainPlayer.ValidateArgs(args);

            Assert.IsFalse(validateResult);
        }
    }
}
