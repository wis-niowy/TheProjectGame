using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheProjectGame.GameArea;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void BoardWidth()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5, board.BoardWidth);
        }

        [TestMethod]
        public void BoardPieceAreaHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5, board.PieceAreaHeight);
        }

        [TestMethod]
        public void BoardGoalAreaHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(3, board.GoalAreaHeight);
        }

        [TestMethod]
        public void BoardHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5 + 2 * 3, board.BoardHeight);
        }
    }
}
