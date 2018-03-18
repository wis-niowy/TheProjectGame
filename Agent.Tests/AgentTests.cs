using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;

namespace Player.Tests
{
    [TestClass]
    public class AgentTests
    {

        [TestMethod]
        public void NewAgent()
        {
            var agent = new Agent(TeamColour.blue);
            Assert.IsNotNull(agent);
            Assert.AreEqual(0u, agent.GetLocation.x);
            Assert.AreEqual(0u, agent.GetLocation.y);
            Assert.AreEqual(TeamColour.blue, agent.GetTeam);
            Assert.AreEqual(0ul, agent.GUID);
        }

        [TestMethod]
        public void GuidSet()
        {
            var agent = new Agent(TeamColour.blue);
            agent.SetGuid(5u);
            Assert.AreEqual(5u, agent.GUID);
        }

        [TestMethod]
        public void BoardSet()
        {
            var board = new Board(2u, 2u, 2u);
            var agent = new Agent(TeamColour.blue);
            Assert.IsNull(agent.GetBoard);
            agent.SetBoard(board);
            Assert.IsNotNull(agent.GetBoard);
            Assert.AreSame(board, agent.GetBoard);
        }

        [TestMethod]
        public void SetLocation()
        {
            var agent = new Agent(TeamColour.blue);
            var newLocation = new Location(2u, 3u);
            agent.SetLocation(newLocation);
            Assert.AreEqual(newLocation.x, agent.GetLocation.x);
            Assert.AreEqual(newLocation.y, agent.GetLocation.y);
        }
    }
}
