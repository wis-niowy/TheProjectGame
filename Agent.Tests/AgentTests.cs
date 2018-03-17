using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheProjectGame.GameArea;

namespace Player.Tests
{
    [TestClass]
    public class AgentTests
    {

        [TestMethod]
        public void NewAgent()
        {
            var agent = new Agent(Team.Blue);
            Assert.IsNotNull(agent);
            Assert.AreEqual(0, agent.GetLocation.X);
            Assert.AreEqual(0, agent.GetLocation.Y);
            Assert.AreEqual(Team.Blue, agent.GetTeam);
            Assert.AreEqual(0, agent.GUID);
        }

        [TestMethod]
        public void GuidSet()
        {
            var agent = new Agent(Team.Blue);
            agent.SetGuid(5);
            Assert.AreEqual(5, agent.GUID);
        }

        [TestMethod]
        public void BoardSet()
        {
            var board = new Board(2, 2, 2);
            var agent = new Agent(Team.Blue);
            Assert.IsNull(agent.GetBoard);
            agent.SetBoard(board);
            Assert.IsNotNull(agent.GetBoard);
            Assert.AreSame(board, agent.GetBoard);
        }

        [TestMethod]
        public void SetLocation()
        {
            var agent = new Agent(Team.Blue);
            var newLocation = new Point(2, 3);
            agent.SetLocation(newLocation);
            Assert.AreEqual(newLocation.X, agent.GetLocation.X);
            Assert.AreEqual(newLocation.Y, agent.GetLocation.Y);
        }
    }
}
